module GrusskartenTests

open System
open Xunit
open Swensen.Unquote
open Grusskarten
open FSharpPlus
open FSharpPlus.Data



[<Fact>]
let ``with double parameters creates valid kunde`` () =
    let t =
        withDoubleParameters 42 "az" None DateTime.Now

    Validation.either (fun (k: Kunde) -> test <@ k.Id = 42 @>) (fun _ -> test <@ true = false @>) t

[<Fact>]
let ``not starting with a fails`` () =
    let t =
        withDoubleParameters 42 "bz" None DateTime.Now

    Validation.either (fun _ -> test <@ true = false @>) (fun err -> test <@ err = [ "Muss mit a starten" ] @>) t

[<Fact>]
let ``not ending with z fails`` () =
    let t =
        withDoubleParameters 42 "ab" None DateTime.Now

    Validation.either (fun _ -> test <@ true = false @>) (fun err -> test <@ err = [ "Muss mit z enden" ] @>) t

[<Fact>]
let ``geburtsdatum in the future fails`` () =
    let t =
        withDoubleParameters 42 "az" None (DateTime.Now.AddDays(1.))

    Validation.either (fun _ -> test <@ true = false @>) (fun err ->
        test <@ err = [ "Darf nicht in der Zukunft liegen" ] @>) t

[<Fact>]
let ``collects multiple errors`` () =
    let t =
        withDoubleParameters 42 "bc" None (DateTime.Now.AddDays(1.))

    Validation.either (fun _ -> test <@ true = false @>) (fun err ->
        test
            <@ err =
                [ "Muss mit a starten"
                  "Muss mit z enden"
                  "Darf nicht in der Zukunft liegen" ] @>) t



[<Fact>]
let ``concatenateErrorStrings creates valid kunde`` () =
    let t =
        concatenateErrorStrings 42 "az" None DateTime.Now

    Validation.either (fun (k: Kunde) -> test <@ k.Id = 42 @>) (fun _ -> test <@ true = false @>) t

[<Fact>]
let ``concatenateErrorStrings not starting with a fails`` () =
    let t =
        concatenateErrorStrings 42 "bz" None DateTime.Now

    Validation.either (fun _ -> test <@ true = false @>) (fun err -> test <@ err = [ "Muss mit a starten" ] @>) t

[<Fact>]
let ``concatenateErrorStrings not ending with z fails`` () =
    let t =
        concatenateErrorStrings 42 "ab" None DateTime.Now

    Validation.either (fun _ -> test <@ true = false @>) (fun err -> test <@ err = [ "Muss mit z enden" ] @>) t

[<Fact>]
let ``concatenateErrorStrings geburtsdatum in the future fails`` () =
    let t =
        concatenateErrorStrings 42 "az" None (DateTime.Now.AddDays(1.))

    Validation.either (fun _ -> test <@ true = false @>) (fun err ->
        test <@ err = [ "Darf nicht in der Zukunft liegen" ] @>) t

[<Fact>]
let ``concatenateErrorStrings collects multiple errors`` () =
    let t =
        concatenateErrorStrings 42 "bc" None (DateTime.Now.AddDays(1.))

    Validation.either (fun _ -> test <@ true = false @>) (fun err ->
        test
            <@ err =
                [ "Muss mit a starten"
                  "Muss mit z enden"
                  "Darf nicht in der Zukunft liegen" ] @>) t




[<Fact>]
let ``niceErrorTypes creates valid kunde`` () =
    let t = niceErrorTypes 42 "az" None DateTime.Now

    Validation.either (fun (k: Kunde) -> test <@ k.Id = 42 @>) (fun _ -> test <@ true = false @>) t

[<Fact>]
let ``niceErrorTypes not starting with a fails`` () =
    let t = niceErrorTypes 42 "bz" None DateTime.Now

    Validation.either (fun _ -> test <@ true = false @>) (fun err -> test <@ err = [ NotStartWithA ] @>) t

[<Fact>]
let ``niceErrorTypes not ending with z fails`` () =
    let t = niceErrorTypes 42 "ab" None DateTime.Now

    Validation.either (fun _ -> test <@ true = false @>) (fun err -> test <@ err = [ NotEndWithZ ] @>) t

[<Fact>]
let ``niceErrorTypes geburtsdatum in the future fails`` () =
    let t =
        niceErrorTypes 42 "az" None (DateTime.Now.AddDays(1.))

    Validation.either (fun _ -> test <@ true = false @>) (fun err -> test <@ err = [ IsInFuture ] @>) t

[<Fact>]
let ``niceErrorTypes collects multiple errors`` () =
    let t =
        niceErrorTypes 42 "bc" None (DateTime.Now.AddDays(1.))

    Validation.either (fun _ -> test <@ true = false @>) (fun err ->
        test
            <@ err =
                [ NotStartWithA
                  NotEndWithZ
                  IsInFuture ] @>) t
