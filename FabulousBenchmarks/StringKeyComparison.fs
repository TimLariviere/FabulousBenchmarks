namespace FabulousBenchmarks

open System.Collections.Concurrent

open BenchmarkDotNet.Attributes
open System
open System.IO

module Helpers =
    let getStrings len = Array.init len (fun _ -> Path.GetRandomFileName())

    let lookup arr (dict:ConcurrentDictionary<string,bool>) =
        arr |> Array.iteri(fun idx elm -> 
            let mutable b = dict.[elm]
            b <- dict.[arr.[0]])

type StringKeyComparison () =
    let mutable arr : string [] = [||]
    let dict1 = ConcurrentDictionary<_,_>()
    let dict2 = ConcurrentDictionary<_,_>(StringComparer.Ordinal)

    [<Params (100, 500, 1000, 2000)>] 
    member val public DictSize = 0 with get, set

    [<GlobalSetup>]
    member self.GlobalSetupData() =
        dict1.Clear(); dict2.Clear()
        arr <- Helpers.getStrings self.DictSize
        arr |> Array.iter (fun x -> dict1.[x] <- true ; dict2.[x] <- true)

    [<Benchmark>]
    member self.StandardLookup () = Helpers.lookup arr dict1

    [<Benchmark>]
    member self.OrdinalLookup () = Helpers.lookup arr dict2
