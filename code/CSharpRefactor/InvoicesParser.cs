using System;
using System.Collections.Generic;
using System.Linq;

namespace CSharpRefactor
{
    public class InvoicesParser
    {
       
        public static IEnumerable<KeyValuePair<string, InvoiceParseResult>> ReadAndParseInvoices(IEnumerable<string> invoiceFilePaths, decimal? discountPercentage, bool? isDiscountAllowed)
        {
            var parsedResults = new Dictionary<string, InvoiceParseResult>();
            var nextId = 0;
            
            foreach (var filePath in invoiceFilePaths)
            {
                try
                {
                    var contents = System.IO.File.ReadLines(filePath).ToArray();

                    if (contents.Length == 1)
                    {
                        if (Decimal.TryParse(contents[0], out var invoiceAmount))
                        {
                            decimal? discountedAmount = null;
                            if (discountPercentage.HasValue 
                                && isDiscountAllowed.HasValue 
                                && isDiscountAllowed.Value)
                            {
                                discountedAmount = invoiceAmount - (invoiceAmount * (discountPercentage / 100m));
                            }
                    
                            parsedResults.Add(filePath, new InvoiceParseResult(nextId++, invoiceAmount, discountedAmount));   
                        }
                        else
                        {
                            parsedResults.Add(filePath, new InvoiceParseResult(nextId++, $"Invoice amount {contents.First()} could not be parsed"));
                        }
                    }
                }
                catch (Exception e)
                {
                    parsedResults.Add(filePath, new InvoiceParseResult(nextId++, e.Message)); 
                }
            }
            
            return parsedResults;
        }
    }
}