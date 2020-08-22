namespace FSharpRefactor

open System
open System.Collections.Generic
open System.Linq


module InvoiceParser =
    type InvoiceSum = {
        sum: decimal
        discountedSum: decimal
    }
    
    type ParseResult = {
        errorText: string
        amount: Nullable<decimal>
        discountedAmount: Nullable<decimal>
    }
    
    type SumOrErrors = {
        invoiceSum: InvoiceSum
        errors: IEnumerable<string>
    }
    
    let parseInvoices (paths : IEnumerable<string>) (discountPercentage : Nullable<decimal>) (isDiscountAllowed : Nullable<bool>) : SumOrErrors =
        let parsedInvoices = List<ParseResult>()
        
        for filePath in paths do
            let parsedInvoice =
                try
                    let lines = System.IO.File.ReadLines(filePath)
                
                    if lines.Count() = 1 then
                        let success, parsedAmount = Decimal.TryParse(lines.First())
                        if success then
                            let discounted =
                                if discountPercentage.HasValue
                                    && isDiscountAllowed.HasValue
                                    && isDiscountAllowed.Value then
                                        Nullable<decimal>(parsedAmount - (parsedAmount * (discountPercentage.Value / 100m)))
                                else
                                    Nullable<decimal>()
                            
                            {
                                ParseResult.errorText = null 
                                amount = Nullable<decimal>(parsedAmount)
                                discountedAmount = discounted
                            }
                        else
                            { ParseResult.errorText = "The file content could not be parsed"
                              amount = Nullable<decimal>()
                              discountedAmount = Nullable<decimal>() }
                    else
                        {   ParseResult.errorText = "The file must have exactly one line"
                            amount = Nullable<decimal>()
                            discountedAmount = Nullable<decimal>() }
                with
                | ex -> {
                                          ParseResult.errorText = ex.Message
                                          amount = Nullable<decimal>()
                                          discountedAmount = Nullable<decimal>() }
            parsedInvoices.Add(parsedInvoice)
                
        if parsedInvoices.Any(fun invoice -> not (String.IsNullOrEmpty(invoice.errorText))) then
            {
                invoiceSum = {
                                sum = 0m
                                discountedSum = 0m
                            }
                errors = parsedInvoices.Select(fun invoice -> invoice.errorText)
            }
        else
            {
                invoiceSum = {
                    sum = parsedInvoices.Select(fun invoice -> invoice.amount).Sum().GetValueOrDefault()
                    discountedSum = parsedInvoices.Select(fun invoice -> invoice.discountedAmount).Sum().GetValueOrDefault()
                }
                errors = null
            }
        
