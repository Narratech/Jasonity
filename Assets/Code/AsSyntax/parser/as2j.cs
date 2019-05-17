/* as2j.cs */
using Assets.Code.AsSyntax;
using Assets.Code.BDIAgent;
using Assets.Code.Stdlib;
using Assets.Code.AsSyntax.parser;
using Assets.Code.AsSyntax.directives;
using System;
using BDIManager.Beliefs;
using static RelExpr;
using static Assets.Code.AsSyntax.ArithExpr;
using System.Collections.Generic;
using Assets.Code.AsSemantics;
using System.Text.RegularExpressions;

namespace Assets.Code.parser
{

    public partial class as2j : as2jConstants
    {

        /* AgentSpeak Grammar */

        /*   agent ::= bels goals plans

             returns true if achieved the end of file
             returns false if achieved a "{ end }" directive
        */
        public bool agent(Agent a)
        {
            Literal b;
            Literal g;
            Plan p;
            curAg = a;
            asSource = a.GetASLSrc();
            bool endDir = false;

            while (!hasError)
            {
                int switch_1 = ((jj_ntk == -1) ? jj_ntk_f() : jj_ntk);
                if (false || switch_1 == 34)
                {
                    
                }
                else
                {
                    jj_la1[0] = jj_gen;
                    goto end_label_1;
                }
                endDir = directive(a);
                if (endDir) return false;
            }
        end_label_1:;
            while (!hasError)
            {
                int switch_2 = ((jj_ntk == -1) ? jj_ntk_f() : jj_ntk);
                if (false || switch_2 == VAR || switch_2 == TK_TRUE || switch_2 == TK_FALSE || switch_2 == TK_NEG || switch_2 == TK_BEGIN || switch_2 == TK_END || switch_2 == ATOM || switch_2 == UNNAMEDVARID || switch_2 == UNNAMEDVAR || switch_2 == 51)
                {
                    
                }
                else
                {
                    jj_la1[1] = jj_gen;
                    goto end_label_2;
                }
                b = belief();
                if (a != null)
                    a.AddInitialBel(b);
                while (!hasError)
                {
                    int switch_3 = ((jj_ntk == -1) ? jj_ntk_f() : jj_ntk);
                    if (false || switch_3 == 34)
                    {
                        
                    }
                    else
                    {
                        jj_la1[2] = jj_gen;
                        goto end_label_3;
                    }
                    endDir = directive(a);
                    if (endDir)
                        return false;
                }
            end_label_3:;
            }
        end_label_2:;
            while (!hasError)
            {
                int switch_4 = ((jj_ntk == -1) ? jj_ntk_f() : jj_ntk);
                if (false || switch_4 == 38)
                {
                    
                }
                else
                {
                    jj_la1[3] = jj_gen;
                    goto end_label_4;
                }
                g = initial_goal();
                if (a != null) a.AddInitialDesires(g);
                while (!hasError)
                {
                    int switch_5 = ((jj_ntk == -1) ? jj_ntk_f() : jj_ntk);
                    if (false || switch_5 == 34)
                    {
                        
                    }
                    else
                    {
                        jj_la1[4] = jj_gen;
                        goto end_label_5;
                    }
                    endDir = directive(a);
                    if (endDir) return false;
                }
            end_label_5:;
            }
        end_label_4:;
            while (!hasError)
            {
                int switch_6 = ((jj_ntk == -1) ? jj_ntk_f() : jj_ntk);
                if (false || switch_6 == TK_LABEL_AT || switch_6 == 41 || switch_6 == 42 || switch_6 == 43)
                {
                    
                }
                else
                {
                    jj_la1[5] = jj_gen;
                    goto end_label_6;
                }
                p = plan();

                if (a != null)
                {
                    p.SetSource(asSource);
                    a.GetPL().Add(p);
                }
               
                while (!hasError)
                {
                    int switch_7 = ((jj_ntk == -1) ? jj_ntk_f() : jj_ntk);
                    if (false || switch_7 == VAR || switch_7 == TK_TRUE || switch_7 == TK_FALSE || switch_7 == TK_NEG || switch_7 == TK_BEGIN || switch_7 == TK_END || switch_7 == ATOM || switch_7 == UNNAMEDVARID || switch_7 == UNNAMEDVAR|| switch_7 == 51)
                    {
                        
                    }
                    else
                    {
                        jj_la1[6] = jj_gen;
                        goto end_label_7;
                    }
                    b = belief();

                    if (a != null)
                    {
                        if (b.IsRule())
                        {
                            a.AddInitialBel(b);
                        }
                        else
                        {
                            throw new ParseException(GetSourceRef(b) + " The belief '" + b + "' is not in the begin of the source code!");
                        }
                    }
                }
            end_label_7:;
                while (!hasError)
                {
                    int switch_8 = ((jj_ntk == -1) ? jj_ntk_f() : jj_ntk);
                    if (false || switch_8 == 34)
                    {
                        
                    }
                    else
                    {
                        jj_la1[7] = jj_gen;
                        goto end_label_8;
                    }
                    endDir = directive(a);
                    if (endDir) return false;
                }
            end_label_8:;
            }
        end_label_6:;
            jj_consume_token(0);

            if (a != null)
                parsedFiles.Add(a.GetASLSrc());
            return true;
        }

        /* Directive returns true if the directive is "{ end }", false otherwise */
        public bool directive(Agent outerAg)
        {
            Pred dir = null;
            Agent resultOfDirective = null;
            Agent bakAg = curAg;
            bool isEOF = false;
            Atom oldNS = null;
            jj_consume_token(34);
            if (jj_2_1(4))
            {
                jj_consume_token(TK_BEGIN);
                dir = pred();
                jj_consume_token(35);

                Agent innerAg = new Agent();
                innerAg.InitAg();
                dir = new Pred(@namespace, dir);
                IDirective d = dProcessor.GetInstance(dir);
                d.Begin(dir, this);
                isEOF = agent(innerAg);
                if (isEOF)
                    throw new ParseException(GetSourceRef(dir) + " The directive '{ begin " + dir + "}' does not end with '{ end }'.");

                resultOfDirective = d.Process(dir, outerAg, innerAg);
                d.End(dir, this);
            }
            else
            {
                int switch_9 = ((jj_ntk == -1) ? jj_ntk_f() : jj_ntk);
                if (false || switch_9 == TK_BEGIN || switch_9 == TK_END || switch_9 == ATOM)
                {
                    dir = pred();
                    jj_consume_token(35);

                    if (dir.ToString().Equals("end"))
                        return true;

                    dir = new Pred(@namespace, dir);
                    IDirective d = dProcessor.GetInstance(dir);
                    d.Begin(dir, this); // to declare the namespace as local
                    resultOfDirective = d.Process(dir, outerAg, null);
                    d.End(dir, this);
                }
                else
                {
                    jj_la1[8] = jj_gen;
                    jj_consume_token(-1);
                    throw new ParseException();
                }
            }

            if (resultOfDirective != null && outerAg != null)
            {
                // import bels, plans and initial goals from agent resultOfDirective
                outerAg.ImportComponents(resultOfDirective);
            }
            curAg = bakAg;
            return false;
        }
        
        /* Beliefs & Rules */
        public Literal belief()
        {
            Literal h; object t;

            h = literal();

            if (h.IsVar())
            {
                throw new ParseException(GetSourceRef(h) + " variables cannot be beliefs!");
            }
            int switch_10 = ((jj_ntk == -1) ? jj_ntk_f() : jj_ntk);
            if (false || switch_10 == 36)
            {
                jj_consume_token(36);
                t = log_expr();

                h = new Rule(h, (ILogicalFormula)t);
            }
            else
            {
                jj_la1[9] = jj_gen;
            }
            jj_consume_token(37);
            return h;
        }

        /* Initial goals */
        public Literal initial_goal()
        {
            Literal g;
            jj_consume_token(38);
            g = literal();
            jj_consume_token(37);

            if (g.IsVar())
            {
                throw new ParseException(GetSourceRef(g) + ". a variable cannot be a goal!");
            }
            return g;
        }




        /* Plan */
        public Plan plan()
        {
            Token k;
            Pred L = null;
            Literal L2;
            Trigger T;
            object C = null;
            IPlanBody B = null;
            int start = -1, end;
            int switch_11 = ((jj_ntk == -1) ? jj_ntk_f() : jj_ntk);
            if (false || switch_11 == TK_LABEL_AT)
            {
                k = jj_consume_token(TK_LABEL_AT);
                L2 = literal();
                start = k.beginLine; L = new Pred(L2);
            }
            else
            {
                jj_la1[10] = jj_gen;
            }
            // use literal to allow namespace
            T = trigger();
            int switch_12 = ((jj_ntk == -1) ? jj_ntk_f() : jj_ntk);
            if (false || switch_12 == 39)
            {
                k = jj_consume_token(39);
                C = log_expr();
                if (start == -1) start = k.beginLine;
            }
            else
            {
                jj_la1[11] = jj_gen;
            }
            int switch_13 = ((jj_ntk == -1) ? jj_ntk_f() : jj_ntk);
            if (false || switch_13 == 40)
            {
                k = jj_consume_token(40);
                B = plan_body();
                if (start == -1) start = k.beginLine;
            }
            else
            {
                jj_la1[12] = jj_gen;
            }

            k = jj_consume_token(37);
            if (start == -1) start = k.beginLine;

            end = k.beginLine;
            InternalActionLiteral ial = null;
            try { ial = CheckInternalActionsInContext((ILogicalFormula)C, curAg); } catch (Exception e) { }

            if (ial != null)
                throw new ParseException(GetSourceRef(ial) + " The internal action '" + ial + "' can not be used in plan's context!");

            if (B != null && B.GetBodyTerm().Equals(Literal.LTrue))
                B = (IPlanBody)B.GetBodyNext();
            Plan p = new Plan(L, T, (ILogicalFormula)C, B);
            p.SetSrcInfo(new SourceInfo(asSource, start, end));
            return p;
        }



        /* Trigger */
        public Trigger trigger()
        {
            TEOperator teOp;
            TEType teType = TEType.belief;
            Literal F;
            int switch_14 = ((jj_ntk == -1) ? jj_ntk_f() : jj_ntk);
            if (false || switch_14 == 41)
            {
                jj_consume_token(41);
                teOp = TEOperator.add;
            }
            else if (false || switch_14 == 42)
            {
                jj_consume_token(42);
                teOp = TEOperator.del;
            }
            else if (false || switch_14 == 43)
            {
                jj_consume_token(43);
                teOp = TEOperator.desireState;
            }
            else
            {
                jj_la1[13] = jj_gen;
                jj_consume_token(-1);
                throw new ParseException();
            }
            int switch_16 = ((jj_ntk == -1) ? jj_ntk_f() : jj_ntk);
            if (false || switch_16 == 38 || switch_16 == 44)
            {
                int switch_15 = ((jj_ntk == -1) ? jj_ntk_f() : jj_ntk);
                if (false || switch_15 == 38)
                {
                    jj_consume_token(38);
                    teType = TEType.achieve;
                }
                else if (false || switch_15 == 44)
                {
                    jj_consume_token(44);
                    teType = TEType.test;
                }
                else
                {
                    jj_la1[14] = jj_gen;
                    jj_consume_token(-1);
                    throw new ParseException();
                }
            }
            else
            {
                jj_la1[15] = jj_gen;
            }


            F = literal();
            return new Trigger(teOp, teType, F.ForceFullLiteralImpl());
        }




        /* Plan body */
        public IPlanBody plan_body()
        {
            Object F; IPlanBody R = null;
            F = plan_body_term();
            int switch_17 = ((jj_ntk == -1) ? jj_ntk_f() : jj_ntk);
            if (false || switch_17 == 45)
            {
                jj_consume_token(45);
            }
            else
            {
                jj_la1[16] = jj_gen;
            }
            int switch_18 = ((jj_ntk == -1) ? jj_ntk_f() : jj_ntk);
            if (false || switch_18 == VAR || switch_18 == TK_TRUE || switch_18 == TK_FALSE || switch_18 == TK_NOT 
                || switch_18 == TK_NEG || switch_18 == TK_BEGIN || switch_18 == TK_END || switch_18 == TK_IF
                || switch_18 == TK_FOR || switch_18 == TK_WHILE || switch_18 == NUMBER || switch_18 == STRING
                || switch_18 == ATOM || switch_18 == UNNAMEDVARID || switch_18 == UNNAMEDVAR || switch_18 == 34
                || switch_18 == 38 || switch_18 == 41 || switch_18 == 42 || switch_18 == 44 || switch_18 == 46
                || switch_18 == 48 || switch_18 == 51)
            {
                R = plan_body();
            }
            else
            {
                jj_la1[17] = jj_gen;
            }

            if (R != null)
            {
                ((IPlanBody)F).SetBodyNext(R);
            }

            return (IPlanBody)F;
        }



