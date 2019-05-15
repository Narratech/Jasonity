using Assets.Code.AsSyntax;
using Assets.Code.Exceptions;
using Assets.Code.ReasoningCycle;
using System;
using System.Collections.Generic;
using Assets.Code.AsSemantics;

namespace Assets.Code.functions
{
    public class RuleToFunction : ArithFunction
    {
        private string literal;
        private int arity;

        public RuleToFunction(string literal, int arity)
        {
            this.literal = literal;
            this.arity = arity;
        }

        public override string GetName()
        {
            //return super.getName()+"_{"+literal+"}";
            return base.GetName()+"_{"+literal+"}";
        }

        public override double Evaluate(Reasoner reasoner, ITerm[] args)
        {
            // create a literal to perform the query
            Literal r;
            if (literal.IndexOf(".") > 0) // is internal action
                r = new InternalActionLiteral(literal);
            else
                r = new LiteralImpl(literal);

            r.AddTerms(args);
            VarTerm answer = new VarTerm("__RuleToFunctionResult");
            r.AddTerm(answer);

            // query the BB
            IEnumerator<Unifier> i = r.LogicalConsequence((reasoner == null ? null : reasoner.GetAgent()), new Unifier());
            if (i.MoveNext())
            {
                ITerm value = i.Current.Get(answer);
                if (value.IsNumeric())
                    return ((INumberTerm)value).Solve();
                else
                    throw new JasonityException("The result of " + r + " (=" + value + ") is not numeric!");
            }
            else
                throw new JasonityException("No solution was found for rule " + r);
        }

        public override bool CheckArity(int a)
        {
            return a == arity;
        }

        public override bool AllowUngroundTerms()
        {
            return true;
        }
    }
}