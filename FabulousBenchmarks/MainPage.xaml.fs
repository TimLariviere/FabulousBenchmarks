namespace FabulousBenchmarks

open BenchmarkDotNet.Analysers
open BenchmarkDotNet.Exporters
open System.Threading.Tasks
open BenchmarkDotNet.Loggers
open BenchmarkDotNet.Running
open Xamarin.Forms
open Xamarin.Forms.Xaml
open System.Linq

type MainPage() as this =
    inherit ContentPage()
    
    let _ = base.LoadFromXaml(typeof<MainPage>)
    let Indicator = base.FindByName<ActivityIndicator>("Indicator")
    let Run = base.FindByName<Button>("Run")
    let Summary = base.FindByName<Label>("Summary")

    let setIsRunning(isRunning) =
        Indicator.IsRunning <- isRunning
        Summary.IsVisible <- not isRunning
        Run.IsVisible <- not isRunning
        
    let setSummary(text) =
        Summary.Text <- text
        let size = Summary.Measure(System.Double.MaxValue, System.Double.MaxValue).Request
        Summary.WidthRequest <- size.Width
        Summary.HeightRequest <- size.Height
        
    let runBenchmark () =
        async {
            do! Async.SwitchToThreadPool()
            
            try
                let logger = AccumulationLogger()
            
                let summary = BenchmarkRunner.Run<StringKeyComparison>();
                MarkdownExporter.Console.ExportToLog(summary, logger);
                ConclusionHelper.Print(logger,
                        summary.BenchmarksCases
                               .SelectMany(fun benchmark -> benchmark.Config.GetCompositeAnalyser().Analyse(summary))
                               .Distinct()
                               .ToList())
                
                Device.BeginInvokeOnMainThread(fun () ->
                    setSummary(logger.GetLog())
                    setIsRunning(false)
                )
            with
            | ex ->
                Device.BeginInvokeOnMainThread(fun () ->
                    setIsRunning(false)
                    this.DisplayAlert("Error", ex.Message, "Ok") |> ignore
                )
        }
        
    do Run.Clicked.AddHandler(System.EventHandler(fun sender args ->
        setIsRunning(true)
        runBenchmark() |> Async.Start
    ))
        