        public IPlanBody plan_body_term()
        {
            object F;
            IPlanBody R = null;
            F = plan_body_factor();
            int switch_19 = ((jj_ntk == -1) ? jj_ntk_f() : jj_ntk);
            if (false || switch_19 == TK_POR)
            {
                jj_consume_token(TK_POR);
                R = plan_body_term();
            }
            else
            {
                jj_la1[18] = jj_gen; 
            }
            if (R == null)
            {
                return (IPlanBody)F;
            }
            else
            {
                try
                {
                    Structure s = AsSyntax.AsSyntax.CreateStructure(".fork", ForkStdLib.aOr, (ITerm)F);
                    if (R.ToString().StartsWith(".fork(or,"))
                    {
                        // if R is another fork or, put they args into this fork
                        InternalActionLiteral ial = (InternalActionLiteral)R.GetBodyTerm();
                        if (ial.GetIA(curAg).GetType() == typeof(ForkStdLib))
                        {
                            for (int i = 1; i < ial.GetArity(); i++)
                            {
                                s.AddTerm(ial.GetTerm(i));
                            }
                        }
                    }
                    else
                    {
                        s.AddTerm(R);
                    }

                    Literal stmtLiteral = new InternalActionLiteral(s, curAg);
                    stmtLiteral.SetSrcInfo(((ITerm)F).GetSrcInfo());
                    return new PlanBodyImpl(BodyType.internalAction, stmtLiteral);
                }
                catch (Exception e)
                {
                    throw new ParseException(e.Message); //Esto daba error porque estaba vac�o y como es lo que se genera s�lo he puesto una expecion porque no se que tiene que devolver
                }
            }
        }



        public IPlanBody plan_body_factor()
        {
            object F; IPlanBody R = null;
            int switch_20 = ((jj_ntk == -1) ? jj_ntk_f() : jj_ntk);
            if (false || switch_20 == TK_IF)
            {
                F = stmtIF();
            }
            else if (false
           || switch_20 == TK_FOR)
            {
                F = stmtFOR();
            }
            else if (false
           || switch_20 == TK_WHILE)
            {
                F = stmtWHILE();
            }
            else if (false || switch_20 == VAR || switch_20 == TK_TRUE || switch_20 == TK_FALSE || switch_20 == TK_NOT
            || switch_20 == TK_NEG || switch_20 == TK_BEGIN || switch_20 == TK_END || switch_20 == NUMBER 
            || switch_20 == STRING || switch_20 == ATOM || switch_20 == UNNAMEDVARID || switch_20 == UNNAMEDVAR
            || switch_20 == 34 || switch_20 == 38 || switch_20 == 41 || switch_20 == 42 || switch_20 == 44
            || switch_20 == 46 || switch_20 == 48 || switch_20 == 51)
            {
                F = body_formula();
                //isControl = false;
                if (!(F.GetType() == typeof(IPlanBody))) throw new ParseException(GetSourceRef(F) + " " + F + " is not a body literal!");


            }
            else
            {
                jj_la1[19] = jj_gen;
                jj_consume_token(-1);
                throw new ParseException();
            }
            int switch_21 = ((jj_ntk == -1) ? jj_ntk_f() : jj_ntk);
            if (false || switch_21 == TK_PAND)
            {
                jj_consume_token(TK_PAND);
                R = plan_body_factor();
            }
            else
            {
                jj_la1[20] = jj_gen;
            }

            if (R == null)
                return (IPlanBody)F;
            try
            {
                Structure s = AsSyntax.AsSyntax.CreateStructure(".fork", ForkStdLib.aAnd, (ITerm)F);
                if (R.ToString().StartsWith(".fork(and,"))
                {
                    // if R is another fork and, put they args into this fork
                    InternalActionLiteral ial = (InternalActionLiteral)R.GetBodyTerm();
                    if ((ial.GetIA(curAg)).GetType() == typeof(ForkStdLib))
                    {
                        for (int i = 1; i < ial.GetArity(); i++)
                        {
                            s.AddTerm(ial.GetTerm(i));
                        }
                    }
                }
                else
                {
                    s.AddTerm(R);
                }
                Literal stmtLiteral = new InternalActionLiteral(s, curAg);
                stmtLiteral.SetSrcInfo(((ITerm)F).GetSrcInfo());
                return new PlanBodyImpl(BodyType.internalAction, stmtLiteral);
            }
            catch (Exception e)
            {
                throw new ParseException(e.Message); //Esto daba error porque estaba vac�o y como es lo que se genera s�lo he puesto una expecion porque no se que tiene que devolver 
            }
        }




        public IPlanBody stmtIF()
        {
            IPlanBody B;
            jj_consume_token(TK_IF);

            B = stmtIFCommon();
            return B;
        }



        public IPlanBody stmtIFCommon()
        {
            object B; ITerm T1; ITerm T2 = null; Literal stmtLiteral = null;
            jj_consume_token(46);

            B = log_expr();
            jj_consume_token(47);

            T1 = rule_plan_term();
            int switch_23 = ((jj_ntk == -1) ? jj_ntk_f() : jj_ntk);
            if (false || switch_23 == TK_ELSE || switch_23 == TK_ELIF)
            {
                int switch_22 = ((jj_ntk == -1) ? jj_ntk_f() : jj_ntk);
                if (false || switch_22 == TK_ELIF)
                {
                    jj_consume_token(TK_ELIF);
                    T2 = stmtIFCommon();
                }
                else if (false || switch_22 == TK_ELSE)
                {
                    jj_consume_token(TK_ELSE);
                    T2 = rule_plan_term();
                }
                else
                {
                    jj_la1[21] = jj_gen;
                    jj_consume_token(-1);
                    throw new ParseException();
                }
            }
            else
            {
                jj_la1[22] = jj_gen;
            }
            try
            {
                if (T1.IsRule())
                {
                    throw new ParseException(GetSourceRef(T1) + " if requires a plan body.");
                }
                if (T2 == null)
                {
                    stmtLiteral = new InternalActionLiteral(AsSyntax.AsSyntax.CreateStructure(".if_then_else", (ITerm)B, T1), curAg);
                }
                else if (T2 != null)
                { // else case
                    if (T2.IsRule())
                    {
                        throw new ParseException(GetSourceRef(T2) + " if (else) requires a plan body.");
                    }
                    stmtLiteral = new InternalActionLiteral(AsSyntax.AsSyntax.CreateStructure(".if_then_else", (ITerm)B, T1, T2), curAg);
                }
                stmtLiteral.SetSrcInfo(((ITerm)B).GetSrcInfo());
                return new PlanBodyImpl(BodyType.internalAction, stmtLiteral);
            }
            catch (Exception e)
            {
                throw new ParseException(e.Message); //Esto daba error porque estaba vac�o y como es lo que se genera s�lo he puesto una expecion porque no se que tiene que devolver
            }
        }



        public IPlanBody stmtFOR()
        {
            object B; ITerm T1; Literal stmtLiteral;
            jj_consume_token(TK_FOR);
            jj_consume_token(46);

            B = log_expr();
            jj_consume_token(47);

            T1 = rule_plan_term();

            try
            {
                if (T1.IsRule())
                {
                    throw new ParseException(GetSourceRef(T1) + "for requires a plan body.");
                }
                stmtLiteral = new InternalActionLiteral(AsSyntax.AsSyntax.CreateStructure(".foreach", (ITerm)B, T1), curAg);
                stmtLiteral.SetSrcInfo(((ITerm)B).GetSrcInfo());
                return new PlanBodyImpl(BodyType.internalAction, stmtLiteral);
            }
            catch (Exception e)
            {
                throw new ParseException(e.Message); //Esto daba error porque estaba vac�o y como es lo que se genera s�lo he puesto una expecion porque no se que tiene que devolver
            }
        }



        public IPlanBody stmtWHILE()
        {
            object B; ITerm T1; Literal stmtLiteral;
            jj_consume_token(TK_WHILE);
            jj_consume_token(46);

            B = log_expr();
            jj_consume_token(47);

            T1 = rule_plan_term();

            try
            {
                if (T1.IsRule())
                {
                    throw new ParseException(GetSourceRef(T1) + "while requires a plan body.");
                }
                stmtLiteral = new InternalActionLiteral(AsSyntax.AsSyntax.CreateStructure(".loop", (ITerm)B, T1), curAg);
                stmtLiteral.SetSrcInfo(((ITerm)B).GetSrcInfo());
                return new PlanBodyImpl(BodyType.internalAction, stmtLiteral);
            }
            catch (Exception e)
            {
                throw new ParseException(e.Message); //Esto daba error porque estaba vac�o y como es lo que se genera s�lo he puesto una expecion porque no se que tiene que devolver
            }
        }




        public Object body_formula()
        {
            BodyType formType = BodyType.action; Object B;
            int switch_29 = ((jj_ntk == -1) ? jj_ntk_f() : jj_ntk);
            if (false || switch_29 == 38 || switch_29 == 41 || switch_29 == 42 || switch_29 == 44 || switch_29 == 48)
            {
                int switch_28 = ((jj_ntk == -1) ? jj_ntk_f() : jj_ntk);
                if (false || switch_28 == 38)
                {
                    jj_consume_token(38);
                    formType = BodyType.achieve;
                }
                else if (false || switch_28 == 48)
                {
                    jj_consume_token(48);
                    formType = BodyType.achieveNF;
                }
                else if (false || switch_28 == 44)
                {
                    jj_consume_token(44);
                    formType = BodyType.test;
                }
                else if (false || switch_28 == 41)
                {
                    jj_consume_token(41);
                    formType = BodyType.addBel;
                    int switch_25 = ((jj_ntk == -1) ? jj_ntk_f() : jj_ntk);
                    if (false || switch_25 == 41 || switch_25 == 49 || switch_25 == 50)
                    {
                        int switch_24 = ((jj_ntk == -1) ? jj_ntk_f() : jj_ntk);
                        if (false || switch_24 == 41)
                        {
                            jj_consume_token(41);
                            formType = BodyType.addBelNewFocus;
                        }
                        else if (false || switch_24 == 49)
                        {
                            jj_consume_token(49);
                            formType = BodyType.addBel;
                        }
                        else if (false || switch_24 == 50)
                        {
                            jj_consume_token(50);
                            formType = BodyType.addBelEnd;
                        }
                        else
                        {
                            jj_la1[23] = jj_gen;
                            jj_consume_token(-1);
                            throw new ParseException();
                        }
                    }
                    else
                    {
                        jj_la1[24] = jj_gen;
                    }
                }
                else if (false || switch_28 == 42)
                {
                    jj_consume_token(42);
                    formType = BodyType.delBel;
                    int switch_27 = ((jj_ntk == -1) ? jj_ntk_f() : jj_ntk);
                    if (false || switch_27 == 41 || switch_27 == 42)
                    {
                        int switch_26 = ((jj_ntk == -1) ? jj_ntk_f() : jj_ntk);
                        if (false || switch_26 == 41)
                        {
                            jj_consume_token(41);
                            formType = BodyType.delAddBel;
                        }
                        else if (false || switch_26 == 42)
                        {
                            jj_consume_token(42);
                            formType = BodyType.delBelNewFocus;
                        }
                        else
                        {
                            jj_la1[25] = jj_gen;
                            jj_consume_token(-1);
                            throw new ParseException();
                        }
                    }
                    else
                    {
                        jj_la1[26] = jj_gen;
                    }
                }
                else
                {
                    jj_la1[27] = jj_gen;
                    jj_consume_token(-1);
                    throw new ParseException();
                }
            }
            else
            {
                jj_la1[28] = jj_gen;
            }
            int switch_30 = ((jj_ntk == -1) ? jj_ntk_f() : jj_ntk);
            if (false || switch_30 == 34)
            {

                B = rule_plan_term();
            }
            else if (false || switch_30 == VAR || switch_30 == TK_TRUE || switch_30 == TK_FALSE || switch_30 == TK_NOT 
            || switch_30 == TK_NEG || switch_30 == TK_BEGIN || switch_30 == TK_END || switch_30 == NUMBER || switch_30 == STRING  || switch_30 == ATOM
            || switch_30 == UNNAMEDVARID || switch_30 == UNNAMEDVAR || switch_30 == 41 || switch_30 == 42 || switch_30 == 46
            || switch_30 == 51)
            {

                B = log_expr();
            }
            else
            {
                jj_la1[29] = jj_gen;
                jj_consume_token(-1);
                throw new ParseException();
            }

            if (formType == BodyType.action && (B.GetType() == typeof(RelExpr)))
            {
                return new PlanBodyImpl(BodyType.constraint, (RelExpr)B); // constraint
            }

            if (B.GetType() == typeof(Plan))
            {
                try
                {
                    InternalActionLiteral ia = null;
                    string ias = "";
                    if (formType == BodyType.delBel)
                    {
                        ia = new InternalActionLiteral(AsSyntax.AsSyntax.CreateStructure(".remove_plan", (ITerm)B), curAg);
                    }
                    else if (formType == BodyType.addBel)
                    {
                        ia = new InternalActionLiteral(AsSyntax.AsSyntax.CreateStructure(".add_plan", (ITerm)B, BeliefBase.ASelf, new Atom("begin")), curAg);
                    }
                    else if (formType == BodyType.addBelEnd)
                    {
                        ia = new InternalActionLiteral(AsSyntax.AsSyntax.CreateStructure(".add_plan", (ITerm)B, BeliefBase.ASelf, new Atom("end")), curAg);
                    }
                    else
                    {
                        throw new ParseException(GetSourceRef(B) + " Wrong combination of operator " + formType + " and plan.");
                    }
                    return new PlanBodyImpl(BodyType.internalAction, ia);
                }
                catch (Exception e)
                {

                }
            }

            if (B.GetType() == typeof(Literal))
            {
                if (((Literal)B).IsInternalAction())
                    formType = BodyType.internalAction;

                return new PlanBodyImpl(formType, (Literal)B);
            }
            else
            {
                if (formType == BodyType.test)
                {
                    if (B.GetType() == typeof(ILogicalFormula))
                        return new PlanBodyImpl(BodyType.test, (ITerm)B);  // used in ?(a & b)
                    else
                        throw new ParseException(GetSourceRef(B) + " The argument for ? is not a logical formula.");
                }
                else
                {
                    return B;
                }
            }
        }



