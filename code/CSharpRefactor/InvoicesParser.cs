using System;
using System.Collections.Generic;
using System.Linq;
using LaYumba.Functional;
using static LaYumba.Functional.F;

namespace CSharpRefactor
{
    public class InvoicesParser
    {
        private static readonly Func<decimal, decimal, bool, decimal> ApplyDiscount = (decimal invoiceAmount, 
            decimal discountPercentage,
            bool isDiscountAllowed) =>
                isDiscountAllowed 
                    ? invoiceAmount - (invoiceAmount * (discountPercentage / 100m)) 
                    : invoiceAmount;

        private static Validation<IEnumerable<string>> IsContentLengthValid(IEnumerable<string> contents)
        {
            return contents.Count() == 1
                ? Valid(contents)
                : Invalid(Error("Line count not exactly 1"));
        }

        private static bool HasError<T>(InterimResult<T> interimResult)
        {
            return interimResult.ErrorText != null;
        }

        private static Validation<decimal> ParseLine(string line)
        {
            return decimal.TryParse(line, out var invoiceAmount) 
                ? Valid(invoiceAmount) 
                : Invalid(Error($"Invoice amount {line} could not be parsed"));
        }

        private static string GetFirstLine(IEnumerable<string> lines)
        {
            return lines.First();
        }

        public static Validation<InvoicesSum> ParseInvoices(
            IEnumerable<Validation<IEnumerable<string>>> rawInvoices,
            Option<decimal> discountPercentage,
            Option<bool> isDiscountAllowed)
        {
            var parsedInvoices = rawInvoices
                .Select((rawInvoice, index) => 
                    rawInvoice
                        .Bind(IsContentLengthValid)
                        .Map(GetFirstLine)
                        .Bind(ParseLine)
                        .Map(CalculateAndApplyDiscount(discountPercentage, isDiscountAllowed, index))
                );
            
            
            

            var hasErrors = parsedInvoices.Any(parsedInvoice => !parsedInvoice.IsValid);
            if (hasErrors)
            {
                IEnumerable<Option<IEnumerable<Error>>> e = parsedInvoices.Map(pi =>
                {
                    var k = 
                    pi.Match(
                        Invalid: (error) => Some(error),
                        Valid: (_) => None
                    );

                    return k;

                });

                return e.Count() == 0
                    ? Valid(new InvoicesSum(-1, -1))
                    : Invalid(e.Flatten());
            }

            return Valid(parsedInvoices.Bind(InvoiceSum.Sum));

        }

        
        private static Func<decimal, InvoiceParseResult> CalculateAndApplyDiscount(Option<decimal> discountPercentage, Option<bool> isDiscountAllowed, int id)
        {
            return (invoiceAmount) =>
            {
                var discountedAmount = Some(ApplyDiscount)
                    .Apply(Some(invoiceAmount))
                    .Apply(discountPercentage)
                    .Apply(isDiscountAllowed);

                return new InvoiceParseResult(id, invoiceAmount, discountedAmount);
            };
        }
    }
}