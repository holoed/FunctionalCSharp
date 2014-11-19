<h2><strong>Project Description</strong></h2>
<p>Simple Functional Programming Library for C#. It introduces several features found in programming languages like F#. It is built in a compositional fashion starting from Pattern Matching and building on that to support Object Expressions, Tuples, Active Patterns, ADTs.<br /><br />Recommended Reading<br /><a href="http://www.amazon.co.uk/gp/product/1933988924?ie=UTF8&amp;tag=httpfsharpcbl-21&amp;linkCode=as2&amp;camp=1634&amp;creative=6738&amp;creativeASIN=1933988924">Functional Programming for the Real World: With Examples in F# and C#</a></p>
<h2><strong>Pattern Matching</strong></h2>
<div style="color: black; background-color: white;">
<pre><span style="color: blue;">var</span> Op = <span style="color: blue;">new</span> Dictionary&lt;ExpressionType, <span style="color: blue;">string</span>&gt; { { ExpressionType.Add, <span style="color: #a31515;">"+"</span> } };

Expression&lt;Func&lt;<span style="color: blue;">int</span>,<span style="color: blue;">int</span>,<span style="color: blue;">int</span>&gt;&gt; add = (x,y) =&gt; x + y;

Func&lt;Expression, <span style="color: blue;">string</span>&gt; toString = <span style="color: blue;">null</span>;
 toString = exp =&gt;
 exp.Match()
    .With&lt;LambdaExpression&gt;(l =&gt; toString(l.Body))
    .With&lt;ParameterExpression&gt;(p =&gt; p.Name)
    .With&lt;BinaryExpression&gt;(b =&gt; String.Format(<span style="color: #a31515;">"{0} {1} {2}"</span>, toString(b.Left), Op[b.NodeType], toString(b.Right)))
    .Return&lt;<span style="color: blue;">string</span>&gt;();
</pre>
</div>
<p><br />F# Equivalent</p>
<pre>let operator x = match x with
                 | ExpressionType.Add -&gt; "+"

let rec toString exp = match exp with
                       | LambdaExpression(args, body) -&gt; toString(body)
                       | ParameterExpression(name) -&gt; name
                       | BinaryExpression(op,l,r) -&gt; sprintf "%s %s %s" (toString l) (operator op) (toString r)
</pre>
<p><br /><br />C# with Active Patterns</p>
<div style="color: black; background-color: white;">
<pre>Func&lt;Expression, <span style="color: blue;">string</span>&gt; toString = <span style="color: blue;">null</span>;
toString = exp =&gt;
           exp.Match()
              .Lambda((args, body) =&gt; toString(body))
              .Param ((name)       =&gt; name)
              .Add   ((l, r)       =&gt; String.Format(<span style="color: #a31515;">"({0} + {1})"</span>, toString(l), toString(r)))
              .Mult  ((l, r)       =&gt; String.Format(<span style="color: #a31515;">"{0} * {1}"</span>, toString(l), toString(r)))
              .Return&lt;<span style="color: blue;">string</span>&gt;();
</pre>
</div>
<p><br />C# Arithmetic Pattern n+ k</p>
<div style="color: black; background-color: white;">
<pre>Func&lt;<span style="color: blue;">int</span>, <span style="color: blue;">bool</span>&gt; even = <span style="color: blue;">null</span>;
Func&lt;<span style="color: blue;">int</span>, <span style="color: blue;">bool</span>&gt; odd = <span style="color: blue;">null</span>;
even = exp =&gt; exp.Match()
                 .With(n =&gt; 0,     _ =&gt; <span style="color: blue;">true</span>)
                 .With(n =&gt; n + 1, n =&gt; odd(n))
                 .Return&lt;<span style="color: blue;">bool</span>&gt;();
odd = exp =&gt; !even(exp);
</pre>
</div>
<p><br />Haskell equivalent</p>
<pre> even 0 = True
 even (n + 1) = odd n
 odd n = not (even n)
</pre>
<p><br /><br />C# Lists Pattern Matching</p>
<div style="color: black; background-color: white;">
<pre>Func&lt;Func&lt;<span style="color: blue;">char</span>, <span style="color: blue;">char</span>&gt;, Func&lt;IEnumerable&lt;<span style="color: blue;">char</span>&gt;, IEnumerable&lt;<span style="color: blue;">char</span>&gt;&gt;&gt; map = <span style="color: blue;">null</span>;
map = f =&gt; s =&gt; s.Match()
                 .List((x, xs) =&gt; f(x).Cons(map(f)(xs)))
                 .Any(() =&gt; s)
                 .Return&lt;IEnumerable&lt;<span style="color: blue;">char</span>&gt;&gt;();
<span style="color: blue;">var</span> toUpper = map(Char.ToUpper);
</pre>
</div>
<p><br />F# Equivalent</p>
<pre>let rec map f xs = match xs with
                   | x::xs -&gt; f(x)::(map f xs)
                   | _ -&gt; xs;;
let toUpper = map System.Char.ToUpper
</pre>
<p>&nbsp;</p>
<h2><strong>Object Expressions</strong></h2>
<p>C#</p>
<div style="color: black; background-color: white;">
<pre><span style="color: blue;">var</span> foo = ObjectExpression.New&lt;IFoo&gt;()
                          .With(o =&gt; o.A, () =&gt; <span style="color: #a31515;">"Hello"</span>)
                          .Return();
</pre>
</div>
<p>F# Equivalent</p>
<pre>let foo = { new IFoo with
                member x.A () = "Hello" }