        public ITerm rule_plan_term()
        {
            Trigger T = null; object C = null; IPlanBody B = null, B1 = null; Plan P = null;
            bool pb = true; // pb = "only plan body"
            Pred L = null;
            Literal h = null; Object t = null;

            jj_consume_token(34);
            if (jj_2_2(4))
            {
                int switch_32 = ((jj_ntk == -1) ? jj_ntk_f() : jj_ntk);
                if (false || switch_32 == TK_LABEL_AT)
                {
                    jj_consume_token(TK_LABEL_AT);
                    int switch_31 = ((jj_ntk == -1) ? jj_ntk_f() : jj_ntk);
                    if (false || switch_31 == TK_BEGIN || switch_31 == TK_END || switch_31 == ATOM)
                    {
                        L = pred();
                    }
                    else if (false || switch_31 == VAR || switch_31 == UNNAMEDVARID || switch_31 == UNNAMEDVAR)
                    {
                        L = var(Literal.DefaultNS);
                    }
                    else
                    {
                        jj_la1[30] = jj_gen;
                        jj_consume_token(-1);
                        throw new ParseException();
                    }
                    pb = false;
                }
                else
                {
                    jj_la1[31] = jj_gen;
                }

                T = trigger();
                if (T.GetTEType() != TEType.belief) pb = false;
                int switch_33 = ((jj_ntk == -1) ? jj_ntk_f() : jj_ntk);
                if (false || switch_33 == 39)
                {
                    jj_consume_token(39);
                    C = log_expr();
                    pb = false;
                }
                else
                {
                    jj_la1[32] = jj_gen;
                }
                int switch_35 = ((jj_ntk == -1) ? jj_ntk_f() : jj_ntk);
                if (false || switch_35 == 40 || switch_35 == 45)
                {
                    int switch_34 = ((jj_ntk == -1) ? jj_ntk_f() : jj_ntk);
                    if (false || switch_34 == 40)
                    {
                        jj_consume_token(40);
                        pb = false;
                    }
                    else if (false || switch_34 == 45)
                    {
                        jj_consume_token(45);
                        if (!pb) throw new ParseException(GetSourceRef(T) + " Wrong place for ';'");
                    }
                    else
                    {
                        jj_la1[33] = jj_gen;
                        jj_consume_token(-1);
                        throw new ParseException();
                    }
                }
                else
                {
                    jj_la1[34] = jj_gen;
                }
            }
            else
            {
                
            }
            if (jj_2_3(150))
            {

                h = literal();
                jj_consume_token(36);

                t = log_expr();
            }
            else
            {
                
            }
            int switch_36 = ((jj_ntk == -1) ? jj_ntk_f() : jj_ntk);
            if (false || switch_36 == VAR || switch_36 == TK_TRUE || switch_36 == TK_FALSE || switch_36 == TK_NOT
             || switch_36 == TK_NEG || switch_36 == TK_BEGIN || switch_36 == TK_END || switch_36 == TK_IF || switch_36 == TK_FOR
             || switch_36 == TK_WHILE || switch_36 == NUMBER || switch_36 == STRING || switch_36 == ATOM || switch_36 == UNNAMEDVARID
             || switch_36 == UNNAMEDVAR || switch_36 == 34 || switch_36 == 38 || switch_36 == 41 || switch_36 == 42 || switch_36 == 44
             || switch_36 == 46 || switch_36 == 48 || switch_36 == 51)
            {
                B = plan_body();
            }
            else
            {
                jj_la1[35] = jj_gen;
            }
            jj_consume_token(35);

            if (h != null)
            {
                Rule r = new Rule(h, (ILogicalFormula)t);
                r.SetAsTerm(true);
                return r;
            }

            // the plan body case
            if (T != null)
            {
                // handle the case of "+a1", parsed as TE, need to be changed to plan's body
                // handle the case of "+a1; +a2", parsed as "TE; Body"

                if (pb && L == null)
                {
                    if (T.IsAddition())
                        B1 = new PlanBodyImpl(BodyType.addBel, T.GetLiteral(), true);
                    else
                        B1 = new PlanBodyImpl(BodyType.delBel, T.GetLiteral(), true);

                    if (B != null)
                        B1.SetBodyNext(B);
                    return B1;
                }

                if (C == null && B == null && L == null)
                {
                    // handle the case of a single trigger
                    T.SetAsTriggerTerm(true);
                    return T;
                }
                else
                {
                    // handle the case of a entire plan
                    Plan p = new Plan(L, T, (ILogicalFormula)C, B);
                    p.SetSrcInfo(T.GetSrcInfo());
                    p.SetAsPlanTerm(true);
                    return p;
                }
            }

            // the case of a simple plan body term
            if (B == null)
                B = new PlanBodyImpl();
            B.SetAsBodyTerm(true);
            return B;
        }

        /* Literal */
        public Literal literal()
        {
            Pred F = null; Pred V; Token k; bool type = Literal.LPos;
            Atom NS = @namespace; Token tns = null; bool explicitAbstractNS = true;
            int switch_41 = ((jj_ntk == -1) ? jj_ntk_f() : jj_ntk);
            if (false || switch_41 == VAR || switch_41 == TK_NEG || switch_41 == TK_BEGIN || switch_41 == TK_END
            || switch_41 == ATOM || switch_41 == UNNAMEDVARID || switch_41 == UNNAMEDVAR || switch_41 == 51)
            {
                if (jj_2_4(27))
                {
                    int switch_38 = ((jj_ntk == -1) ? jj_ntk_f() : jj_ntk);
                    if (false || switch_38 == VAR || switch_38 == ATOM || switch_38 == UNNAMEDVARID || switch_38 == UNNAMEDVAR)
                    {
                        int switch_37 = ((jj_ntk == -1) ? jj_ntk_f() : jj_ntk);
                        if (false || switch_37 == ATOM)
                        {
                            tns = jj_consume_token(ATOM);

                            if (tns.image.Equals("default"))
                                NS = Literal.DefaultNS;
                            else if (tns.image.Equals("this_ns"))
                                NS = thisnamespace;
                            else
                                NS = new Atom(tns.image);
                            explicitAbstractNS = false;
                        }
                        else if (false || switch_37 == VAR || switch_37 == UNNAMEDVARID || switch_37 == UNNAMEDVAR)
                        {
                            NS = var(Literal.DefaultNS);
                            if (NS.HasAnnot())
                                throw new ParseException(GetSourceRef(NS) + " name space cannot have annotations.");
                            explicitAbstractNS = false;
                        }
                        else
                        {
                            jj_la1[36] = jj_gen;
                            jj_consume_token(-1);
                            throw new ParseException();
                        }
                    }
                    else
                    {
                        jj_la1[37] = jj_gen;
                    }
                    jj_consume_token(51);
                    if (explicitAbstractNS)
                        NS = thisnamespace;
                }
                else
                {
                    
                }
                int switch_39 = ((jj_ntk == -1) ? jj_ntk_f() : jj_ntk);
                if (false || switch_39 == TK_NEG)
                {
                    jj_consume_token(TK_NEG);
                    type = Literal.LNeg;
                }
                else
                {
                    jj_la1[38] = jj_gen;
                }
                int switch_40 = ((jj_ntk == -1) ? jj_ntk_f() : jj_ntk);
                if (false || switch_40 == TK_BEGIN || switch_40 == TK_END || switch_40 == ATOM)
                {
                    F = pred();
                }
                else if (false || switch_40 == VAR || switch_40 == UNNAMEDVARID || switch_40 == UNNAMEDVAR)
                {
                    V = var(NS);

                    VarTerm vt = (VarTerm)V;
                    vt.SetNegated(type);
                    return vt;
                }
                else
                {
                    jj_la1[39] = jj_gen;
                    jj_consume_token(-1);
                    throw new ParseException();
                }
            }
            else if (false || switch_41 == TK_TRUE)
            {
                k = jj_consume_token(TK_TRUE);
                return Literal.LTrue;
            }
            else if (false || switch_41 == TK_FALSE)
            {
                k = jj_consume_token(TK_FALSE);
                return Literal.LFalse;
            }
            else
            {
                jj_la1[40] = jj_gen;
                jj_consume_token(-1);
                throw new ParseException();
            }

            if (AsSyntax.AsSyntax.IsKeyword(F))
                NS = Literal.DefaultNS;
            NS = nsDirective.Map(NS);

            if (F.GetFunctor().IndexOf(".") >= 0)
            {
                if (F.HasAnnot())
                    throw new ParseException(GetSourceRef(F) + " Internal actions cannot have annotations.");
                if (type == Literal.LNeg)
                    throw new ParseException(GetSourceRef(F) + " Internal actions cannot be negated.");
                try
                {
                    if (F.GetFunctor().Equals(".include")) // .include needs a namespace (see its code)
                        return new InternalActionLiteral(NS, F, curAg);
                    else
                        return new InternalActionLiteral(F, curAg);
                }
                catch (Exception e)
                {

                }
            }
            return new LiteralImpl(NS, type, F);
        }



        /* Annotated Formulae */
        public Pred pred()
        {
            Token K;
            Pred p;
            List<ITerm> l;
            IListTerm lt;
            ITerm b;
            Atom ons = @namespace; @namespace = Literal.DefaultNS;

            // do not replace abstract namespace for terms

            int switch_42 = ((jj_ntk == -1) ? jj_ntk_f() : jj_ntk);
            if (false || switch_42 == ATOM)
            {

                K = jj_consume_token(ATOM);
            }
            else if (false || switch_42 == TK_BEGIN)
            {

                K = jj_consume_token(TK_BEGIN);
            }
            else if (false || switch_42 == TK_END)
            {

                K = jj_consume_token(TK_END);
            }
            else
            {
                jj_la1[41] = jj_gen;
                jj_consume_token(-1);
                throw new ParseException();
            }
            p = new Pred(K.image);
            p.SetSrcInfo(new SourceInfo(asSource, K.beginLine));


            int switch_43 = ((jj_ntk == -1) ? jj_ntk_f() : jj_ntk);
            if (false || switch_43 == 46)
            {
                jj_consume_token(46);
                l = terms();
                jj_consume_token(47);
                p.SetTerms(l);
            }
            else
            {
                jj_la1[42] = jj_gen;
            }
            int switch_44 = ((jj_ntk == -1) ? jj_ntk_f() : jj_ntk);
            if (false || switch_44 == 53)
            {

                lt = list();
                p.SetAnnots(lt);
            }
            else
            {
                jj_la1[43] = jj_gen;
            }
            @namespace = ons;
            return p;
        }




        /* List of terms */
        public List<ITerm> terms()
        {
            List<ITerm> listTerms = new List<ITerm>(); ITerm v; IPlanBody o;

            v = term();
            listTerms.Add(v);
            while (!hasError)
            {
                int switch_45 = ((jj_ntk == -1) ? jj_ntk_f() : jj_ntk);
                if (false || switch_45 == 52)
                {
                    
                }
                else
                {
                    jj_la1[44] = jj_gen;
                    goto end_label_9;
                }
                jj_consume_token(52);
                v = term();
                listTerms.Add(v);
            }
        end_label_9:;
            listTerms.TrimExcess();
            return listTerms;
        }


        public ITerm term()
        {
            object o;
            int switch_46 = ((jj_ntk == -1) ? jj_ntk_f() : jj_ntk);
            if (false || switch_46 == 53)
            {
                o = list();
            }
            else if (false || switch_46 == 34)
            {
                o = rule_plan_term();
            }
            else if (false || switch_46 == VAR || switch_46 == TK_TRUE || switch_46 == TK_FALSE || switch_46 == TK_NOT
            || switch_46 == TK_NEG || switch_46 == TK_BEGIN || switch_46 == TK_END || switch_46 == NUMBER || switch_46 == STRING || switch_46 == ATOM
            || switch_46 == UNNAMEDVARID || switch_46 == UNNAMEDVAR || switch_46 == 41 || switch_46 == 42 || switch_46 == 46
            || switch_46 == 51)
            {
                o = log_expr();
            }
            else
            {
                jj_la1[45] = jj_gen;
                jj_consume_token(-1);
                throw new ParseException();
            }
            return ChangeToAtom(o);
        }




