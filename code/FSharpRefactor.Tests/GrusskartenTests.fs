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

    Validation.either (fun _ -> test <@ true = false @>) (fun (err: string) -> test <@ err = "Muss mit a starten" @>) t

[<Fact>]
let ``not ending with z fails`` () =
    let t =
        withDoubleParameters 42 "ab" None DateTime.Now

    Validation.either (fun _ -> test <@ true = false @>) (fun (err: string) -> test <@ err = "Muss mit z enden" @>) t

[<Fact>]
let ``geburtsdatum in the future fails`` () =
    let t =
        withDoubleParameters 42 "az" None (DateTime.Now.AddDays(1.))

    Validation.either (fun _ -> test <@ true = false @>) (fun (err: string) ->
        test <@ err = "Darf nicht in der Zukunft liegen" @>) t

[<Fact>]
let ``collects multiple errors`` () =
    let t =
        withDoubleParameters 42 "bc" None (DateTime.Now.AddDays(1.))

    Validation.either (fun _ -> test <@ true = false @>) (fun (err: string) ->
        test <@ err = "Muss mit a startenMuss mit z endenDarf nicht in der Zukunft liegen" @>) t



[<Fact>]
let ``concatenateErrorStrings creates valid kunde`` () =
    let t =
        concatenateErrorStrings 42 "az" None DateTime.Now

    Validation.either (fun (k: Kunde) -> test <@ k.Id = 42 @>) (fun _ -> test <@ true = false @>) t

[<Fact>]
let ``concatenateErrorStrings not starting with a fails`` () =
    let t =
        concatenateErrorStrings 42 "bz" None DateTime.Now

    Validation.either (fun _ -> test <@ true = false @>) (fun (err: string) -> test <@ err = "Muss mit a starten" @>) t

[<Fact>]
let ``concatenateErrorStrings not ending with z fails`` () =
    let t =
        concatenateErrorStrings 42 "ab" None DateTime.Now

    Validation.either (fun _ -> test <@ true = false @>) (fun (err: string) -> test <@ err = "Muss mit z enden" @>) t

[<Fact>]
let ``concatenateErrorStrings geburtsdatum in the future fails`` () =
    let t =
        concatenateErrorStrings 42 "az" None (DateTime.Now.AddDays(1.))

    Validation.either (fun _ -> test <@ true = false @>) (fun (err: string) ->
        test <@ err = "Darf nicht in der Zukunft liegen" @>) t

[<Fact>]
let ``concatenateErrorStrings collects multiple errors`` () =
    let t =
        concatenateErrorStrings 42 "bc" None (DateTime.Now.AddDays(1.))

    Validation.either (fun _ -> test <@ true = false @>) (fun (err: string) ->
        test <@ err = "Muss mit a startenMuss mit z endenDarf nicht in der Zukunft liegen" @>) t
