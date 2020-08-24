namespace FSharpRefactor

open System
open System.Collections.Generic


module InvoiceParser =
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
        
    type ApplicationError =
        | FileReadError of string
        | LineCountError
        | NotADecimalError
    
    type Lines = string list
    
    let parseFile path : Result<Lines, ApplicationError> =
        try
            System.IO.File.ReadLines(path)
            |> Seq.toList
            |> Ok
            
        with
        | ex -> ex.Message |> FileReadError |> Error
    
    let parseFiles paths =
        Seq.map parseFile paths
        |> Seq.toList
 
    let isValid rawInvoice =
        match rawInvoice with
        | Ok _ -> true
        | Error _ -> false
        
    let hasCorrectLineCount (lines: Lines) : Result<Lines, ApplicationError> =
        if lines.Length = 1 then
            Ok lines
        else
            Error LineCountError
        
    let applyDiscount amount (discountPercentage: decimal) (isDiscountAllowed: bool) =
        if isDiscountAllowed then
                Some(amount - (amount * (discountPercentage / 100m)))
        else
            None
        
    let valueAsDecimal (input: string) =
        let success, parsedAmount =
            input |> Decimal.TryParse
        if success then
            Ok parsedAmount
        else
            Error NotADecimalError
                    
    let parseInvoice (discountPercentage: Option<decimal>) (isDiscountAllowed: Option<bool>) (rawInvoice : Result<Lines, ApplicationError>) =
        let createErrorResult errorText =
            { ParseResult.errorText = errorText
              amount = None
              discountedAmount = None }
        
        match rawInvoice with
        | Ok lines ->
            match hasCorrectLineCount lines with
            | Ok lines ->
                let parsed =
                    lines
                    |> List.head
                    |> valueAsDecimal 
                
                match parsed with
                | Ok parsedAmount ->
                    let discounted =
                        match discountPercentage, isDiscountAllowed with
                            | Some dp, Some ida -> applyDiscount parsedAmount dp ida
                            | _ -> None
                    
                    {
                        ParseResult.errorText = null 
                        amount = Some(parsedAmount)
                        discountedAmount = discounted
                    }
                | Error (NotADecimalError) ->
                    createErrorResult "The file content could not be parsed"
            | Error LineCountError ->
                createErrorResult "The file must have exactly one line"
        | Error (FileReadError err) ->
            createErrorResult err
           
    
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
