namespace FabulousBenchmarks

open BenchmarkDotNet.Attributes

type TailCallDetector () =
    
    let rec factorial n =
            match n with
            | 0 | 1 -> 1
            | _ -> n * factorial(n-1)
            
    let factorial1 n =
        let rec loop i acc =
            match i with
            | 0 | 1 -> acc
            | _ -> loop (i-1) (acc * i)
        loop n 1
        
    let factorial2 n =
        let rec tailCall n f =
            if n <= 1 then
                f()
            else
                tailCall (n - 1) (fun () -> n * f())
 
        tailCall n (fun () -> 1)

    [<Params (7)>] 
    member val public facRank = 0 with get, set
            
    [<Benchmark>]
    member self.test () =
       factorial self.facRank
    
    [<Benchmark>]
    member self.test1 () =
       factorial1 self.facRank
    
    [<Benchmark>]
    member self.test2 () =
       factorial2 self.facRank