        public ListTermImpl list()
        {
            ListTermImpl lt = new ListTermImpl(); IListTerm last; Token K; ITerm f;
            Atom ons = @namespace; @namespace = Literal.DefaultNS;

            // do not replace abstract namespace for terms

            jj_consume_token(53);
            int switch_50 = ((jj_ntk == -1) ? jj_ntk_f() : jj_ntk);
            if (false || switch_50 == VAR || switch_50 == TK_TRUE || switch_50 == TK_FALSE || switch_50 == TK_NEG
            || switch_50 == TK_BEGIN || switch_50 == TK_END || switch_50 == NUMBER || switch_50 == STRING || switch_50 == ATOM
            || switch_50 == UNNAMEDVARID || switch_50 == UNNAMEDVAR || switch_50 == 34 || switch_50 == 41 || switch_50 == 42
            || switch_50 == 46 || switch_50 == 51 || switch_50 == 53)
            {

                f = term_in_list();
                last = lt.Append(f); lt.SetSrcInfo(f.GetSrcInfo());
                while (!hasError)
                {
                    int switch_47 = ((jj_ntk == -1) ? jj_ntk_f() : jj_ntk);
                    if (false
                       || switch_47 == 52)
                    {
                        
                    }
                    else
                    {
                        jj_la1[46] = jj_gen;
                        goto end_label_10;
                    }
                    jj_consume_token(52);
                    f = term_in_list();
                    last = last.Append(f);
                }
            end_label_10:;
                int switch_49 = ((jj_ntk == -1) ? jj_ntk_f() : jj_ntk);
                if (false || switch_49 == 54)
                {
                    jj_consume_token(54);
                    int switch_48 = ((jj_ntk == -1) ? jj_ntk_f() : jj_ntk);
                    if (false || switch_48 == VAR)
                    {
                        K = jj_consume_token(VAR);
                        last.SetNext(new VarTerm(K.image));
                    }
                    else if (false || switch_48 == UNNAMEDVAR)
                    {
                        K = jj_consume_token(UNNAMEDVAR);
                        last.SetNext(UnnamedVar.Create(K.image));
                    }
                    else if (false || switch_48 == 53)
                    {
                        f = list();
                        last = last.Concat((IListTerm)f);
                    }
                    else
                    {
                        jj_la1[47] = jj_gen;
                        jj_consume_token(-1);
                        throw new ParseException();
                    }
                }
                else
                {
                    jj_la1[48] = jj_gen;
                    
                }
            }
            else
            {
                jj_la1[49] = jj_gen;
                
            }
            jj_consume_token(55);
            @namespace = ons; return lt;
        }



        // term_in_list is the same as term, but log_expr/plan_body must be enclosed by "("....")" to avoid problem with |
        public ITerm term_in_list()
        {
            object o;
            int switch_51 = ((jj_ntk == -1) ? jj_ntk_f() : jj_ntk);
            if (false || switch_51 == 53)
            {
                o = list();
            }
            else if (false || switch_51 == VAR || switch_51 == TK_TRUE || switch_51 == TK_FALSE || switch_51 == TK_NEG
            || switch_51 == TK_BEGIN || switch_51 == TK_END || switch_51 == NUMBER || switch_51 == ATOM || switch_51 == UNNAMEDVARID
            || switch_51 == UNNAMEDVAR || switch_51 == 41 || switch_51 == 42 || switch_51 == 46 || switch_51 == 51)
            {
                o = arithm_expr();
            }
            else if (false || switch_51 == STRING)
            {
                o = stringMethod();
            }
            else if (false || switch_51 == 34)
            {
                o = rule_plan_term();
            }
            else
            {
                jj_la1[50] = jj_gen;
                jj_consume_token(-1);
                throw new ParseException();
            }
            return ChangeToAtom(o);
        }




        /* logical expression */
        public Object log_expr()
        {
            object t1, t2;
            t1 = log_expr_trm();
            int switch_52 = ((jj_ntk == -1) ? jj_ntk_f() : jj_ntk);
            if (false || switch_52 == 54)
            {
                jj_consume_token(54);
                t2 = log_expr();
                return new LogExpr((ILogicalFormula)t1, LogicalOp.or, (ILogicalFormula)t2);
            }
            else
            {
                jj_la1[51] = jj_gen;
            }
            return t1;
        }



        public Object log_expr_trm()
        {
            object t1, t2;
            t1 = log_expr_factor();
            int switch_53 = ((jj_ntk == -1) ? jj_ntk_f() : jj_ntk);
            if (false || switch_53 == 56)
            {
                jj_consume_token(56);
                t2 = log_expr_trm();
                return new LogExpr((ILogicalFormula)t1, LogicalOp.and, (ILogicalFormula)t2);
            }
            else
            {
                jj_la1[52] = jj_gen;
            }
            return t1;
        }



        public Object log_expr_factor()
        {
            object t;
            int switch_54 = ((jj_ntk == -1) ? jj_ntk_f() : jj_ntk);
            if (false || switch_54 == TK_NOT)
            {
                jj_consume_token(TK_NOT);
                t = log_expr_factor();
                return new LogExpr(LogicalOp.not, (ILogicalFormula)t);
            }
            else if (false || switch_54 == VAR || switch_54 == TK_TRUE || switch_54 == TK_FALSE || switch_54 == TK_NEG
            || switch_54 == TK_BEGIN || switch_54 == TK_END || switch_54 == NUMBER || switch_54 == STRING || switch_54 == ATOM
            || switch_54 == UNNAMEDVARID || switch_54 == UNNAMEDVAR || switch_54 == 41 || switch_54 == 42 || switch_54 == 46
            || switch_54 == 51)
            {

                t = rel_expr();
                return t;
            }
            else
            {
                jj_la1[53] = jj_gen;
                jj_consume_token(-1);
                throw new ParseException();
            }
        }




        /* relational expression
           used in context, body and term

             <VAR>      [ <OPREL> <EXP> ]  --> this method returns the VarTerm
           | <LITERAL>  [ <OPREL> <EXP> ]  --> returns the Literal
           | <EXP>      [ <OPREL> <EXP> ]  --> returns the ExprTerm
        */
        public Object rel_expr()
        {
            object op1 = null;
            object op2 = null;
            RelationalOp operatorR = RelationalOp.none;

            int switch_55 = ((jj_ntk == -1) ? jj_ntk_f() : jj_ntk);
            if (false || switch_55 == VAR || switch_55 == TK_TRUE || switch_55 == TK_FALSE || switch_55 == TK_NEG
            || switch_55 == TK_BEGIN || switch_55 == TK_END || switch_55 == NUMBER || switch_55 == ATOM
            || switch_55 == UNNAMEDVARID || switch_55 == UNNAMEDVAR || switch_55 == 41 || switch_55 == 42 || switch_55 == 46
            || switch_55 == 51)
            {
                op1 = arithm_expr();
            }
            else if (false || switch_55 == STRING)
            {
                op1 = stringMethod();
            }
            else
            {
                jj_la1[54] = jj_gen;
                jj_consume_token(-1);
                throw new ParseException();
            }
            int switch_58 = ((jj_ntk == -1) ? jj_ntk_f() : jj_ntk);
            if (false || switch_58 == 49 || switch_58 == 50 || switch_58 == 57 || switch_58 == 58 || switch_58 == 59 
            || switch_58 == 60 || switch_58 == 61 || switch_58 == 62)
            {
                int switch_56 = ((jj_ntk == -1) ? jj_ntk_f() : jj_ntk);
                if (false || switch_56 == 49)
                {
                    jj_consume_token(49);
                    operatorR = RelationalOp.lt;
                }
                else if (false || switch_56 == 57)
                {
                    jj_consume_token(57);
                    operatorR = RelationalOp.lte;
                }
                else if (false || switch_56 == 50)
                {
                    jj_consume_token(50);
                    operatorR = RelationalOp.gt;
                }
                else if (false || switch_56 == 58)
                {
                    jj_consume_token(58);
                    operatorR = RelationalOp.gte;
                }
                else if (false || switch_56 == 59)
                {
                    jj_consume_token(59);
                    operatorR = RelationalOp.eq;
                }
                else if (false || switch_56 == 60)
                {
                    jj_consume_token(60);
                    operatorR = RelationalOp.dif;
                }
                else if (false || switch_56 == 61)
                {
                    jj_consume_token(61);
                    operatorR = RelationalOp.unify;
                }
                else if (false || switch_56 == 62)
                {
                    jj_consume_token(62);
                    operatorR = RelationalOp.literalBuilder;
                }
                else
                {
                    jj_la1[55] = jj_gen;
                    jj_consume_token(-1);
                    throw new ParseException();
                }
                int switch_57 = ((jj_ntk == -1) ? jj_ntk_f() : jj_ntk);
                if (false || switch_57 == VAR || switch_57 == TK_TRUE || switch_57 == TK_FALSE || switch_57 == TK_NEG 
                || switch_57 == TK_BEGIN || switch_57 == TK_END || switch_57 == NUMBER || switch_57 == ATOM
                || switch_57 == UNNAMEDVARID || switch_57 == UNNAMEDVAR || switch_57 == 41 || switch_57 == 42 || switch_57 == 46
                || switch_57 == 51)
                {
                    op2 = arithm_expr();
                }
                else if (false || switch_57 == STRING)
                {
                    op2 = stringMethod();
                }
                else if (false || switch_57 == 53)
                {
                    op2 = list();
                }
                else if (false || switch_57 == 34)
                {
                    op2 = rule_plan_term();
                }
                else
                {
                    jj_la1[56] = jj_gen;
                    jj_consume_token(-1);
                    throw new ParseException();
                }

                if (((ITerm)op1).IsInternalAction() && operatorR != RelationalOp.literalBuilder)
                    throw new ParseException(GetSourceRef(op1) + " RelExpr: operand '" + op1 + "' can not be an internal action.");

                if (((ITerm)op2).IsInternalAction() && operatorR != RelationalOp.literalBuilder)
                    throw new ParseException(GetSourceRef(op2) + " RelExpr: operand '" + op2 + "' can not be an internal action.");

                return new RelExpr((ITerm)op1, operatorR, (ITerm)op2);
            }
            else
            {
                jj_la1[57] = jj_gen;
            }
            return op1;
        }

        /* arithmetic expression */
        public Object arithm_expr()
        {
            object t1, t2; ArithmeticOp op;
            t1 = arithm_expr_trm();
            op = ArithmeticOp.none;
            while (!hasError)
            {
                int switch_59 = ((jj_ntk == -1) ? jj_ntk_f() : jj_ntk);
                if (false || switch_59 == 41 || switch_59 == 42)
                {
                    
                }
                else
                {
                    jj_la1[58] = jj_gen;
                    goto end_label_11;
                }
                int switch_60 = ((jj_ntk == -1) ? jj_ntk_f() : jj_ntk);
                if (false || switch_60 == 41)
                {
                    jj_consume_token(41);
                    op = ArithmeticOp.plus;
                }
                else if (false || switch_60 == 42)
                {
                    jj_consume_token(42);
                    op = ArithmeticOp.minus;
                }
                else
                {
                    jj_la1[59] = jj_gen;
                    jj_consume_token(-1);
                    throw new ParseException();
                }

                t2 = arithm_expr_trm();

                if (!(t1.GetType() == typeof(INumberTerm)))
                {
                    throw new ParseException(GetSourceRef(t1) + " ArithExpr: first operand '" + t1 + "' is not numeric or variable.");
                }

                if (!(t2.GetType() == typeof(INumberTerm)))
                {
                    throw new ParseException(GetSourceRef(t2) + " ArithExpr: second operand '" + t2 + "' is not numeric or variable.");
                }
                t1 = new ArithExpr((INumberTerm)t1, op, (INumberTerm)t2);
            }
        end_label_11:;
            return t1;
        }

