#region License

/* ****************************************************************************
 * Copyright (c) Edmondo Pentangelo. 
 *
 * This source code is subject to terms and conditions of the Microsoft Public License. 
 * A copy of the license can be found in the License.html file at the root of this distribution. 
 * By using this source code in any fashion, you are agreeing to be bound by the terms of the 
 * Microsoft Public License.
 *
 * You must not remove this notice, or any other, from this software.
 * ***************************************************************************/

#endregion

using FunctionalCSharp;
using FunctionalCSharp.DiscriminatedUnions;

namespace FunctionalCSharpTests.DiscriminatedUnions
{
    public static class LambdaCalculus
    {
        public interface Exp
        {
            Exp Var(string x);
            Exp Lam(string x, Exp e);
            Exp App(Exp e1, Exp e2);
        }

        public static Exp Subst(string x, Exp v, Exp exp)
        {
            return exp.Match()
                .With(o => o.Var, (string y) => x == y ? v : exp)
                .With(o => o.Lam, (string y, Exp e) =>
                {
                    var xp = x == y ? null : x;
                    return DataType.New<Exp>().Lam(y, Subst(xp, v, e));
                })
                .With(o => o.App, (Exp e1, Exp e2) => DataType.New<Exp>().App(Subst(x, v, e1), Subst(x, v, e2)))
                .Return<Exp>();
        }

        public static Exp Reduce(Exp exp)
        {
            return exp.Match()
                .With(o => o.Var, (string _) => exp)
                .With(o => o.Lam, (string s, Exp e) => DataType.New<Exp>().Lam(s, Reduce(e)))
                .With(o => o.App, (Exp e1, Exp e2) => e1.Match()
                                                      .With(o => o.Lam, (string s, Exp e3) => Subst(s, e2, e3))
                                                      .Default(_ => DataType.New<Exp>().App(Reduce(e1), Reduce(e2)))
                                                      .Return<Exp>())
                .Return<Exp>();
        }

        public static Exp Interpret(Exp exp)
        {
            var e1 = Reduce(exp);
            return Equals(e1, exp) ? exp : Interpret(e1);
        }

        public static string ExpToString(Exp exp)
        {
            return exp.Match()
                .With(o => o.Var, (string x) => x)
                .With(o => o.Lam, (string x, Exp e) => string.Format(@"(λ{0}.{1})", x, ExpToString(e)))
                .With(o => o.App, (Exp e1, Exp e2) => string.Format("({0} {1})", ExpToString(e1), ExpToString(e2)))
                .Return<string>();
        }
    }
}
