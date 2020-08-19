using System;
using System.Collections.Generic;
using System.Linq;

namespace CSharpRefactor
{
    public class InvoicesParser
    {
        private readonly IEnumerable<string> _invoiceFilePaths;
        private readonly decimal? _discountPercentage;
        private readonly bool? _isDiscountAllowed;

        public InvoicesParser(IEnumerable<string> invoiceFilePaths, decimal? discountPercentage, bool? isDiscountAllowed)
        {
            _invoiceFilePaths = invoiceFilePaths;
            _discountPercentage = discountPercentage;
            _isDiscountAllowed = isDiscountAllowed;
        }

        public IEnumerable<KeyValuePair<string, InvoiceParseResult>> ReadAndParseInvoices()
        {
            var parsedResults = new Dictionary<string, InvoiceParseResult>();
            var nextId = 0;
            
            foreach (var filePath in _invoiceFilePaths)
            {
                try
                {
                    var contents = System.IO.File.ReadLines(filePath).ToArray();

                    if (contents.Length == 1)
                    {
                        if (Decimal.TryParse(contents[0], out var invoiceAmount))
                        {
                            decimal? discountedAmount = null;
                            if (_discountPercentage.HasValue 
                                && _isDiscountAllowed.HasValue 
                                && _isDiscountAllowed.Value)
                            {
                                discountedAmount = invoiceAmount * _discountPercentage;
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
                    continue;
                }
            }
            
            return parsedResults;
        }
    }
}