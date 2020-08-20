using System;
using System.Collections.Generic;
using System.Linq;

namespace CSharpRefactor
{
    public class InvoicesParser
    {
        private static decimal? ApplyDiscount(decimal invoiceAmount, decimal? discountPercentage,
            bool? isDiscountAllowed)
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

        public static InvoicesSum ParseInvoices(
            IEnumerable<KeyValuePair<string, InterimResult<IEnumerable<string>>>> rawInvoices,
            decimal? discountPercentage,
            bool? isDiscountAllowed)
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

                    if (parsedLine.Contents.HasValue)
                    {
                        var invoiceAmount = parsedLine.Contents.Value;
                        var discountedAmount = ApplyDiscount(invoiceAmount, discountPercentage, isDiscountAllowed);

                        return new InvoiceParseResult(index, invoiceAmount, discountedAmount);
                    }
                    else
                    {
                        return new InvoiceParseResult(index, parsedLine.ErrorText);
                    }
                })
                .Aggregate(new InvoicesSum(0, 0), InvoiceSum.AggregateSum);
        }
    }
}