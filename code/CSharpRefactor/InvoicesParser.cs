using System;
using System.Collections.Generic;
using System.Linq;

namespace CSharpRefactor
{
    public class InvoicesParser
    {
        private class InvoiceContents
        {
            public string[] Contents { get; }
            public string ErrorText { get; }

            public InvoiceContents(string[] contents, string errorText)
            {
                Contents = contents;
                ErrorText = errorText;
            }
        }
        
        private static InvoiceContents ReadInvoice(string filePath)
        {
            try
            {
                var contents = System.IO.File.ReadLines(filePath).ToArray();
                return new InvoiceContents(contents, null);
            }
            catch (Exception e)
            {
                return new InvoiceContents(new string[0], e.Message);
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

        private static bool HasError(InvoiceContents invoiceContents)
        {
            return invoiceContents.ErrorText != null;
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
                    parsedResults.Add(filePath, new InvoiceParseResult(nextId++, invoiceContent.ErrorText)); 
                }
                else
                {
                    if (IsContentLengthValid(invoiceContent.Contents))
                    {
                        if (Decimal.TryParse(invoiceContent.Contents[0], out var invoiceAmount))
                        {
                            var discountedAmount = ApplyDiscount(invoiceAmount, discountPercentage, isDiscountAllowed);

                            parsedResults.Add(filePath,
                                new InvoiceParseResult(nextId++, invoiceAmount, discountedAmount));
                        }
                        else
                        {
                            parsedResults.Add(filePath,
                                new InvoiceParseResult(nextId++,
                                    $"Invoice amount {invoiceContent.Contents[0]} could not be parsed"));
                        }
                    }
                }
            }
            
            return parsedResults;
        }
    }
}