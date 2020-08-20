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

        private static bool IsContentLengthValid(IEnumerable<string> contents)
        {
            return contents.Count() == 1;
        }

        private static bool HasError<T>(InterimResult<T> interimResult)
        {
            return interimResult.ErrorText != null;
        }

        private static InterimResult<Option<decimal>> ParseLine(string line)
        {
            if (System.Decimal.TryParse(line, out var invoiceAmount))
            {
                return new InterimResult<Option<decimal>>(Some(invoiceAmount));
            }

            return new InterimResult<Option<decimal>>(None, $"Invoice amount {line} could not be parsed");
        }

        public static InvoicesSum ParseInvoices(
            IEnumerable<KeyValuePair<string, InterimResult<IEnumerable<string>>>> rawInvoices,
            Option<decimal> discountPercentage,
            Option<bool> isDiscountAllowed)
        {
            return rawInvoices
                .Where(filePathAndInvoice =>
                {
                    var invoiceContent = filePathAndInvoice.Value;
                    return !HasError(invoiceContent);
                })
                .Where(filePathAndInvoice =>
                {
                    var invoiceContent = filePathAndInvoice.Value;
                    return IsContentLengthValid(invoiceContent.Contents);
                })
                .Select((filePathAndInvoice, index) =>
                {
                    var filePath = filePathAndInvoice.Key;
                    var invoiceContent = filePathAndInvoice.Value;

                    var parsedLine = ParseLine(invoiceContent.Contents.First());

                    return parsedLine.Contents.Match(
                        None: () => new InvoiceParseResult(index, parsedLine.ErrorText),
                        Some:
                            invoiceAmount =>
                            {
                                var discountedAmount =
                                    Some(ApplyDiscount)
                                        .Apply(Some(invoiceAmount))
                                        .Apply(discountPercentage)
                                        .Apply(isDiscountAllowed);

                                return new InvoiceParseResult(index, invoiceAmount, discountedAmount);
                            });
                })
                .Aggregate(new InvoicesSum(0, 0), InvoiceSum.AggregateSum);
        }
    }
}