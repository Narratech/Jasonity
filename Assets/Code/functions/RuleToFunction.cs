using Assets.Code.AsSyntax;
using Assets.Code.ReasoningCycle;
using System;
using System.Collections.Generic;

namespace Assets.Code.functions
{
    public partial class RuleToFunction : DefaultArithFunction
    {
        private string literal;
        private int arity;

        public RuleToFunction(string literal, int arity)
        {
            this.literal = literal;
            this.arity = arity;
        }

        public string GetName()
        {
            //return super.getName()+"_{"+literal+"}";
            return base.GetName()+"_{"+literal+"}";
        }

        public double Evaluate(Reasoner reasoner, ITerm[] args)
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
                    throw new JasonException("The result of " + r + " (=" + value + ") is not numeric!");
            }
            else
                throw new JasonException("No solution was found for rule " + r);
        }

        public bool CheckArity(int a)
        {
            return a == arity;
        }

        public bool allowUngroundTerms()
        {
            return true;
        }
    }
}