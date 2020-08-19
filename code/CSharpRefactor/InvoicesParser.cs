using System;
using System.Collections.Generic;
using System.Linq;

namespace CSharpRefactor
{
    public class InvoicesParser
    {
        private class InterimResult<T>
        {
            public T Contents { get; }
            public string ErrorText { get; }

            public InterimResult(T contents)
            {
                Contents = contents;
            }
            
            public InterimResult(string errorText)
            {
                Contents = default(T);
                ErrorText = errorText;
            }
        }
        
        private static InterimResult<IEnumerable<string>> ReadInvoice(string filePath)
        {
            try
            {
                var contents = System.IO.File.ReadLines(filePath).ToArray();
                return new InterimResult<IEnumerable<string>>(contents);
            }
            catch (Exception e)
            {
                return new InterimResult<IEnumerable<string>>(e.Message);
            }
        }

        private static decimal? ApplyDiscount(decimal invoiceAmount, decimal? discountPercentage, bool? isDiscountAllowed)
        {
            decimal? discountedAmount = null;
            if (discountPercentage.HasValue
                && isDiscountAllowed.HasValue
                && isDiscountAllowed.Value)
            {
                discountedAmount = invoiceAmount - (invoiceAmount * (discountPercentage / 100m));
            }

            return discountedAmount;
        }

        private static bool IsContentLengthValid(IEnumerable<string> contents)
        {
            return contents.Count() == 1;
        }

        private static bool HasError<T>(InterimResult<T> interimResult)
        {
            return interimResult.ErrorText != null;
        }

        private static InterimResult<decimal?> ParseLine(string line)
        {
            if (Decimal.TryParse(line, out var invoiceAmount))
            {
                return new InterimResult<decimal?>(invoiceAmount);
            }
            
            return new InterimResult<decimal?>($"Invoice amount {line} could not be parsed");
        }
        
        public static IEnumerable<KeyValuePair<string, InvoiceParseResult>> ReadAndParseInvoices(IEnumerable<string> invoiceFilePaths, decimal? discountPercentage, bool? isDiscountAllowed)
        {
            var parsedResults = new Dictionary<string, InvoiceParseResult>();
            var nextId = 0;
            
            foreach (var filePath in invoiceFilePaths)
            {
                var invoiceContent = ReadInvoice(filePath);

                if (HasError(invoiceContent))
                {
                    parsedResults.Add(filePath, new InvoiceParseResult(nextId, invoiceContent.ErrorText)); 
                }
                else
                {
                    if (IsContentLengthValid(invoiceContent.Contents))
                    {
                        var parsedLine = ParseLine(invoiceContent.Contents.First());
                        
                        if (parsedLine.Contents.HasValue)
                        {
                            var invoiceAmount = parsedLine.Contents.Value;
                            var discountedAmount = ApplyDiscount(invoiceAmount, discountPercentage, isDiscountAllowed);

                            parsedResults.Add(filePath,
                                new InvoiceParseResult(nextId, invoiceAmount, discountedAmount));
                        }
                        else
                        {
                            parsedResults.Add(filePath,
                                new InvoiceParseResult(nextId, parsedLine.ErrorText));
                        }
                    }
                }

                nextId++;
            }
            
            return parsedResults;
        }
    }
}