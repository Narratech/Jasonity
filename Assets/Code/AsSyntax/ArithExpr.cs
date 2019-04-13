using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Code.Exceptions;
using Assets.Code.ReasoningCycle;
using UnityEngine;

namespace Assets.Code.AsSyntax
{
    /*
     * Represents and solve arithmetic exprissions like "10 + 30".
     */
    public class ArithExpr:ArithFunctionTerm, INumberTerm
    {
        private ArithmeticOp op = ArithmeticOp.none;

        public class ArithmeticOp
        {
            public static readonly ArithmeticOp none;
            public static readonly ArithmeticOp plus;
            public static readonly ArithmeticOp minus;
            public static readonly ArithmeticOp times;
            public static readonly ArithmeticOp div;
            public static readonly ArithmeticOp mod;
            public static readonly ArithmeticOp pow;
            public static readonly ArithmeticOp intdiv;

            public double Eval(double x, double y)
            {
                if (this == none)
                {
                    return 0;
                }
                else if (this == plus)
                {
                    return x + y;
                }
                else if (this == minus)
                {
                    return x - y;
                }
                else if (this == times)
                {
                    return x * y;
                }
                else if (this == div)
                {
                    return x / y;
                }
                else if (this == mod)
                {
                    return x % y;
                }
                else if (this == pow)
                {
                    return Math.Pow(x,y);
                }
                else if (this == intdiv)
                {
                    return (int)x / (int)y;
                }
                else
                {
                    return 0;
                }
            }

            public override string ToString()
            {
                if (this == none)
                {
                    return "";
                }
                else if (this == plus)
                {
                    return "+";
                }
                else if (this == minus)
                {
                    return "-";
                }
                else if (this == times)
                {
                    return "*";
                }
                else if (this == div)
                {
                    return "/";
                }
                else if (this == mod)
                {
                    return " mod ";
                }
                else if (this == pow)
                {
                    return "**";
                }
                else if (this == intdiv)
                {
                    return " div ";
                }
                else
                {
                    return null;
                }
            }
        }

        public ArithExpr(INumberTerm t1, ArithmeticOp oper, INumberTerm t2) : base(oper.ToString(),2)
        {
            AddTerm(t1);
            AddTerm(t2);
            op = oper;
            if (t1.GetSrcInfo() != null)
                srcInfo = t1.GetSrcInfo();
            else
                srcInfo = t2.GetSrcInfo();
        }

        public ArithExpr(ArithmeticOp oper, INumberTerm t1) : base(oper.ToString(), 1)
        {
            AddTerm(t1);
            op = oper;
            srcInfo = t1.GetSrcInfo();
        }

        private ArithExpr(ArithExpr ae):base(ae)
        {
            op = ae.op;
        }

        public override ITerm Capply(Unifier u)
        {
            try
            {
                double l = ((INumberTerm)GetTerm(0).Capply(u)).Solve();
                if (IsUnary())
                {
                    if (op == ArithmeticOp.minus)
                    {
                        value = new NumberTermImpl(-l);
                    }
                    else
                    {
                        value = new NumberTermImpl(l);
                    }
                }
                else
                {
                    double r = ((INumberTerm)GetTerm(1).Capply(u)).Solve();
                    value = new NumberTermImpl(op.Eval(l, r));
                }
                return value;
            }
            catch (InvalidCastException)
            {
                Debug.Log("The value of "+this+" is not a number! Unifier = "+u+". Code: "+GetSrcInfo());
                return new NumberTermImpl(double.NaN);
            }
            catch (NoValueException)
            {
                return Clone();
            }
        }

        public bool CheckArity(int a)
        {
            return a == 1 || a == 2;
        }

        /** make a hard copy of the terms */
        public new INumberTerm Clone()
        {
            return new ArithExpr(this);
        }

        /** gets the Operation of this Expression */
        public ArithmeticOp GetOp()
        {
            return op;
        }

        /** gets the LHS of this Expression */
        public INumberTerm GetLHS()
        {
            return (INumberTerm)GetTerm(0);
        }

        /** gets the RHS of this Expression */
        public INumberTerm GetRHS()
        {
            return (INumberTerm)GetTerm(1);
        }

        public override string ToString()
        {
            if (IsUnary())
            {
                return "(" + op + GetTerm(0) + ")";
            }
            else
            {
                return "(" + GetTerm(0) + op + GetTerm(1) + ")";
            }
        }
    }
}
