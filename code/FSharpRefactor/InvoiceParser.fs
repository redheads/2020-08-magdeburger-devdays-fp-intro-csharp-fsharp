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
        amount: Option<decimal>
        discountedAmount: Option<decimal>
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
        
    let applyDiscount amount (discountPercentage: decimal) (isDiscountAllowed: bool) =
        if isDiscountAllowed then
                Some(amount - (amount * (discountPercentage / 100m)))
        else
            None
        
    let parseInvoice (discountPercentage: Option<decimal>) (isDiscountAllowed: Option<bool>) rawInvoice =
        let createErrorResult errorText =
            { ParseResult.errorText = errorText
              amount = None
              discountedAmount = None }
        
        if isValid rawInvoice then
            let lines = rawInvoice.contents
            if hasCorrectLineCount lines then
                let success, parsedAmount =
                    lines
                    |> List.head
                    |> Decimal.TryParse
                
                if success then
                    let discounted =
                        match discountPercentage, isDiscountAllowed with
                            | Some dp, Some ida -> applyDiscount parsedAmount dp ida
                            | _ -> None
                    
                    {
                        ParseResult.errorText = null 
                        amount = Some(parsedAmount)
                        discountedAmount = discounted
                    }
                else
                    createErrorResult "The file content could not be parsed"
            else
                createErrorResult "The file must have exactly one line"
        else
            createErrorResult rawInvoice.errorText
    
    let calculateSum parsedInvoices =
          let zero _ = 0m 
          {
                invoiceSum = {
                    sum = List.sumBy (fun invoice -> invoice.amount |> Option.defaultWith zero) parsedInvoices
                    discountedSum = List.sumBy (fun invoice -> invoice.discountedAmount |> Option.defaultWith zero) parsedInvoices
                }
                errors = []
            }
          
    let parseInvoices (paths : IEnumerable<string>) (discountPercentage : Option<decimal>) (isDiscountAllowed : Option<bool>) : SumOrErrors =
        let parsedInvoices =
            paths
            |> parseFiles
            |> List.map (parseInvoice discountPercentage isDiscountAllowed) 
                
        if List.exists (fun invoice -> not (String.IsNullOrEmpty(invoice.errorText))) parsedInvoices then
            {
                invoiceSum = {
                                sum = 0m
                                discountedSum = 0m
                            }
                errors = List.map (fun x -> x.errorText) parsedInvoices
            }
        else
            calculateSum parsedInvoices
