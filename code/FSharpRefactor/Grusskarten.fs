module Grusskarten

open System
open FSharpPlus
open FSharpPlus.Data

type Kunde =
    { Id: int
      Vorname: string
      Spitzname: string option
      Geburtsdatum: DateTime }

let createKunde id vorname (spitzname: string) geburtsdatum: Kunde option =
    if String.IsNullOrWhiteSpace vorname then
        None
    else
        let spitznameOpt =
            if String.IsNullOrWhiteSpace spitzname then None else Some spitzname

        Some
            { Id = id
              Vorname = vorname
              Spitzname = spitznameOpt
              Geburtsdatum = geburtsdatum }

let startsWithA s =
    if String.startsWith "a" s then Success s else Failure "Muss mit a starten"

let endsWithZ s =
    if String.endsWith "z" s then Success s else Failure "Muss mit z enden"

let notEmpty s =
    if String.IsNullOrWhiteSpace s then Failure "Darf nicht leer sein" else Success s

let notTooLong s =
    if String.length s > 30 then Failure "Darf nicht zu lang sein" else Success s

let notInTheFuture dt =
    if dt > DateTime.Now
    then Failure "Darf nicht in der Zukunft liegen"
    else Success dt

// int -> string -> string option -> DateTime -> Kunde
let createKundeValidated id vorname spitzname geburtsdatum: Kunde =
    { Id = id
      Vorname = vorname
      Spitzname = spitzname
      Geburtsdatum = geburtsdatum }

// int -> string -> string option -> DateTime -> Validation<string,Kunde>
let validate id vorname spitzname geburtsdatum =


    let validateVorname v =
        v |> notEmpty |> Validation.bind notTooLong

    (createKundeValidated id)
    <!> (validateVorname vorname)
    <*> (Success spitzname)
    <*> (notInTheFuture geburtsdatum)





let greeting (greetingFn: string -> string -> string) (kunde: Kunde option) =
    match kunde with
    | None -> ""
    | Some k ->
        match k.Spitzname with
        | None -> greetingFn k.Vorname ""
        | Some s -> greetingFn k.Vorname s


let x =
    let sayHello = greeting (fun v s -> "hello!")
    (createKunde 3 "a" "b" DateTime.Now) |> sayHello
    (createKunde 5 "a32" "b43" DateTime.Now)
    |> sayHello

let x2 =
    [ (createKunde 3 "a" "b" (new DateTime(1980, 1, 1)))
      (createKunde 3 "a" "b" (new DateTime(1980, 1, 1))) ]
    |> List.filter (fun k ->
        match k with
        | None -> false
        | Some k' -> k'.Geburtsdatum = new DateTime(1980, 1, 1))
    |> List.map (fun k -> 1)
    |> List.sum


let x3: string option list =
    [ (createKunde 3 "a" "b" (new DateTime(1980, 1, 1)))
      (createKunde 3 "a" "b" (new DateTime(1980, 1, 1))) ]
    |> List.filter (Option.isSome)
    |> List.map (fun k ->
        Option.bind (fun (k': Kunde) -> Option.map (fun (k'': string) -> k''.ToUpper()) k'.Spitzname) k)

type Fehler = string

let createGrusskartentext (kunde: Kunde): Result<string, Fehler> = Ok kunde.Vorname

type DruckVersion = string

let print (text: string): Result<DruckVersion, Fehler> = Ok "Druck mich"

type Quittung = string
type EmailAdresse = string

let post (version: DruckVersion): Result<Quittung, Fehler> = Ok "Quittung"

let sendEmail (emailAdresse: EmailAdresse) (ergebnis: Result<Quittung, Fehler>): string =
    match ergebnis with
    | Ok x -> "Versendet"
    | Error e -> e

let workflow =
    let kunde =
        { Id = 1
          Vorname = "Abc"
          Spitzname = Some "A"
          Geburtsdatum = new DateTime(1990, 1, 1) }

    let result: string =
        createGrusskartentext kunde
        |> Result.bind print
        |> Result.bind post
        |> sendEmail "t@t.de"

    result

(*
                Option.map (fun (k' : Kunde) ->
                        Option.map (fun (k'' : string) -> k''.ToUpper()) k'.Spitzname
                    )
*)

//        match k with
//        | None -> None
//        | Some k' ->
//            match k'.Spitzname with
//            | None -> None
//            | Some k'' -> Some(k''.ToUpper())
//                Option.map (fun (k'' : string) -> k''.ToUpper()) k'.Spitzname


type Vorname = string
// int -> string -> string option -> DateTime -> Validation<string, Kunde>
let withDoubleParameters id vorname spitzname geburtsdatum =
    let createKundeWrapper i (v: Vorname) (_v: Vorname) s g = createKundeValidated i v s g

    createKundeWrapper id
    <!> startsWithA vorname
    <*> endsWithZ vorname
    <*> (Success spitzname)
    <*> notInTheFuture geburtsdatum

let concatenateErrorStrings id vorname spitzname geburtsdatum =
    let vornameValidations = [ startsWithA; endsWithZ ]

    let geburtsdatumValidations = [ notInTheFuture ]

    let validate (validators: ('a -> Validation<string, 'a>) list) (input: 'a): Validation<string, 'a> =
        let errors =
            validators
            |> map (fun fn -> fn input)
            |> List.filter (function
                | Success _ -> false
                | Failure _ -> true)

        match errors with
        | [] -> Success input
        | failedValidations -> List.reduce (Validation.appValidation (++)) failedValidations

    createKundeValidated id
    <!> validate vornameValidations vorname
    <*> Success spitzname
    <*> validate geburtsdatumValidations geburtsdatum
