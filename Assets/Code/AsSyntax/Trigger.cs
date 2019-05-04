using Assets.Code.ReasoningCycle;
using BDIManager.Beliefs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.AsSyntax
{
    public class Trigger : Structure
    {
        private TEOperator op = TEOperator.add;
        private TEType type = TEType.belief;
        private Literal literal;
        private bool isTerm = false;

        public Trigger(TEOperator op, TEType t, Literal l) : base("te", 0)
        {
            this.literal = l;
            this.op = op;
            this.type = t;
            SetTrigOp(op);
            SetSrcInfo(l.GetSrcInfo());
        } 

        public override int GetArity()
        {
            return 2;
        }
         
        public override ITerm GetTerm(int i)
        {
            switch (i)
            {
                case 0:
                    {
                        return new StringTermImpl(op.ToString() + type.ToString());
                    }
                case 1:
                    {
                        return literal;
                    } 
                default:
                    {
                        return null;
                    }
            }
        }

        public override void SetTerm(int i, ITerm t)
        {
            switch (i)
            {
                case 0:
                    {
                        //logger.warning("setTerm(i,t) for i=0 -- the operator -- , IS NOT IMPLEMENTED YET!!!");
                        break;
                    } 
                case 1:
                    {
                        literal = (Literal)t;
                        break;
                    }
            }
        }

        public void SetTrigOp(TEOperator o)
        {
            op = o;
            predicateIndicatorCache = null;
        }

        public bool SameType(Trigger e)
        {
            return op == e.op && type == e.type;
        }

        public override bool Equals(object o)
        {
            if (o != null && o.GetType() == typeof(Trigger)) {
                Trigger t = (Trigger)o;
                return (op == t.op && type == t.type && literal.Equals(t.GetLiteral()));
            }
            return false;
        }

        public bool IsAchvDesire()
        {
            return type == TEType.achieve;
        }

        public bool IsDesire()
        {
            return type == TEType.achieve || type == TEType.test;
        }

        public bool IsMetaEvent()
        {
            return op == TEOperator.desireState;
        }

        public TEOperator GetOperator()
        {
            return op;
        }

        public TEType GetTEType()
        {
            return type;
        }

        public bool IsAddition()
        {
            return op == TEOperator.add;
        }

        public new Trigger Clone()
        {
            Trigger c = new Trigger(op, type, literal.Copy());
            c.predicateIndicatorCache = predicateIndicatorCache;
            c.isTerm = isTerm;
            return c;
        }

        public new Trigger Capply(Unifier u)
        {
            Trigger c = new Trigger(op, type, (Literal)literal.Capply(u));
            c.predicateIndicatorCache = predicateIndicatorCache;
            c.isTerm = isTerm;
            return c;
        }

        public override PredicateIndicator GetPredicateIndicator()
        {
            if (predicateIndicatorCache == null)
            {
                predicateIndicatorCache = new PredicateIndicator(literal.GetNS(), op.ToString() + type + literal.GetFunctor(), literal.GetArity());
            }
            return predicateIndicatorCache;
        }

        public Literal GetLiteral()
        {
            return literal;
        }

        public void SetLiteral(Literal literal)
        {
            this.literal = literal;
            predicateIndicatorCache = null;
        }

        public void SetAsTriggerTerm(bool b)
        {
            isTerm = b;
        }

        public override string ToString()
        {
            string b, e;
            if (isTerm)
            {
                b = "{ ";
                e = " }";
            }
            else
            {
                b = "";
                e = "";
            }
            return b + op + type + literal + e;
        }

        public static Trigger TryToGetTrigger(ITerm t)
        {
            if (t.GetType() == typeof(Trigger)) 
            {
                return (Trigger) t;
            }
            if (t.IsPlanBody())
            {
                IPlanBody p = (IPlanBody)t;
                if (p.GetPlanSize() == 1)
                {
                    TEOperator op = null;
                    if (p.GetBodyType() == BodyType.addBel)
                    {
                        op = TEOperator.add;
                    } 
                    else if (p.GetBodyType() == BodyType.delBel)
                    {
                        op = TEOperator.del;
                    }
                    if (op != null)
                    {
                        Literal l = ((Literal)p.GetBodyTerm().Clone()).ForceFullLiteralImpl();
                        l.DelAnnot(BeliefBase.TSelf);
                        return new Trigger(op, TEType.belief, l);

                    }                       
                }   
            }   
            if (t.IsString())
            {
                return AsSyntax.ParseTrigger(((IStringTerm) t).GetString());
            }
            return null;
        }
    }

    public class TEType
    {
        public readonly static TEType belief;
        public readonly static TEType achieve;
        public readonly static TEType test;

        public override string ToString()
        {
            if (this == belief)
            {
                return "";
            } else if (this == achieve)
            {
                return "!";
            } else if (this == test)
            {
                return "?";
            } else
            {
                return null;
            }
        }
    }

    public class TEOperator
    {
        public readonly static TEOperator add;
        public readonly static TEOperator del;
        public readonly static TEOperator desireState;

        public override string ToString()
        {
            if (this == add)
            {
                return "+";
            }
            else if (this == del)
            {
                return "-";
            }
            else if (this == desireState)
            {
                return "^";
            }
            else
            {
                return null;
            }
        }
    }
}