        public Object arithm_expr_trm()
        {
            object t1, t2; ArithmeticOp op;
            t1 = arithm_expr_factor();
            op = ArithmeticOp.none;
            while (!hasError)
            {
                int switch_61 = ((jj_ntk == -1) ? jj_ntk_f() : jj_ntk);
                if (false || switch_61 == TK_INTDIV || switch_61 == TK_INTMOD || switch_61 == 63 || switch_61 == 64)
                {
                    
                }
                else
                {
                    jj_la1[60] = jj_gen;
                    goto end_label_12;
                }
                int switch_62 = ((jj_ntk == -1) ? jj_ntk_f() : jj_ntk);
                if (false || switch_62 == 63)
                {
                    jj_consume_token(63);
                    op = ArithmeticOp.times;
                }
                else if (false || switch_62 == 64)
                {
                    jj_consume_token(64);
                    op = ArithmeticOp.div;
                }
                else if (false || switch_62 == TK_INTDIV)
                {
                    jj_consume_token(TK_INTDIV);
                    op = ArithmeticOp.intdiv;
                }
                else if (false || switch_62 == TK_INTMOD)
                {
                    jj_consume_token(TK_INTMOD);
                    op = ArithmeticOp.mod;
                }
                else
                {
                    jj_la1[61] = jj_gen;
                    jj_consume_token(-1);
                    throw new ParseException();
                }

                t2 = arithm_expr_factor();

                if (!(t1.GetType() == typeof(INumberTerm)))
                {
                    throw new ParseException(GetSourceRef(t1) + " ArithTerm: first operand '" + t1 + "' is not numeric or variable.");
                }
                if (!(t2.GetType() == typeof(INumberTerm)))
                {
                    throw new ParseException(GetSourceRef(t2) + " ArithTerm: second operand '" + t2 + "' is not numeric or variable.");
                }
                t1 = new ArithExpr((INumberTerm)t1, op, (INumberTerm)t2);
            }
        end_label_12:;
            return t1;
        }

        public Object arithm_expr_factor()
        {
            object t1, t2; ArithmeticOp op;

            t1 = arithm_expr_simple();
            op = ArithmeticOp.none;
            int switch_63 = ((jj_ntk == -1) ? jj_ntk_f() : jj_ntk);
            if (false || switch_63 == 65)
            {
                jj_consume_token(65);
                op = ArithmeticOp.pow;

                t2 = arithm_expr_factor();

                if (!(t1.GetType() == typeof(INumberTerm)))
                {
                    throw new ParseException(GetSourceRef(t1) + " ArithFactor: first operand '" + t1 + "' is not numeric or variable.");
                }

                if (!(t2.GetType() == typeof(INumberTerm)))
                {
                    throw new ParseException(GetSourceRef(t2) + " ArithFactor: second operand '" + t2 + "' is not numeric or variable.");
                }
                return new ArithExpr((INumberTerm)t1, op, (INumberTerm)t2);
            }
            else
            {
                jj_la1[62] = jj_gen;
            }
            return t1;
        }

        public Object arithm_expr_simple()
        {
            Token K; object t; VarTerm v;
            int switch_64 = ((jj_ntk == -1) ? jj_ntk_f() : jj_ntk);
            if (false
               || switch_64 == NUMBER)
            {
                K = jj_consume_token(NUMBER);
                INumberTerm ni = AsSyntax.AsSyntax.ParseNumber(K.image);
                ni.SetSrcInfo(new SourceInfo(asSource, K.beginLine));
                return ni;
            }
            else if (false || switch_64 == 42)
            {
                jj_consume_token(42);
                t = arithm_expr_simple();

                if (!(t.GetType() == typeof(INumberTerm)))
                {
                    throw new ParseException(GetSourceRef(t) + " The argument '" + t + "' of operator '-' is not numeric or variable.");
                }
                return new ArithExpr(ArithmeticOp.minus, (INumberTerm)t);
            }
            else if (false || switch_64 == 41)
            {
                jj_consume_token(41);
                t = arithm_expr_simple();

                if (!(t.GetType() == typeof(INumberTerm)))
                {
                    throw new ParseException(GetSourceRef(t) + " The argument '" + t + "' of operator '+' is not numeric or variable.");
                }
                return new ArithExpr(ArithmeticOp.plus, (INumberTerm)t);





            }
            else if (false || switch_64 == 46)
            {
                jj_consume_token(46);
                t = log_expr();
                jj_consume_token(47);
                return t;
            }
            else if (false || switch_64 == VAR || switch_64 == TK_TRUE || switch_64 == TK_FALSE || switch_64 == TK_NEG
            || switch_64 == TK_BEGIN|| switch_64 == TK_END || switch_64 == ATOM || switch_64 == UNNAMEDVARID || switch_64 == UNNAMEDVAR
            || switch_64 == 51)
            {
                t = function();
                return t;
            }
            else
            {
                jj_la1[63] = jj_gen;
                jj_consume_token(-1);
                throw new ParseException();
            }
        }



        public ITerm function()
        {
            Literal l; //Mirar bien para ver que hacemos con los arithfunction

            l = literal();
            ArithFunction af = GetArithFunction(l);
            if (af == null)
            {
                return l;
            }
            else
            {
                ArithFunctionTerm at = new ArithFunctionTerm(af);
                //ArithFunction at = new ArithFunction(af);
                at.SetSrcInfo(l.GetSrcInfo());
                at.SetTerms(l.GetTerms());
                at.SetAgent(curAg);
                return at;
            }
        }



        public VarTerm var(Atom ns)
        {
            Token K;
            VarTerm v;
            IListTerm lt = null;
            int switch_65 = ((jj_ntk == -1) ? jj_ntk_f() : jj_ntk);
            if (false || switch_65 == VAR)
            {
                K = jj_consume_token(VAR);
                v = new VarTerm(ns, K.image); v.SetSrcInfo(new SourceInfo(asSource, K.beginLine));
            }
            else if (false || switch_65 == UNNAMEDVARID)
            {
                K = jj_consume_token(UNNAMEDVARID);
                //Mirar la clase Regex y el match 
                //Matcher m = patternUnnamedWithId.matcher(K.image);
                Match matcher = patternUnnamedWithId.Match(K.image); //CAMBIAR: Clase Matcher. Mirar las expresiones regulares. Mirar RegEx
                if (matcher.Success/*find()*/)
                {
                    v = UnnamedVar.Create(ns, int.Parse(matcher.Value/*.group(1)*/), K.image);
                }
                else
                {
                    v = UnnamedVar.Create(ns, K.image);
                }
            }
            else if (false || switch_65 == UNNAMEDVAR)
            {
                K = jj_consume_token(UNNAMEDVAR);
                v = UnnamedVar.Create(ns, K.image);
            }
            else
            {
                jj_la1[64] = jj_gen;
                jj_consume_token(-1);
                throw new ParseException();
            }
            int switch_66 = ((jj_ntk == -1) ? jj_ntk_f() : jj_ntk);
            if (false || switch_66 == 53)
            {

                lt = list();
                v.SetAnnots(lt);
            }
            else
            {
                jj_la1[65] = jj_gen;
            }
            return v;
        }



        public IStringTerm stringMethod()
        {
            Token k;
            StringTermImpl s;

            k = jj_consume_token(STRING);
            s = new StringTermImpl(k.image.Substring(1, k.image.Length - 1).Replace("\\\\n", "\n").Replace("\\\\\"", "\""));
            s.SetSrcInfo(new SourceInfo(asSource, k.beginLine));
            return s;
        }

        private bool jj_2_1(int xla)
        {
            jj_la = xla; jj_lastpos = jj_scanpos = token;
            jj_done = false;
            if (!jj_3_1() || jj_done) return true;
            jj_save(1, xla);
            return false;
        }

        private bool jj_2_2(int xla)
        {
            jj_la = xla; jj_lastpos = jj_scanpos = token;
            jj_done = false;
            if (!jj_3_2() || jj_done) return true;
            jj_save(2, xla);
            return false;
        }

        private bool jj_2_3(int xla)
        {
            jj_la = xla; jj_lastpos = jj_scanpos = token;
            jj_done = false;
            if (!jj_3_3() || jj_done) return true;
            jj_save(3, xla);
            return false;
        }

        private bool jj_2_4(int xla)
        {
            jj_la = xla; jj_lastpos = jj_scanpos = token;
            jj_done = false;
            if (!jj_3_4() || jj_done) return true;
            jj_save(4, xla);
            return false;
        }

        private bool jj_3R_95()
        {
            if (jj_done) return true;
            if (jj_scan_token(54)) return true;
            Token xsp;
            xsp = jj_scanpos;
            if (jj_3R_98())
            {
                jj_scanpos = xsp;
                if (jj_3R_99())
                {
                    jj_scanpos = xsp;
                    if (jj_3R_100()) return true;
                }
            }
            return false;
        }

        private bool jj_3R_94()
        {
            if (jj_done) return true;
            if (jj_scan_token(52)) return true;
            if (jj_3R_69()) return true;
            return false;
        }

        private bool jj_3R_58()
        {
            if (jj_done) return true;
            if (jj_3R_69()) return true;
            Token xsp;
            while (true)
            {
                xsp = jj_scanpos;
                if (jj_3R_94()) { jj_scanpos = xsp; break; }
            }
            xsp = jj_scanpos;
            if (jj_3R_95()) jj_scanpos = xsp;
            return false;
        }

        private bool jj_3R_44()
        {
            if (jj_done) return true;
            if (jj_scan_token(53)) return true;
            Token xsp;
            xsp = jj_scanpos;
            if (jj_3R_58()) jj_scanpos = xsp;
            if (jj_scan_token(55)) return true;
            return false;
        }

        private bool jj_3R_126()
        {
            if (jj_done) return true;
            if (jj_3R_131()) return true;
            return false;
        }

        private bool jj_3R_125()
        {
            if (jj_done) return true;
            if (jj_3R_130()) return true;
            return false;
        }

        private bool jj_3R_124()
        {
            if (jj_done) return true;
            if (jj_3R_129()) return true;
            return false;
        }

        private bool jj_3R_127()
        {
            if (jj_done) return true;
            if (jj_scan_token(TK_PAND)) return true;
            if (jj_3R_121()) return true;
            return false;
        }

        private bool jj_3R_123()
        {
            if (jj_done) return true;
            if (jj_3R_128()) return true;
            return false;
        }

        private bool jj_3R_121()
        {
            if (jj_done) return true;
            Token xsp;
            xsp = jj_scanpos;
            if (jj_3R_123())
            {
                jj_scanpos = xsp;
                if (jj_3R_124())
                {
                    jj_scanpos = xsp;
                    if (jj_3R_125())
                    {
                        jj_scanpos = xsp;
                        if (jj_3R_126()) return true;
                    }
                }
            }
            xsp = jj_scanpos;
            if (jj_3R_127()) jj_scanpos = xsp;
            return false;
        }

        private bool jj_3R_68()
        {
            if (jj_done) return true;
            if (jj_3R_20()) return true;
            return false;
        }

        private bool jj_3R_67()
        {
            if (jj_done) return true;
            if (jj_3R_71()) return true;
            return false;
        }

        private bool jj_3R_66()
        {
            if (jj_done) return true;
            if (jj_3R_44()) return true;
            return false;
        }

        private bool jj_3R_57()
        {
            if (jj_done) return true;
            Token xsp;
            xsp = jj_scanpos;
            if (jj_3R_66())
            {
                jj_scanpos = xsp;
                if (jj_3R_67())
                {
                    jj_scanpos = xsp;
                    if (jj_3R_68()) return true;
                }
            }
            return false;
        }

        private bool jj_3R_81()
        {
            if (jj_done) return true;
            if (jj_scan_token(52)) return true;
            if (jj_3R_57()) return true;
            return false;
        }

        private bool jj_3R_43()
        {
            if (jj_done) return true;
            if (jj_3R_57()) return true;
            Token xsp;
            while (true)
            {
                xsp = jj_scanpos;
                if (jj_3R_81()) { jj_scanpos = xsp; break; }
            }
            return false;
        }

        private bool jj_3R_23()
        {
            if (jj_done) return true;
            if (jj_3R_44()) return true;
            return false;
        }

        private bool jj_3R_22()
        {
            if (jj_done) return true;
            if (jj_scan_token(46)) return true;
            if (jj_3R_43()) return true;
            if (jj_scan_token(47)) return true;
            return false;
        }

        private bool jj_3R_122()
        {
            if (jj_done) return true;
            if (jj_scan_token(TK_POR)) return true;
            if (jj_3R_118()) return true;
            return false;
        }

        private bool jj_3R_118()
        {
            if (jj_done) return true;
            if (jj_3R_121()) return true;
            Token xsp;
            xsp = jj_scanpos;
            if (jj_3R_122()) jj_scanpos = xsp;
            return false;
        }

        private bool jj_3R_119()
        {
            if (jj_done) return true;
            if (jj_3R_112()) return true;
            return false;
        }

        private bool jj_3R_13()
        {
            if (jj_done) return true;
            Token xsp;
            xsp = jj_scanpos;
            if (jj_scan_token(26))
            {
                jj_scanpos = xsp;
                if (jj_scan_token(14))
                {
                    jj_scanpos = xsp;
                    if (jj_scan_token(15)) return true;
                }
            }
            xsp = jj_scanpos;
            if (jj_3R_22()) jj_scanpos = xsp;
            xsp = jj_scanpos;
            if (jj_3R_23()) jj_scanpos = xsp;
            return false;
        }

        private bool jj_3R_112()
        {
            if (jj_done) return true;
            if (jj_3R_118()) return true;
            Token xsp;
            xsp = jj_scanpos;
            if (jj_scan_token(45)) jj_scanpos = xsp;
            xsp = jj_scanpos;
            if (jj_3R_119()) jj_scanpos = xsp;
            return false;
        }

