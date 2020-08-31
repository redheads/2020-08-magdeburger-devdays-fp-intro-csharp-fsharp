module FSharpRefactor.Maybe


type Maybe<'T> = Just of 'T | Nothing

let map f x =
    match x with
    | None -> None
    | Some x' -> f x' |> Some
    
let bind f x =
    match x with
    | None -> None
    | Some x' -> f x'
