using System;
using System.Collections.Generic;
using System.Linq;

namespace CSharpRefactor
{
    public class InvoicesParser
    {
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

        public static IEnumerable<KeyValuePair<string, InvoiceParseResult>> ParseInvoices(IEnumerable<KeyValuePair<string, InterimResult<IEnumerable<string>>>> rawInvoices,
            decimal? discountPercentage, 
            bool? isDiscountAllowed)
        {
            var parsedResults = new Dictionary<string, InvoiceParseResult>();
            var nextId = 0;
            
            foreach (var kv in rawInvoices)
            {
                var filePath = kv.Key;
                var invoiceContent = kv.Value;
                
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