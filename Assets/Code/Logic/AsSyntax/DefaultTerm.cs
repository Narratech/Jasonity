using Assets.Code.Logic.AsSyntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Logic
{
    public abstract class DefaultTerm: Term
    {
        public DefaultTerm() { }

        //An atom, structure, predicate,...
        public virtual bool IsLiteral()
        {
            return false;
        }

        //A structure with annotations: functor(arguments)[functor(arguments)]
        public virtual bool IsPredicate()
        {
            return false;
        }

        //A functor with arguments: "functor(arguments)"
        public virtual bool IsStructure()
        {
            return false;
        }

        public bool IsList()
        {
            return false;
        }

        //No variable term
        public virtual bool IsAtom()
        {
            return false;
        }

        //Body of the plan: " a <- (plan body)"
        public virtual bool IsPlanBody()
        {
            return false;
        }

        //Internal action
        public virtual bool IsInternalAction()
        {
            return false;
        }

        //A literal with a body: "a : b <- c"
        //                            (body)
        public virtual bool IsRule()
        {
            return false;
        }

        public bool IsTerm()
        {
            return true;
        }

        public bool IsVar()
        {
            return false;
        }

        public bool IsUnnamedVar()
        {
            return false;
        }

        public bool IsString()
        {
            return false;
        }

        public bool IsArithExpr()
        {
            return false;
        }

        public bool IsNumeric()
        {
            return false;
        }

        public bool IsGround()
        {
            return false;
        }

        public bool IsCyclicTerm()
        {
            return false;
        }

        public object Clone()
        {
            throw new NotImplementedException();
        }

        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }
    }
}
