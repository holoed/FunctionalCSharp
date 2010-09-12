using FunctionalCSharp.DiscriminatedUnions;
using NUnit.Framework;
using Exp = FunctionalCSharpTests.DiscriminatedUnions.LambdaCalculus.Exp;

namespace FunctionalCSharpTests.DiscriminatedUnions
{
    [TestFixture]
    public class LambdaCalculusTests
    {
        private Exp _b,_fix,_ifThenElse,_btrue,_bfalse,_isZero,_zero,_one,_mult,_two,_three,_four,_pred,_six;
        private Exp _factorial;

        [SetUp]
        public void SetUp()
        {
            _b = DataType.New<Exp>();
            _fix = _b.Lam("f", _b.App(
             _b.Lam("x", _b.App(_b.Var("f"), _b.Lam("y", _b.App(_b.App(_b.Var("x"), _b.Var("x")), _b.Var("y"))))),
             _b.Lam("x", _b.App(_b.Var("f"), _b.Lam("y", _b.App(_b.App(_b.Var("x"), _b.Var("x")), _b.Var("y")))))));
            _ifThenElse = _b.Lam("p", _b.Lam("a", _b.Lam("b", _b.App(_b.App(_b.Var("p"), _b.Var("a")), _b.Var("b")))));
            _btrue = _b.Lam("x", _b.Lam("y", _b.Var("x")));
            _bfalse = _b.Lam("x", _b.Lam("y", _b.Var("y")));
            _isZero = _b.Lam("n", _b.App(_b.App(_b.Var("n"), _b.Lam("x", _bfalse)), _btrue));
            _zero = _b.Lam("f", _b.Lam("x", _b.Var("x")));
            _one = _b.Lam("f", _b.Lam("x", _b.App(_b.Var("f"), _b.Var("x"))));
            _mult = _b.Lam("m", _b.Lam("n", _b.Lam("f", _b.App(_b.Var("n"), _b.App(_b.Var("m"), _b.Var("f"))))));
            _two = _b.Lam("f", _b.Lam("x", _b.App(_b.Var("f"), _b.App(_b.Var("f"), _b.Var("x")))));
            _three = _b.Lam("f", _b.Lam("x", _b.App(_b.Var("f"), _b.App(_b.Var("f"), _b.App(_b.Var("f"), _b.Var("x"))))));
            _four = _b.Lam("f", _b.Lam("x", _b.App(_b.Var("f"), _b.App(_b.Var("f"), _b.App(_b.Var("f"), _b.App(_b.Var("f"), _b.Var("x")))))));
            _pred = _b.Lam("n", _b.Lam("f", _b.Lam("x", _b.App(_b.App(_b.App(_b.Var("n"), _b.Lam("g", _b.Lam("h", _b.App(_b.Var("h"), _b.App(_b.Var("g"), _b.Var("f")))))), _b.Lam("u", _b.Var("x"))), _b.Lam("u", _b.Var("u"))))));
            _six = LambdaCalculus.Interpret(_b.App(_b.App(_mult, _three), _two));

             _factorial = _b.App(_fix,
                                 _b.Lam("k",
                                       _b.Lam("i",
                                             _b.App(_b.App(_b.App(_ifThenElse, _b.App(_isZero, _b.Var("i"))), _one),
                                                   _b.App(_b.App(_mult, _b.Var("i")),
                                                         _b.App(_b.Var("k"), _b.App(_pred, _b.Var("i"))))))));            
        }
        
        [Test]
        public void Identity()
        {         
            var exp = _b.App(_b.Lam("x", _b.Var("x")), _b.Lam("y", _b.Var("y")));
            Assert.AreEqual("((λx.x) (λy.y))",LambdaCalculus.ExpToString(exp));
            var exp2 = LambdaCalculus.Interpret(exp);
            Assert.AreEqual("(λy.y)", LambdaCalculus.ExpToString(exp2));
        }

        [Test]
        public void Fix()
        {
            Assert.AreEqual("(λf.((λx.(f (λy.((x x) y)))) (λx.(f (λy.((x x) y))))))", LambdaCalculus.ExpToString(_fix));
        }

        [Test]
        public void IfThenElse()
        {            
            Assert.AreEqual("(λp.(λa.(λb.((p a) b))))", LambdaCalculus.ExpToString(_ifThenElse));
        }

        [Test]
        public void True()
        {            
            Assert.AreEqual("(λx.(λy.x))", LambdaCalculus.ExpToString(_btrue));
        }

