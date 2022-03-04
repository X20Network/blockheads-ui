module Imports

open Fable.Core
open Fable.Core.JsInterop

[<Import("default", from = "web3")>]
type Web3(provider :obj) =

    member _.x :int = jsNative

[<Import("default", from = "web3modal")>]
type Web3Modal(config: obj) =

    member _.x :int = jsNative

    member _.connect() :JS.Promise<obj> = jsNative
