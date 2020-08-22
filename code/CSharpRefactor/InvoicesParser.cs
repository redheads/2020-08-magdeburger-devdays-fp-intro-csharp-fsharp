using System;
using System.Collections.Generic;
using System.Linq;
using LaYumba.Functional;
using static LaYumba.Functional.F;

namespace CSharpRefactor
{
    public static class InvoicesParser
    {
        public static Validation<InvoicesSum> ParseInvoices(
            IEnumerable<Validation<IEnumerable<string>>> rawInvoices,
            Option<decimal> discountPercentage,
            Option<bool> isDiscountAllowed)
        {
            var parsedInvoices = 
                rawInvoices
                    .Select((rawInvoice, index) => 
                        rawInvoice
                            .Bind(IsContentLengthValid)
                            .Map(GetFirstLine)
                            .Bind(ParseLine)
                            .Map(CalculateAndApplyDiscount(discountPercentage, isDiscountAllowed, index))
                    ).ToArray();

            var collectedErrors = CollectErrors(parsedInvoices).ToArray();

            return collectedErrors.Any()
                ? Invalid(collectedErrors)
                : Valid(CalculateInvoicesSum(parsedInvoices));
        }
        
        private static readonly Func<decimal, decimal, bool, decimal> ApplyDiscount = 
            (invoiceAmount, discountPercentage, isDiscountAllowed) =>
                isDiscountAllowed 
                    ? invoiceAmount - (invoiceAmount * (discountPercentage / 100m)) 
                    : invoiceAmount;

        private static Validation<IEnumerable<string>> IsContentLengthValid(IEnumerable<string> contents)
        {
            var enumeratedContents = contents.ToArray(); 
            return enumeratedContents.Length == 1
                ? Valid((IEnumerable<string>) enumeratedContents)
                : Invalid(Error("Line count not exactly 1"));
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

        private static InvoicesSum CalculateInvoicesSum(IEnumerable<Validation<InvoiceParseResult>> parsedInvoices)
        {
            return parsedInvoices.Aggregate(new InvoicesSum(0, 0),
                (acc, invoice) =>
                {
                    return invoice.Match(
                        Invalid: (_) => acc,
                        Valid: (i) => InvoiceSum.AggregateSum(acc, i));
                });
        }

        private static IEnumerable<Error> CollectErrors(IEnumerable<Validation<InvoiceParseResult>> parsedInvoices)
        {
            return parsedInvoices.SelectMany(parsedInvoice =>
            {
                return parsedInvoice.Match(
                    Invalid: errors => errors,
                    Valid: (_) => new Error[0]);
            });
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