using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pruebas
{
    //The father of all the other classes
    public abstract class Term
    {
        public Term(){}

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

        //A literal with a body: "a :- b"
        //                            (body)
        public virtual bool IsRule()
        {
            return false;
        }

        //A Trigger plus a PlanBody and, optionnaly, conditions:
        //"a : b <- c
        public virtual bool IsPlan()
        {
            return false;
        }

        //The event to throw a Plan
        public virtual bool IsTrigger()
        {
            return false;
        }

        public bool IsTerm()
        {
            return true;
        }
    }
}
