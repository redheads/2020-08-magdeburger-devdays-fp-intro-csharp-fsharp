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

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                // Suitable nullity checks etc, of course :)
                if (Amount.HasValue)
                {
                    hash = hash * 23 + Amount.Value.GetHashCode();
                }
                
                if (DiscountedAmount.HasValue)
                {
                    hash = hash * 23 + DiscountedAmount.Value.GetHashCode();
                }
                
                if (ErrorText != null)
                {
                    hash = hash * 23 + ErrorText.GetHashCode();
                }
                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            var item = obj as ParseResult;

            if (item == null)
            {
                return false;
            }

            return 
                (Amount.HasValue && item.Amount.HasValue && Amount.Equals(item.Amount))
                || (!Amount.HasValue && !item.Amount.HasValue)
                && (DiscountedAmount.HasValue && item.DiscountedAmount.HasValue && DiscountedAmount.Equals(item.DiscountedAmount))
                || (!DiscountedAmount.HasValue && !item.DiscountedAmount.HasValue)
                && ErrorText.Equals(item.ErrorText);
        }
    }
    
    public class Invoices
    {
        public static Dictionary<string, ParseResult> ReadInvoices(IEnumerable<string> invoiceFilePaths, decimal? discountPercentage, Func<bool?> isDiscountAllowed)
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
                    continue;
                }

                if (contents.Count() < 1)
                {
                    parsedResults.Add(filePath, new ParseResult("File is empty"));  
                    continue;
                }
                
                if (contents.Count() > 1)
                {
                    parsedResults.Add(filePath, new ParseResult("File has too many lines"));  
                    continue;
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
                else
                {
                    parsedResults.Add(filePath, new ParseResult($"Invoice amount {contents.First()} could not be parsed"));
                }
            }
            
            return parsedResults;
        }
    }
}