        [Test]
        public void False()
        {            
            Assert.AreEqual("(λx.(λy.y))", LambdaCalculus.ExpToString(_bfalse));
        }

        public void IsZero()
        {            
            Assert.AreEqual("(λn.((n (λx.(λx.(λy.y)))) (λx.(λy.x))))", LambdaCalculus.ExpToString(_isZero));

            var retTrue = LambdaCalculus.Interpret(_b.App(_isZero, _zero));
            Assert.AreEqual("(λx.(λy.x))", LambdaCalculus.ExpToString(retTrue));

            var retFalse = LambdaCalculus.Interpret(_b.App(_isZero, _one));
            Assert.AreEqual("(λx.(λy.y))", LambdaCalculus.ExpToString(retFalse));
        }

        public void Zero()
        {            
            Assert.AreEqual("(λf.(λx.x))", LambdaCalculus.ExpToString(_zero));
        }

        public void One()
        {
            Assert.AreEqual("(λf.(λx.(f x)))", LambdaCalculus.ExpToString(_one));
        }

        public void Two()
        {            
            Assert.AreEqual("(λf.(λx.(f (f x))))", LambdaCalculus.ExpToString(_two));
        }

        public void Three()
        {            
            Assert.AreEqual("(λf.(λx.(f (f (f x)))))", LambdaCalculus.ExpToString(_three));
        }

        public void Four()
        {            
            Assert.AreEqual("(λf.(λx.(f (f (f (f x))))))", LambdaCalculus.ExpToString(_four));
        }

        public void Mult()
        {            
            Assert.AreEqual("(λm.(λn.(λf.(n (m f)))))", LambdaCalculus.ExpToString(_mult));
        }

        public void Six()
        {            
            Assert.AreEqual("(λf.(λx.(f (f (f (f (f (f x))))))))", LambdaCalculus.ExpToString(_six));
        }

        public void Pred()
        {            
            Assert.AreEqual("(λn.(λf.(λx.(((n (λg.(λh.(h (g f))))) (λu.x)) (λu.u)))))", LambdaCalculus.ExpToString(_pred));

            var retThree = LambdaCalculus.Interpret(_b.App(_pred, _four));
            Assert.AreEqual("(λf.(λx.(f (f (f x)))))", LambdaCalculus.ExpToString(retThree));
        }
        
        [Test]
        public void Factorial()
        {                   
            Assert.AreEqual("((λf.((λx.(f (λy.((x x) y)))) (λx.(f (λy.((x x) y)))))) (λk.(λi.((((λp.(λa.(λb.((p a) b)))) ((λn.((n (λx.(λx.(λy.y)))) (λx.(λy.x)))) i)) (λf.(λx.(f x)))) (((λm.(λn.(λf.(n (m f))))) i) (k ((λn.(λf.(λx.(((n (λg.(λh.(h (g f))))) (λu.x)) (λu.u))))) i)))))))", LambdaCalculus.ExpToString(_factorial));            
        }

        [Test]
        public void FactorialOfZero()
        {
            Assert.AreEqual(LambdaCalculus.ExpToString(_one), LambdaCalculus.ExpToString(LambdaCalculus.Interpret(_b.App(_factorial, _zero))));            
        }

        [Test]
        public void FactorialOfOne()
        {
            Assert.AreEqual(LambdaCalculus.ExpToString(_one), LambdaCalculus.ExpToString(LambdaCalculus.Interpret(_b.App(_factorial, _one))));            
        }

        [Test]
        public void FactorialOfTwo()
        {
            Assert.AreEqual(LambdaCalculus.ExpToString(_two), LambdaCalculus.ExpToString(LambdaCalculus.Interpret(_b.App(_factorial, _two))));            
        }

        [Test]
        public void FactorialOfThree()
        {
            Assert.AreEqual(LambdaCalculus.ExpToString(_six), LambdaCalculus.ExpToString(LambdaCalculus.Interpret(_b.App(_factorial, _three))));
        }

        [Test]
        public void FactorialOfFour()
        {
            Assert.AreEqual(LambdaCalculus.ExpToString(LambdaCalculus.Interpret(_b.App(_b.App(_mult, _b.App(_b.App(_mult, _four), _three)), _two))), 
                LambdaCalculus.ExpToString(LambdaCalculus.Interpret(_b.App(_factorial, _four))));
        }
    }
}
