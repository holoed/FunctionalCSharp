// Learn more about F# at http://fsharp.net

module FsNUnit

open System
open FsCheck
open NUnit.Framework

let private Runner = 
    { new IRunner with
       member x.OnArguments (ntest,args, every) = ()
       member x.OnShrink(args, everyShrink) = ()
       member x.OnFinished(name, result)=
           match result with
           | TestResult.True data -> Assert.IsTrue(true);printfn "%A" data
           | TestResult.False (_,args,_,FsCheck.Property.Outcome.False,_) -> Assert.Fail("{0}-Falsifiable: {1}", [|(name :> obj);(sprintf "%A" args :> obj)|])
           | TestResult.False (_,args,_,FsCheck.Property.Outcome.Exception(exc),_) -> Assert.Fail("{0}-Falsifiable: {1}", [|(name :> obj);(sprintf "%A with exception:%O" args exc :>obj)|])
           | TestResult.Exhausted data -> Assert.Inconclusive("Exhausted after {0} tests", [|(data.NumberOfTests :> obj)|])
           | _ -> raise (new NotSupportedException())
    }


let Config =
    let config = new Configuration()
    config.Runner <- Runner
    config
