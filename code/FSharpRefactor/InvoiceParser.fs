namespace FSharpRefactor

open System
open System.Collections.Generic
open System.Linq


module InvoiceParser =
    type RawInvoice = {
        contents: string list
        errorText: string
    }
        
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
        errors: string list
    }
    
    let parseFile path =
        try
            let lines = System.IO.File.ReadLines(path) |> Seq.toList
            {
                contents = lines
                errorText = null
            }
        with
        | ex -> {
            contents = []
            errorText = ex.Message
        }
    
    let parseFiles paths =
        Seq.map parseFile paths
        |> Seq.toList
 
    let isValid (rawInvoice: RawInvoice) =
        String.IsNullOrEmpty rawInvoice.errorText
        
    let hasCorrectLineCount (lines: string list) =
        lines.Length = 1
        
    let parseInvoices (paths : IEnumerable<string>) (discountPercentage : Nullable<decimal>) (isDiscountAllowed : Nullable<bool>) : SumOrErrors =
        let parsedInvoices = List<ParseResult>()
        
        for rawInvoice in (parseFiles paths) do
            let parsedInvoice =
                    if isValid rawInvoice then
                        let lines = rawInvoice.contents
                        if hasCorrectLineCount lines then
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
                    else
                        {
                             ParseResult.errorText = rawInvoice.errorText
                             amount = Nullable<decimal>()
                             discountedAmount = Nullable<decimal>()
                        }   
                        
               
            parsedInvoices.Add(parsedInvoice)
                
        if parsedInvoices.Any(fun invoice -> not (String.IsNullOrEmpty(invoice.errorText))) then
            {
                invoiceSum = {
                                sum = 0m
                                discountedSum = 0m
                            }
                errors = parsedInvoices.Select(fun invoice -> invoice.errorText) |> Seq.toList
            }
        else
            {
                invoiceSum = {
                    sum = parsedInvoices.Select(fun invoice -> invoice.amount).Sum().GetValueOrDefault()
                    discountedSum = parsedInvoices.Select(fun invoice -> invoice.discountedAmount).Sum().GetValueOrDefault()
                }
                errors = []
            }
        
