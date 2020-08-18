using System;
using System.Collections.Generic;
using System.Linq;

namespace CSharpRefactor
{
    public class ParseResult
    {
        public decimal? Amount;
        public decimal? DiscountedAmount;
        public string ErrorText;

        public ParseResult(decimal? amount, decimal? discountedAmount)
        {
            Amount = amount;
            DiscountedAmount = discountedAmount;
            ErrorText = null;
        }
        
        public ParseResult(string errorText)
        {
            Amount = null;
            DiscountedAmount = null;
            ErrorText = errorText;
        }
    }
    
    public class Invoices
    {
        public Dictionary<string, ParseResult> ReadInvoices(IEnumerable<string> invoiceFilePaths, decimal? discountPercentage, Func<bool?> isDiscountAllowed)
        {
            var parsedResults = new Dictionary<string, ParseResult>();
            
            foreach (var filePath in invoiceFilePaths)
            {
                IEnumerable<string> contents = new string[] {};
                
                try
                {
                    contents = System.IO.File.ReadLines(filePath);
                }
                catch (Exception e)
                {
                    parsedResults.Add(filePath, new ParseResult(e.Message));                    
                }

                if (contents.Count() < 1)
                {
                    parsedResults.Add(filePath, new ParseResult("File is empty"));  
                }
                
                if (contents.Count() > 1)
                {
                    parsedResults.Add(filePath, new ParseResult("File has too many lines"));  
                }

                if (Decimal.TryParse(contents.First(), out var invoiceAmount))
                {
                    decimal? discountedAmount = null;
                    bool? discountAllowed = isDiscountAllowed();
                    if (discountPercentage.HasValue 
                        && discountAllowed.HasValue 
                        && discountAllowed.Value)
                    {
                        discountedAmount = invoiceAmount * discountPercentage;
                    }
                    
                    parsedResults.Add(filePath, new ParseResult(invoiceAmount, discountedAmount));   
                }
            }
            
            return parsedResults;
        }
    }
}
