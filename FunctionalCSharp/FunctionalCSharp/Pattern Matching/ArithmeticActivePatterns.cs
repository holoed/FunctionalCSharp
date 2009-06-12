using System;
using System.Linq;
using System.Linq.Expressions;

namespace FunctionalCSharp
{
    public static class ArithmeticActivePatterns
    {
        public static PatternMatching<T> With<T>(this PatternMatching<T> o, Expression<Func<int, int>> p, Func<int, object> f)
        {
            Func<int, int> g = y => y;

            return o.With<int>(x =>
                              {
                                  if (IsConst(p))
                                      return Equals(p, x);
                                  else
                                  {
                                      g = Compile(p);
                                      return g(x) > -1;
                                  }
                              }, x => f(g(x)));
        }

        private static Func<int, int> Compile(Expression<Func<int, int>> p)
        {
            return ((Expression<Func<int, int>>)InvertOperation(p)).Compile();
        }

        private static Expression InvertOperation(Expression exp)
        {
            return exp.Match()
                .Lambda((args, body) => Expression.Lambda(InvertOperation(body), args.ToArray()))
                .Add(Expression.Subtract)
                .Return<Expression>();
        }

        private static bool IsConst(Expression exp)
        {
            return exp.Match()
                .Lambda((_, body) => IsConst(body))
                .Const(c => true)
                .Any(() => false)
                .Return<bool>();
        }

        private static bool Equals<T>(Expression exp, T value) 
        {
            return exp.Match()
                .Lambda((_, body) => Equals(body, value))
                .Const(c => c.Equals(value))
                .Any(() => false)
                .Return<bool>();
        }

    }
}
