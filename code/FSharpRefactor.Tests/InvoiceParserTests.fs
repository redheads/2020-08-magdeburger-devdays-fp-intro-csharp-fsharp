module InvoiceParserTests

open System
open System.Linq
open FSharpRefactor
open Xunit
open FsUnit.Xunit
open InvoiceParser


[<Fact>]
let ``Non-existing file results in corresponding error message`` () =
    let actual = parseInvoices (["invoice_.txt"]) (Nullable<decimal>()) (Nullable<bool>())
    
    actual.errors.First()
    |> should startWith "Could not find file"
    
[<Fact>]
let ``Valid file results in correct sum`` () =
    let actual = parseInvoices (["invoice1.txt"]) (Nullable<decimal>()) (Nullable<bool>())
    
    let expected = {
        SumOrErrors.errors = []
        invoiceSum = {
            sum = 42m
            discountedSum = 0m
        }
    }
    
    actual |> should equal expected
    
[<Fact>]
let ``Multiple files are summed correctly`` () =
    let actual = parseInvoices (["invoice1.txt"; "invoice1.txt"]) (Nullable<decimal>()) (Nullable<bool>())
    
    let expected = {
        SumOrErrors.errors = []
        invoiceSum = {
            sum = 42m * 2m
            discountedSum = 0m
        }
    }
    
    actual |> should equal expected
    
[<Fact>]
let ``Empty file results in an error`` () =
    let actual = parseInvoices (["invoice2.txt"]) (Nullable<decimal>()) (Nullable<bool>())
    
    let expected = {
        SumOrErrors.errors =  ["The file must have exactly one line"]
        invoiceSum = {
            sum = 0m
            discountedSum = 0m
        }
    }
    
    actual |> should equal expected
    
[<Fact>]
let ``Multiple lines in file results in an error`` () =
    let actual = parseInvoices (["invoice3.txt"]) (Nullable<decimal>()) (Nullable<bool>())
    
    let expected = {
        SumOrErrors.errors =  ["The file must have exactly one line"]
        invoiceSum = {
            sum = 0m
            discountedSum = 0m
        }
    }
    
    actual |> should equal expected
    
[<Fact>]
let ``Invalid content in file results in an error`` () =
    let actual = parseInvoices (["invoice4.txt"]) (Nullable<decimal>()) (Nullable<bool>())
    
    let expected = {
        SumOrErrors.errors =  ["The file content could not be parsed"]
        invoiceSum = {
            sum = 0m
            discountedSum = 0m
        }
    }
    
    actual |> should equal expected

[<Fact>]
let ``Discount is correctly applied`` () =
    let actual = parseInvoices (["invoice5.txt"]) (Nullable<decimal>(10m)) (Nullable<bool>(true))
    
    let expected = {
        SumOrErrors.errors =  []
        invoiceSum = {
            sum = 10m
            discountedSum = 9m
        }
    }
    
    actual |> should equal expected

[<Fact>]
let ``Forbidden discount is not applied`` () =
    let actual = parseInvoices (["invoice5.txt"]) (Nullable<decimal>(10m)) (Nullable<bool>(false))
    
    let expected = {
        SumOrErrors.errors =  []
        invoiceSum = {
            sum = 10m
            discountedSum = 0m
        }
    }
    
    actual |> should equal expected