</pre>
<p><br />Sample IoC Container</p>
<div style="color: black; background-color: white;">
<pre>            <span style="color: blue;">var</span> container = ObjectExpression
                 .New&lt;IoC_Container&gt;()
                 .With(o =&gt; o.Get&lt;IFoo&gt;, () =&gt; ObjectExpression.New&lt;IFoo&gt;().Return())
                 .With(o =&gt; o.Get&lt;IBar&gt;, () =&gt; ObjectExpression.New&lt;IBar&gt;().Return())
                 .Return();

            <span style="color: blue;">var</span> foo = container.Get&lt;IFoo&gt;();
            <span style="color: blue;">var</span> bar = container.Get&lt;IBar&gt;();
</pre>
</div>
<p><br />Sorted List (Create an IComparer on the fly ;))</p>
<div style="color: black; background-color: white;">
<pre><span style="color: blue;">var</span> list = <span style="color: blue;">new</span> SortedList&lt;<span style="color: blue;">int</span>, <span style="color: blue;">string</span>&gt;(
                ObjectExpression
                .New&lt;IComparer&lt;<span style="color: blue;">int</span>&gt;&gt;()
                .With(o =&gt; o.Compare, (<span style="color: blue;">int</span> x, <span style="color: blue;">int</span> y) =&gt; x - y)
                .Return());
</pre>
</div>
<h2><strong>C# Discriminated Unions</strong></h2>
<div style="color: black; background-color: white;">
<pre><span style="color: blue;">public</span> <span style="color: blue;">interface</span> Exp
{
   Exp Var(<span style="color: blue;">string</span> x);
   Exp Lam(<span style="color: blue;">string</span> x, Exp e);
   Exp Let(<span style="color: blue;">string</span> x, Exp e1, Exp e2);
   Exp App(Exp e1, Exp e2);
}

<span style="color: blue;">var</span> exp = DataType.New&lt;Exp&gt;();

exp = exp.Let(<span style="color: #a31515;">"compose"</span>,
                            exp.Lam(<span style="color: #a31515;">"f"</span>,
                                    exp.Lam(<span style="color: #a31515;">"g"</span>,
                                            exp.Lam(<span style="color: #a31515;">"x"</span>,
                                                    exp.App(exp.Var(<span style="color: #a31515;">"g"</span>), exp.App(exp.Var(<span style="color: #a31515;">"f"</span>), exp.Var(<span style="color: #a31515;">"x"</span>)))))),
                            exp.Var(<span style="color: #a31515;">"compose"</span>));

Func&lt;Exp, <span style="color: blue;">string</span>&gt; toString = <span style="color: blue;">null</span>;
toString = expr =&gt; expr.Match()
    .With(o =&gt; o.Var, (<span style="color: blue;">string</span> x) =&gt; x)
    .With(o =&gt; o.Let, (<span style="color: blue;">string</span> x, Exp e1, Exp e2) =&gt; <span style="color: blue;">string</span>.Format(<span style="color: #a31515;">"let {0} = {1} in {2}"</span>, x, toString(e1), toString(e2)))
    .With(o =&gt; o.Lam, (<span style="color: blue;">string</span> x, Exp e) =&gt; <span style="color: blue;">string</span>.Format(<span style="color: #a31515;">"fun {0} -&gt; {1}"</span>, x, toString(e)))
    .With(o =&gt; o.App, (Exp e1, Exp e2) =&gt; <span style="color: blue;">string</span>.Format(<span style="color: #a31515;">"({0} {1})"</span>, toString(e1), toString(e2)))
    .Return&lt;<span style="color: blue;">string</span>&gt;();

Assert.AreEqual(<span style="color: #a31515;">"let compose = fun f -&gt; fun g -&gt; fun x -&gt; (g (f x)) in compose"</span>, toString(exp));

</pre>
</div>
<p><br />F# Equivalent</p>
<pre>type Exp = Var of string
         | Lam of string * Exp
         | Let of string * Exp * Exp
         | App of Exp * Exp

let composeAst = Let("compose",
                    Lam("f",
                        Lam("g",
                            Lam ("x",
                                App(Var "g", App(Var "f", Var "x"))))),
                        Var "compose")

let rec toString exp = 
    match exp with
    | Var x -&gt; x
    | Let (x, e1, e2) -&gt; String.Format("let {0} = {1} in {2}", x, toString(e1), toString(e2))
    | Lam (x, e) -&gt; String.Format("fun {0} -&gt; {1}", x, toString(e))
    | App (e1, e2) -&gt; String.Format("({0} {1})", toString(e1), toString(e2))

let ret = toString composeAst
</pre>
<p>&nbsp;</p>
<h2><strong>Monadic Parser</strong></h2>
<p>C#</p>
<div style="color: black; background-color: white;">
<pre><span style="color: blue;">public</span> <span style="color: blue;">static</span> IParser&lt;IEnumerable&lt;<span style="color: blue;">char</span>&gt;, IEnumerable&lt;<span style="color: blue;">string</span>&gt;&gt; String()
{
   <span style="color: blue;">return</span> CharParser.Word()
                .AsString()
                .SepBy(CharParser.Whitespace()
                                 .Many1()
                                 .AsString());
}

CollectionAssert.AreEqual
                (<span style="color: blue;">new</span>[] {<span style="color: #a31515;">"Welcome"</span>, <span style="color: #a31515;">"to"</span>, <span style="color: #a31515;">"the"</span>, <span style="color: #a31515;">"real"</span>, <span style="color: #a31515;">"world"</span>},
                 StringParser.String().ParseString(<span style="color: #a31515;">"Welcome to the real world"</span>).First());
</pre>
</div>