        private bool jj_3R_51()
        {
            if (jj_done) return true;
            if (jj_scan_token(44)) return true;
            return false;
        }

        private bool jj_3R_50()
        {
            if (jj_done) return true;
            if (jj_scan_token(38)) return true;
            return false;
        }

        private bool jj_3R_33()
        {
            if (jj_done) return true;
            Token xsp;
            xsp = jj_scanpos;
            if (jj_3R_50())
            {
                jj_scanpos = xsp;
                if (jj_3R_51()) return true;
            }
            return false;
        }

        private bool jj_3R_32()
        {
            if (jj_done) return true;
            if (jj_scan_token(43)) return true;
            return false;
        }

        private bool jj_3R_31()
        {
            if (jj_done) return true;
            if (jj_scan_token(42)) return true;
            return false;
        }

        private bool jj_3R_30()
        {
            if (jj_done) return true;
            if (jj_scan_token(41)) return true;
            return false;
        }

        private bool jj_3R_16()
        {
            if (jj_done) return true;
            Token xsp;
            xsp = jj_scanpos;
            if (jj_3R_30())
            {
                jj_scanpos = xsp;
                if (jj_3R_31())
                {
                    jj_scanpos = xsp;
                    if (jj_3R_32()) return true;
                }
            }
            xsp = jj_scanpos;
            if (jj_3R_33()) jj_scanpos = xsp;
            if (jj_3R_19()) return true;
            return false;
        }

        private bool jj_3R_38()
        {
            if (jj_done) return true;
            if (jj_scan_token(TK_FALSE)) return true;
            return false;
        }

        private bool jj_3R_37()
        {
            if (jj_done) return true;
            if (jj_scan_token(TK_TRUE)) return true;
            return false;
        }

        private bool jj_3R_54()
        {
            if (jj_done) return true;
            if (jj_3R_49()) return true;
            return false;
        }

        private bool jj_3R_53()
        {
            if (jj_done) return true;
            if (jj_3R_13()) return true;
            return false;
        }

        private bool jj_3R_52()
        {
            if (jj_done) return true;
            if (jj_scan_token(TK_NEG)) return true;
            return false;
        }

        private bool jj_3R_42()
        {
            if (jj_done) return true;
            if (jj_3R_49()) return true;
            return false;
        }

        private bool jj_3R_59()
        {
            if (jj_done) return true;
            if (jj_scan_token(TK_LABEL_AT)) return true;
            return false;
        }

        private bool jj_3R_48()
        {
            if (jj_done) return true;
            Token xsp;
            xsp = jj_scanpos;
            if (jj_3R_59()) jj_scanpos = xsp;
            if (jj_3R_16()) return true;
            return false;
        }

        private bool jj_3R_41()
        {
            if (jj_done) return true;
            if (jj_scan_token(ATOM)) return true;
            return false;
        }

        private bool jj_3R_21()
        {
            if (jj_done) return true;
            Token xsp;
            xsp = jj_scanpos;
            if (jj_3R_41())
            {
                jj_scanpos = xsp;
                if (jj_3R_42()) return true;
            }
            return false;
        }

        private bool jj_3_4()
        {
            if (jj_done) return true;
            Token xsp;
            xsp = jj_scanpos;
            if (jj_3R_21()) jj_scanpos = xsp;
            if (jj_scan_token(51)) return true;
            return false;
        }

        private bool jj_3R_36()
        {
            if (jj_done) return true;
            Token xsp;
            xsp = jj_scanpos;
            if (jj_3_4()) jj_scanpos = xsp;
            xsp = jj_scanpos;
            if (jj_3R_52()) jj_scanpos = xsp;
            xsp = jj_scanpos;
            if (jj_3R_53())
            {
                jj_scanpos = xsp;
                if (jj_3R_54()) return true;
            }
            return false;
        }

        private bool jj_3R_19()
        {
            if (jj_done) return true;
            Token xsp;
            xsp = jj_scanpos;
            if (jj_3R_36())
            {
                jj_scanpos = xsp;
                if (jj_3R_37())
                {
                    jj_scanpos = xsp;
                    if (jj_3R_38()) return true;
                }
            }
            return false;
        }

        private bool jj_3R_47()
        {
            if (jj_done) return true;
            if (jj_scan_token(38)) return true;
            return false;
        }

        private bool jj_3R_46()
        {
            if (jj_done) return true;
            if (jj_3R_19()) return true;
            return false;
        }

        private bool jj_3R_80()
        {
            if (jj_done) return true;
            if (jj_scan_token(STRING)) return true;
            return false;
        }

        private bool jj_3R_63()
        {
            if (jj_done) return true;
            if (jj_3R_44()) return true;
            return false;
        }

        private bool jj_3R_62()
        {
            if (jj_done) return true;
            if (jj_scan_token(UNNAMEDVAR)) return true;
            return false;
        }

        private bool jj_3R_29()
        {
            if (jj_done) return true;
            if (jj_3R_49()) return true;
            return false;
        }

        private bool jj_3R_61()
        {
            if (jj_done) return true;
            if (jj_scan_token(UNNAMEDVARID)) return true;
            return false;
        }

        private bool jj_3R_60()
        {
            if (jj_done) return true;
            if (jj_scan_token(VAR)) return true;
            return false;
        }

        private bool jj_3R_49()
        {
            if (jj_done) return true;
            Token xsp;
            xsp = jj_scanpos;
            if (jj_3R_60())
            {
                jj_scanpos = xsp;
                if (jj_3R_61())
                {
                    jj_scanpos = xsp;
                    if (jj_3R_62()) return true;
                }
            }
            xsp = jj_scanpos;
            if (jj_3R_63()) jj_scanpos = xsp;
            return false;
        }

        private bool jj_3R_28()
        {
            if (jj_done) return true;
            if (jj_3R_13()) return true;
            return false;
        }

        private bool jj_3R_105()
        {
            if (jj_done) return true;
            if (jj_3R_112()) return true;
            return false;
        }

        private bool jj_3_3()
        {
            if (jj_done) return true;
            if (jj_3R_19()) return true;
            if (jj_scan_token(36)) return true;
            if (jj_3R_20()) return true;
            return false;
        }

        private bool jj_3R_120()
        {
            if (jj_done) return true;
            if (jj_3R_19()) return true;
            return false;
        }

        private bool jj_3R_35()
        {
            if (jj_done) return true;
            if (jj_scan_token(45)) return true;
            return false;
        }

        private bool jj_3R_34()
        {
            if (jj_done) return true;
            if (jj_scan_token(40)) return true;
            return false;
        }

        private bool jj_3R_18()
        {
            if (jj_done) return true;
            Token xsp;
            xsp = jj_scanpos;
            if (jj_3R_34())
            {
                jj_scanpos = xsp;
                if (jj_3R_35()) return true;
            }
            return false;
        }

        private bool jj_3R_17()
        {
            if (jj_done) return true;
            if (jj_scan_token(39)) return true;
            if (jj_3R_20()) return true;
            return false;
        }

        private bool jj_3R_117()
        {
            if (jj_done) return true;
            if (jj_3R_120()) return true;
            return false;
        }

        private bool jj_3R_15()
        {
            if (jj_done) return true;
            if (jj_scan_token(TK_LABEL_AT)) return true;
            Token xsp;
            xsp = jj_scanpos;
            if (jj_3R_28())
            {
                jj_scanpos = xsp;
                if (jj_3R_29()) return true;
            }
            return false;
        }

        private bool jj_3R_116()
        {
            if (jj_done) return true;
            if (jj_scan_token(46)) return true;
            if (jj_3R_20()) return true;
            if (jj_scan_token(47)) return true;
            return false;
        }

        private bool jj_3_2()
        {
            if (jj_done) return true;
            Token xsp;
            xsp = jj_scanpos;
            if (jj_3R_15()) jj_scanpos = xsp;
            if (jj_3R_16()) return true;
            xsp = jj_scanpos;
            if (jj_3R_17()) jj_scanpos = xsp;
            xsp = jj_scanpos;
            if (jj_3R_18()) jj_scanpos = xsp;
            return false;
        }

        private bool jj_3_1()
        {
            if (jj_done) return true;
            if (jj_scan_token(TK_BEGIN)) return true;
            if (jj_3R_13()) return true;
            if (jj_scan_token(35)) return true;
            if (jj_3R_14()) return true;
            return false;
        }

        private bool jj_3R_71()
        {
            if (jj_done) return true;
            if (jj_scan_token(34)) return true;
            Token xsp;
            xsp = jj_scanpos;
            if (jj_3_2()) jj_scanpos = xsp;
            xsp = jj_scanpos;
            if (jj_3_3()) jj_scanpos = xsp;
            xsp = jj_scanpos;
            if (jj_3R_105()) jj_scanpos = xsp;
            if (jj_scan_token(35)) return true;
            return false;
        }

        private bool jj_3R_45()
        {
            if (jj_done) return true;
            if (jj_scan_token(34)) return true;
            return false;
        }

        private bool jj_3R_115()
        {
            if (jj_done) return true;
            if (jj_scan_token(41)) return true;
            if (jj_3R_106()) return true;
            return false;
        }

        private bool jj_3R_114()
        {
            if (jj_done) return true;
            if (jj_scan_token(42)) return true;
            if (jj_3R_106()) return true;
            return false;
        }

        private bool jj_3R_113()
        {
            if (jj_done) return true;
            if (jj_scan_token(NUMBER)) return true;
            return false;
        }

        private bool jj_3R_106()
        {
            if (jj_done) return true;
            Token xsp;
            xsp = jj_scanpos;
            if (jj_3R_113())
            {
                jj_scanpos = xsp;
                if (jj_3R_114())
                {
                    jj_scanpos = xsp;
                    if (jj_3R_115())
                    {
                        jj_scanpos = xsp;
                        if (jj_3R_116())
                        {
                            jj_scanpos = xsp;
                            if (jj_3R_117()) return true;
                        }
                    }
                }
            }
            return false;
        }

        private bool jj_3R_107()
        {
            if (jj_done) return true;
            if (jj_scan_token(65)) return true;
            if (jj_3R_101()) return true;
            return false;
        }

        private bool jj_3R_101()
        {
            if (jj_done) return true;
            if (jj_3R_106()) return true;
            Token xsp;
            xsp = jj_scanpos;
            if (jj_3R_107()) jj_scanpos = xsp;
            return false;
        }

        private bool jj_3R_27()
        {
            if (jj_done) return true;
            if (jj_3R_48()) return true;
            return false;
        }

        private bool jj_3R_26()
        {
            if (jj_done) return true;
            if (jj_3R_47()) return true;
            return false;
        }

        private bool jj_3R_135()
        {
            if (jj_done) return true;
            if (jj_3R_20()) return true;
            return false;
        }

        private bool jj_3R_134()
        {
            if (jj_done) return true;
            if (jj_3R_71()) return true;
            return false;
        }

        private bool jj_3R_111()
        {
            if (jj_done) return true;
            if (jj_scan_token(TK_INTMOD)) return true;
            return false;
        }

        private bool jj_3R_110()
        {
            if (jj_done) return true;
            if (jj_scan_token(TK_INTDIV)) return true;
            return false;
        }

        private bool jj_3R_25()
        {
            if (jj_done) return true;
            if (jj_3R_46()) return true;
            return false;
        }

        private bool jj_3R_109()
        {
            if (jj_done) return true;
            if (jj_scan_token(64)) return true;
            return false;
        }

        private bool jj_3R_150()
        {
            if (jj_done) return true;
            if (jj_scan_token(42)) return true;
            return false;
        }

        private bool jj_3R_108()
        {
            if (jj_done) return true;
            if (jj_scan_token(63)) return true;
            return false;
        }

        private bool jj_3R_149()
        {
            if (jj_done) return true;
            if (jj_scan_token(41)) return true;
            return false;
        }

        private bool jj_3R_24()
        {
            if (jj_done) return true;
            if (jj_3R_45()) return true;
            return false;
        }

        private bool jj_3R_145()
        {
            if (jj_done) return true;
            Token xsp;
            xsp = jj_scanpos;
            if (jj_3R_149())
            {
                jj_scanpos = xsp;
                if (jj_3R_150()) return true;
            }
            return false;
        }

        private bool jj_3R_102()
        {
            if (jj_done) return true;
            Token xsp;
            xsp = jj_scanpos;
            if (jj_3R_108())
            {
                jj_scanpos = xsp;
                if (jj_3R_109())
                {
                    jj_scanpos = xsp;
                    if (jj_3R_110())
                    {
                        jj_scanpos = xsp;
                        if (jj_3R_111()) return true;
                    }
                }
            }
            if (jj_3R_101()) return true;
            return false;
        }

        private bool jj_3R_96()
        {
            if (jj_done) return true;
            if (jj_3R_101()) return true;
            Token xsp;
            while (true)
            {
                xsp = jj_scanpos;
                if (jj_3R_102()) { jj_scanpos = xsp; break; }
            }
            return false;
        }

