module Program

open BenchmarkDotNet.Attributes
open BenchmarkDotNet.Running
open Fabulous.XamarinForms
open Xamarin.Forms
       
[<MemoryDiagnoser>]
type ViewElementBenches() =
    [<Benchmark>]
    member self.EmptyDeclaration() =
        [ for i in 1 .. 10000 ->
            View.Label()
        ]
        
    [<Benchmark>]
    member self.SimpleDeclaration() =
        [ for i in 1 .. 10000 ->
            View.Label(
                text = "Hello world",
                fontSize = FontSize 22.,
                textColor = Color.Black
            )
        ]
        
    [<Benchmark>]
    member self.SimpleDeclarationWith() =
        [ for i in 1 .. 10000 ->
            View.Label(
                text = "Hello world",
                fontSize = FontSize 22.,
                textColor = Color.Black
            ).Row(1).Column(1)
        ]
       
let defaultSwitch () = BenchmarkSwitcher [|typeof<ViewElementBenches> |]

[<EntryPoint>]
let Main args =
    let summary = defaultSwitch().Run args
    0