        private bool jj_3R_148()
        {
            if (jj_done) return true;
            if (jj_scan_token(50)) return true;
            return false;
        }

        private bool jj_3R_14()
        {
            if (jj_done) return true;
            Token xsp;
            while (true)
            {
                xsp = jj_scanpos;
                if (jj_3R_24()) { jj_scanpos = xsp; break; }
            }
            while (true)
            {
                xsp = jj_scanpos;
                if (jj_3R_25()) { jj_scanpos = xsp; break; }
            }
            while (true)
            {
                xsp = jj_scanpos;
                if (jj_3R_26()) { jj_scanpos = xsp; break; }
            }
            while (true)
            {
                xsp = jj_scanpos;
                if (jj_3R_27()) { jj_scanpos = xsp; break; }
            }
            if (jj_scan_token(0)) return true;
            return false;
        }

        private bool jj_3R_147()
        {
            if (jj_done) return true;
            if (jj_scan_token(49)) return true;
            return false;
        }

        private bool jj_3R_141()
        {
            if (jj_done) return true;
            if (jj_scan_token(42)) return true;
            Token xsp;
            xsp = jj_scanpos;
            if (jj_3R_145()) jj_scanpos = xsp;
            return false;
        }

        private bool jj_3R_146()
        {
            if (jj_done) return true;
            if (jj_scan_token(41)) return true;
            return false;
        }

        private bool jj_3R_144()
        {
            if (jj_done) return true;
            Token xsp;
            xsp = jj_scanpos;
            if (jj_3R_146())
            {
                jj_scanpos = xsp;
                if (jj_3R_147())
                {
                    jj_scanpos = xsp;
                    if (jj_3R_148()) return true;
                }
            }
            return false;
        }

        private bool jj_3R_140()
        {
            if (jj_done) return true;
            if (jj_scan_token(41)) return true;
            Token xsp;
            xsp = jj_scanpos;
            if (jj_3R_144()) jj_scanpos = xsp;
            return false;
        }

        private bool jj_3R_139()
        {
            if (jj_done) return true;
            if (jj_scan_token(44)) return true;
            return false;
        }

        private bool jj_3R_138()
        {
            if (jj_done) return true;
            if (jj_scan_token(48)) return true;
            return false;
        }

        private bool jj_3R_133()
        {
            if (jj_done) return true;
            Token xsp;
            xsp = jj_scanpos;
            if (jj_3R_137())
            {
                jj_scanpos = xsp;
                if (jj_3R_138())
                {
                    jj_scanpos = xsp;
                    if (jj_3R_139())
                    {
                        jj_scanpos = xsp;
                        if (jj_3R_140())
                        {
                            jj_scanpos = xsp;
                            if (jj_3R_141()) return true;
                        }
                    }
                }
            }
            return false;
        }

        private bool jj_3R_137()
        {
            if (jj_done) return true;
            if (jj_scan_token(38)) return true;
            return false;
        }

        private bool jj_3R_104()
        {
            if (jj_done) return true;
            if (jj_scan_token(42)) return true;
            return false;
        }

        private bool jj_3R_103()
        {
            if (jj_done) return true;
            if (jj_scan_token(41)) return true;
            return false;
        }

        private bool jj_3R_131()
        {
            if (jj_done) return true;
            Token xsp;
            xsp = jj_scanpos;
            if (jj_3R_133()) jj_scanpos = xsp;
            xsp = jj_scanpos;
            if (jj_3R_134())
            {
                jj_scanpos = xsp;
                if (jj_3R_135()) return true;
            }
            return false;
        }

        private bool jj_3R_97()
        {
            if (jj_done) return true;
            Token xsp;
            xsp = jj_scanpos;
            if (jj_3R_103())
            {
                jj_scanpos = xsp;
                if (jj_3R_104()) return true;
            }
            if (jj_3R_96()) return true;
            return false;
        }

        private bool jj_3R_79()
        {
            if (jj_done) return true;
            if (jj_3R_96()) return true;
            Token xsp;
            while (true)
            {
                xsp = jj_scanpos;
                if (jj_3R_97()) { jj_scanpos = xsp; break; }
            }
            return false;
        }

        private bool jj_3R_93()
        {
            if (jj_done) return true;
            if (jj_3R_71()) return true;
            return false;
        }

        private bool jj_3R_92()
        {
            if (jj_done) return true;
            if (jj_3R_44()) return true;
            return false;
        }

        private bool jj_3R_91()
        {
            if (jj_done) return true;
            if (jj_3R_80()) return true;
            return false;
        }

        private bool jj_3R_90()
        {
            if (jj_done) return true;
            if (jj_3R_79()) return true;
            return false;
        }

        private bool jj_3R_130()
        {
            if (jj_done) return true;
            if (jj_scan_token(TK_WHILE)) return true;
            if (jj_scan_token(46)) return true;
            if (jj_3R_20()) return true;
            if (jj_scan_token(47)) return true;
            if (jj_3R_71()) return true;
            return false;
        }

        private bool jj_3R_89()
        {
            if (jj_done) return true;
            if (jj_scan_token(62)) return true;
            return false;
        }

        private bool jj_3R_88()
        {
            if (jj_done) return true;
            if (jj_scan_token(61)) return true;
            return false;
        }

        private bool jj_3R_87()
        {
            if (jj_done) return true;
            if (jj_scan_token(60)) return true;
            return false;
        }

        private bool jj_3R_86()
        {
            if (jj_done) return true;
            if (jj_scan_token(59)) return true;
            return false;
        }

        private bool jj_3R_85()
        {
            if (jj_done) return true;
            if (jj_scan_token(58)) return true;
            return false;
        }

        private bool jj_3R_84()
        {
            if (jj_done) return true;
            if (jj_scan_token(50)) return true;
            return false;
        }

        private bool jj_3R_83()
        {
            if (jj_done) return true;
            if (jj_scan_token(57)) return true;
            return false;
        }

        private bool jj_3R_82()
        {
            if (jj_done) return true;
            if (jj_scan_token(49)) return true;
            return false;
        }

        private bool jj_3R_78()
        {
            if (jj_done) return true;
            Token xsp;
            xsp = jj_scanpos;
            if (jj_3R_82())
            {
                jj_scanpos = xsp;
                if (jj_3R_83())
                {
                    jj_scanpos = xsp;
                    if (jj_3R_84())
                    {
                        jj_scanpos = xsp;
                        if (jj_3R_85())
                        {
                            jj_scanpos = xsp;
                            if (jj_3R_86())
                            {
                                jj_scanpos = xsp;
                                if (jj_3R_87())
                                {
                                    jj_scanpos = xsp;
                                    if (jj_3R_88())
                                    {
                                        jj_scanpos = xsp;
                                        if (jj_3R_89()) return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            xsp = jj_scanpos;
            if (jj_3R_90())
            {
                jj_scanpos = xsp;
                if (jj_3R_91())
                {
                    jj_scanpos = xsp;
                    if (jj_3R_92())
                    {
                        jj_scanpos = xsp;
                        if (jj_3R_93()) return true;
                    }
                }
            }
            return false;
        }

        private bool jj_3R_77()
        {
            if (jj_done) return true;
            if (jj_3R_80()) return true;
            return false;
        }

        private bool jj_3R_76()
        {
            if (jj_done) return true;
            if (jj_3R_79()) return true;
            return false;
        }

        private bool jj_3R_70()
        {
            if (jj_done) return true;
            Token xsp;
            xsp = jj_scanpos;
            if (jj_3R_76())
            {
                jj_scanpos = xsp;
                if (jj_3R_77()) return true;
            }
            xsp = jj_scanpos;
            if (jj_3R_78()) jj_scanpos = xsp;
            return false;
        }

        private bool jj_3R_129()
        {
            if (jj_done) return true;
            if (jj_scan_token(TK_FOR)) return true;
            if (jj_scan_token(46)) return true;
            if (jj_3R_20()) return true;
            if (jj_scan_token(47)) return true;
            if (jj_3R_71()) return true;
            return false;
        }

        private bool jj_3R_65()
        {
            if (jj_done) return true;
            if (jj_3R_70()) return true;
            return false;
        }

        private bool jj_3R_64()
        {
            if (jj_done) return true;
            if (jj_scan_token(TK_NOT)) return true;
            if (jj_3R_55()) return true;
            return false;
        }

        private bool jj_3R_55()
        {
            if (jj_done) return true;
            Token xsp;
            xsp = jj_scanpos;
            if (jj_3R_64())
            {
                jj_scanpos = xsp;
                if (jj_3R_65()) return true;
            }
            return false;
        }

        private bool jj_3R_143()
        {
            if (jj_done) return true;
            if (jj_scan_token(TK_ELSE)) return true;
            if (jj_3R_71()) return true;
            return false;
        }

        private bool jj_3R_142()
        {
            if (jj_done) return true;
            if (jj_scan_token(TK_ELIF)) return true;
            if (jj_3R_132()) return true;
            return false;
        }

        private bool jj_3R_136()
        {
            if (jj_done) return true;
            Token xsp;
            xsp = jj_scanpos;
            if (jj_3R_142())
            {
                jj_scanpos = xsp;
                if (jj_3R_143()) return true;
            }
            return false;
        }

        private bool jj_3R_56()
        {
            if (jj_done) return true;
            if (jj_scan_token(56)) return true;
            if (jj_3R_39()) return true;
            return false;
        }

        private bool jj_3R_39()
        {
            if (jj_done) return true;
            if (jj_3R_55()) return true;
            Token xsp;
            xsp = jj_scanpos;
            if (jj_3R_56()) jj_scanpos = xsp;
            return false;
        }

        private bool jj_3R_40()
        {
            if (jj_done) return true;
            if (jj_scan_token(54)) return true;
            if (jj_3R_20()) return true;
            return false;
        }

        private bool jj_3R_132()
        {
            if (jj_done) return true;
            if (jj_scan_token(46)) return true;
            if (jj_3R_20()) return true;
            if (jj_scan_token(47)) return true;
            if (jj_3R_71()) return true;
            Token xsp;
            xsp = jj_scanpos;
            if (jj_3R_136()) jj_scanpos = xsp;
            return false;
        }

        private bool jj_3R_20()
        {
            if (jj_done) return true;
            if (jj_3R_39()) return true;
            Token xsp;
            xsp = jj_scanpos;
            if (jj_3R_40()) jj_scanpos = xsp;
            return false;
        }

        private bool jj_3R_128()
        {
            if (jj_done) return true;
            if (jj_scan_token(TK_IF)) return true;
            if (jj_3R_132()) return true;
            return false;
        }

        private bool jj_3R_75()
        {
            if (jj_done) return true;
            if (jj_3R_71()) return true;
            return false;
        }

        private bool jj_3R_74()
        {
            if (jj_done) return true;
            if (jj_3R_80()) return true;
            return false;
        }

        private bool jj_3R_73()
        {
            if (jj_done) return true;
            if (jj_3R_79()) return true;
            return false;
        }

        private bool jj_3R_72()
        {
            if (jj_done) return true;
            if (jj_3R_44()) return true;
            return false;
        }

        private bool jj_3R_69()
        {
            if (jj_done) return true;
            Token xsp;
            xsp = jj_scanpos;
            if (jj_3R_72())
            {
                jj_scanpos = xsp;
                if (jj_3R_73())
                {
                    jj_scanpos = xsp;
                    if (jj_3R_74())
                    {
                        jj_scanpos = xsp;
                        if (jj_3R_75()) return true;
                    }
                }
            }
            return false;
        }

        private bool jj_3R_100()
        {
            if (jj_done) return true;
            if (jj_3R_44()) return true;
            return false;
        }

        private bool jj_3R_99()
        {
            if (jj_done) return true;
            if (jj_scan_token(UNNAMEDVAR)) return true;
            return false;
        }

        private bool jj_3R_98()
        {
            if (jj_done) return true;
            if (jj_scan_token(VAR)) return true;
            return false;
        }

        /////////////// BOILER PLATE CODE ///////////////////////////
        public bool jj_dummy = true;
        private bool jj_done = false;
        /** Generated Token Manager. */
        public as2jTokenManager token_source;
        SimpleCharStream jj_input_stream;
        /** Current token. */
        public Token token;

        /** Next token. */
        public Token jj_nt;
        private int jj_ntk;
        private Token jj_scanpos, jj_lastpos;
        private int jj_la;
        private int jj_gen;
        private bool hasError = false;
        private bool jj_semLA = false;
        private bool jj_lookingAhead = false;

        /** Constructor. */
        public as2j(System.IO.TextReader stream)
        {
            jj_input_stream = new SimpleCharStream(stream, 1, 1);
            token_source = new as2jTokenManager(jj_input_stream);
            token = new Token();
            jj_ntk = -1;
            jj_gen = 0;
            for (int i = 0; i < jj_2_rtns.Length; i++) jj_2_rtns[i] = new JJCalls();
            hasError = false;
            nsDirective = (NameSpace)dProcessor.GetInstance("namespace");
        }

        /** Reinitialise. */
        public void ReInit(System.IO.TextReader stream)
        {
            if (jj_input_stream == null)
            {
                jj_input_stream = new SimpleCharStream(stream, 1, 1);
            }
            else
            {
                jj_input_stream.ReInit(stream, 1, 1);
            }
            if (token_source == null)
            {
                token_source = new as2jTokenManager(jj_input_stream);
            }

            token_source.ReInit(jj_input_stream);
            token = new Token();
            jj_ntk = -1;
            jj_gen = 0;
            hasError = false;
            for (int i = 0; i < jj_2_rtns.Length; i++) jj_2_rtns[i] = new JJCalls();
        }

        /** Constructor with generated Token Manager. */
        public as2j(as2jTokenManager tm)
        {
            token_source = tm;
            token = new Token();
            jj_ntk = -1;
            jj_gen = 0;
            hasError = false;
            for (int i = 0; i < jj_2_rtns.Length; i++) jj_2_rtns[i] = new JJCalls();
        }

        /** Reinitialise. */
        public void ReInit(as2jTokenManager tm)
        {
            token_source = tm;
            token = new Token();
            jj_ntk = -1;
            jj_gen = 0;
            hasError = false;
            for (int i = 0; i < jj_2_rtns.Length; i++) jj_2_rtns[i] = new JJCalls();
        }

        private Token jj_consume_token(int kind)
        {
            Token oldToken;
            if ((oldToken = token).next != null) token = token.next;
            else token = token.next = token_source.GetNextToken();
            jj_ntk = -1;
            if (token.kind == kind)
            {

                jj_gen++;
                if (++jj_gc > 100)
                {
                    jj_gc = 0;
                    for (int i = 0; i < jj_2_rtns.Length; i++)
                    {
                        JJCalls c = jj_2_rtns[i];
                        while (c != null)
                        {
                            if (c.gen < jj_gen) c.first = null;
                            c = c.next;
                        }
                    }
                }
                trace_token(token, "");

                return token;
            }

            token = oldToken;
            jj_kind = kind;
            throw generateParseException();
        }

        private bool jj_scan_token(int kind)
        {
            if (jj_scanpos == jj_lastpos)
            {
                jj_la--;
                if (jj_scanpos.next == null)
                {
                    jj_lastpos = jj_scanpos = jj_scanpos.next = token_source.GetNextToken();
                }
                else
                {
                    jj_lastpos = jj_scanpos = jj_scanpos.next;
                }
            }
            else
            {
                jj_scanpos = jj_scanpos.next;
            }
            if (jj_rescan)
            {
                int i = 0; Token tok = token;
                while (tok != null && tok != jj_scanpos) { i++; tok = tok.next; }
                if (tok != null) jj_add_error_token(kind, i);
            }
            if (jj_scanpos.kind != kind) return true;
            if (jj_la == 0 && jj_scanpos == jj_lastpos) { return jj_done = true; }
            return false;
        }


        /** Get the next Token. */
        public Token getNextToken()
        {
            if (token.next != null) token = token.next;
            else token = token.next = token_source.GetNextToken();
            jj_ntk = -1;
            jj_gen++;
            return token;
        }

        /** Get the specific Token. */
        public Token getToken(int index)
        {
            Token t = jj_lookingAhead ? jj_scanpos : token;
            for (int i = 0; i < index; i++)
            {
                if (t.next != null) t = t.next;
                else t = t.next = token_source.GetNextToken();
            }
            return t;
        }

        private int jj_ntk_f()
        {
            if ((jj_nt = token.next) == null)
                return (jj_ntk = (token.next = token_source.GetNextToken()).kind);
            else
                return (jj_ntk = jj_nt.kind);
        }

        /** Generate ParseException. */
        public ParseException generateParseException()
        {
            hasError = true;
            int[][] exptokseq = new int[0][];
            jj_expentries.Clear();
            bool[] la1tokens = new bool[344];
            if (jj_kind >= 0)
            {
                la1tokens[jj_kind] = true;
                jj_kind = -1;
            }

            for (int i = 0; i < 0; i++)
            {
                if (jj_la1[i] == jj_gen)
                {
                    for (int j = 0; j < 32; j++)
                    {
                        if ((jj_la1_0[i] & (1 << j)) != 0)
                        {
                            la1tokens[j] = true;
                        }
                        if ((jj_la1_1[i] & (1 << j)) != 0)
                        {
                            la1tokens[32 + j] = true;
                        }
                        if ((jj_la1_2[i] & (1 << j)) != 0)
                        {
                            la1tokens[64 + j] = true;
                        }
                        if ((jj_la1_3[i] & (1 << j)) != 0)
                        {
                            la1tokens[96 + j] = true;
                        }
                        if ((jj_la1_4[i] & (1 << j)) != 0)
                        {
                            la1tokens[128 + j] = true;
                        }
                        if ((jj_la1_5[i] & (1 << j)) != 0)
                        {
                            la1tokens[160 + j] = true;
                        }
                        if ((jj_la1_6[i] & (1 << j)) != 0)
                        {
                            la1tokens[192 + j] = true;
                        }
                        if ((jj_la1_7[i] & (1 << j)) != 0)
                        {
                            la1tokens[224 + j] = true;
                        }
                        if ((jj_la1_8[i] & (1 << j)) != 0)
                        {
                            la1tokens[256 + j] = true;
                        }
                        if ((jj_la1_9[i] & (1 << j)) != 0)
                        {
                            la1tokens[288 + j] = true;
                        }
                        if ((jj_la1_10[i] & (1 << j)) != 0)
                        {
                            la1tokens[320 + j] = true;
                        }
                    }
                }
            }
            for (int i = 0; i < 344; i++)
            {
                if (la1tokens[i])
                {
                    jj_expentry = new int[1];
                    jj_expentry[0] = i;
                    jj_expentries.Add(jj_expentry);
                }
            }
            jj_endpos = 0;
            jj_rescan_token();
            jj_add_error_token(0, 0);
            exptokseq = new int[jj_expentries.Count][];
            for (int i = 0; i < jj_expentries.Count; i++)
            {
                exptokseq[i] = jj_expentries[i];
            }

            return new ParseException(token, exptokseq, tokenImage);
        }


        private int[] jj_la1 = new int[142];  // TODO(sreeni): check this out.
        static private int[] jj_la1_0;
        static private int[] jj_la1_1;
        static private int[] jj_la1_2;
        static private int[] jj_la1_3;
        static private int[] jj_la1_4;
        static private int[] jj_la1_5;
        static private int[] jj_la1_6;
        static private int[] jj_la1_7;
        static private int[] jj_la1_8;
        static private int[] jj_la1_9;
        static private int[] jj_la1_10;
        static as2j()
        {
            jj_la1_init_0();
            jj_la1_init_1();
            jj_la1_init_2();
            jj_la1_init_3();
            jj_la1_init_4();
            jj_la1_init_5();
            jj_la1_init_6();
            jj_la1_init_7();
            jj_la1_init_8();
            jj_la1_init_9();
            jj_la1_init_10();
        }
        private static void jj_la1_init_0()
        {
            jj_la1_0 = new int[] { };
        }
        private static void jj_la1_init_1()
        {
            jj_la1_1 = new int[] { };
        }
        private static void jj_la1_init_2()
        {
            jj_la1_2 = new int[] { };
        }
        private static void jj_la1_init_3()
        {
            jj_la1_3 = new int[] { };
        }
        private static void jj_la1_init_4()
        {
            jj_la1_4 = new int[] { };
        }
        private static void jj_la1_init_5()
        {
            jj_la1_5 = new int[] { };
        }
        private static void jj_la1_init_6()
        {
            jj_la1_6 = new int[] { };
        }
        private static void jj_la1_init_7()
        {
            jj_la1_7 = new int[] { };
        }
        private static void jj_la1_init_8()
        {
            jj_la1_8 = new int[] { };
        }
        private static void jj_la1_init_9()
        {
            jj_la1_9 = new int[] { };
        }
        private static void jj_la1_init_10()
        {
            jj_la1_10 = new int[] { };
        }

        private int trace_indent = 0;
        private bool traceEnabled;

        /** Trace enabled. */
        public bool trace_enabled()
        {
            return traceEnabled;
        }

        /** Enable tracing. */
        public void enable_tracing()
        {
            traceEnabled = true;
        }

        /** Disable tracing. */
        public void disable_tracing()
        {
            traceEnabled = false;
        }

        protected void trace_call(string s)
        {
            if (traceEnabled)
            {
                for (int i = 0; i < trace_indent; i++) { System.Console.Out.Write(" "); }
                System.Console.Out.WriteLine("Call:	" + s);
            }
            trace_indent = trace_indent + 2;
        }

        protected void trace_return(string s)
        {
            trace_indent = trace_indent - 2;
            if (traceEnabled)
            {
                for (int i = 0; i < trace_indent; i++) { System.Console.Out.Write(" "); }
                System.Console.Out.WriteLine("Return: " + s);
            }
        }

        protected void trace_scan(Token t1, int t2)
        {
            if (traceEnabled)
            {
                for (int i = 0; i < trace_indent; i++) { System.Console.Out.Write(" "); }
                System.Console.Out.Write("Visited token: <" + tokenImage[t1.kind]);
                if (t1.kind != 0 && !tokenImage[t1.kind].Equals("\"" + t1.image + "\""))
                {
                    System.Console.Out.Write(": \"" + TokenMgrError.AddEscapes(t1.image) + "\"");
                }
                System.Console.Out.WriteLine(" at line " + t1.beginLine + " column " + t1.beginColumn + ">; Expected token: <" + tokenImage[t2] + ">");
            }
        }

        protected void trace_token(Token t, string where)
        {
            if (traceEnabled)
            {
                for (int i = 0; i < trace_indent; i++) { System.Console.Out.Write(" "); }
                System.Console.Out.Write("Consumed token of kind: " + t.kind + " <" + t.image);
                System.Console.Out.WriteLine(" at line " + t.beginLine + " column " + t.beginColumn + ">" + where);
            }
        }

        private int jj_kind = -1;
        private JJCalls[] jj_2_rtns = new JJCalls[142];
        private bool jj_rescan = false;
        private int jj_gc = 0;

        private void jj_save(int index, int xla)
        {
            JJCalls p = jj_2_rtns[index];
            while (p.gen > jj_gen)
            {
                if (p.next == null) { p = p.next = new JJCalls(); break; }
                p = p.next;
            }

            p.gen = jj_gen + xla - jj_la;
            p.first = token;
            p.arg = xla;
        }


        internal partial class JJCalls
        {
            internal int gen;
            internal Token first;
            internal int arg;
            internal JJCalls next;
        }

        private void jj_rescan_token()
        {
            jj_rescan = true;
            for (int i = 0; i < 142; i++)
            {
                JJCalls p = jj_2_rtns[i];

                while (p != null)
                {
                    if (p.gen > jj_gen)
                    {
                        jj_la = p.arg; jj_lastpos = jj_scanpos = p.first;
                        System.Type thisType = this.GetType();
                        System.Reflection.MethodInfo theMethod = thisType.GetMethod("jj_3_" + (i + 1), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        if (theMethod != null)
                            theMethod.Invoke(this, new object[0]);
                    }
                    p = p.next;
                }
            }
            jj_rescan = false;
        }

        private System.Collections.Generic.List<int[]> jj_expentries = new System.Collections.Generic.List<int[]>();
        private int[] jj_expentry;
        private int[] jj_lasttokens = new int[100];
        private int jj_endpos;

        private void jj_add_error_token(int kind, int pos)
        {
            if (pos >= 100)
            {
                return;
            }

            if (pos == jj_endpos + 1)
            {
                jj_lasttokens[jj_endpos++] = kind;
            }
            else if (jj_endpos != 0)
            {
                jj_expentry = new int[jj_endpos];

                for (int i = 0; i < jj_endpos; i++)
                {
                    jj_expentry[i] = jj_lasttokens[i];
                }

                foreach (int[] oldentry in jj_expentries)
                {
                    if (oldentry.Length == jj_expentry.Length)
                    {
                        bool isMatched = true;

                        for (int i = 0; i < jj_expentry.Length; i++)
                        {
                            if (oldentry[i] != jj_expentry[i])
                            {
                                isMatched = false;
                                break;
                            }

                        }
                        if (isMatched)
                        {
                            jj_expentries.Add(jj_expentry);
                            break;
                        }
                    }
                }

                if (pos != 0)
                {
                    jj_lasttokens[(jj_endpos = pos) - 1] = kind;
                }
            }
        }

        /*FUNCI�N A�ADIDA PARA PODER COMPILAR
         LA FUNCI�N ORIGINALK DE JASON EST� EN OTRO FICHERO LLAMADO MAS2J
         */
        public Dictionary<string, object> ASoptions()
        {
            /*Esto est� MAL ,deber�a devolver un diccionario con las opciones*/
            return new Dictionary<string, object>();
        }
    }
}

