using System.Collections.Generic;
using System;
using Assets.Code.AsSyntax;
using Assets.Code.functions;

namespace Assets.Code.Logic.AsSyntax.directives
{
    public class FunctionRegister : DefaultDirective, IDirective {

        private static Dictionary<string, ArithFunctionTerm> functions = new Dictionary<string, ArithFunctionTerm>();

        static FunctionRegister() {
            AddJasonFunction(typeof(Abs));
            AddJasonFunction(typeof(Max));
            AddJasonFunction(typeof(Min));
            AddJasonFunction(typeof(Sum));
            AddJasonFunction(typeof(StdDev));
            AddJasonFunction(typeof(Average));
            AddJasonFunction(typeof(Length));
            AddJasonFunction(typeof(random));
            AddJasonFunction(typeof(Round));
            AddJasonFunction(typeof(Sqrt));
            AddJasonFunction(typeof(pi));
            AddJasonFunction(typeof(e));
            AddJasonFunction(typeof(floor));
            AddJasonFunction(typeof(ceil));
            AddJasonFunction(typeof(log));
            AddJasonFunction(typeof(time));
        }

        private static void AddJasonFunction(Type c)
        {
            try
            {
                ArithFunctionTerm af = c.NewInstance();
                functions[af.GetName()] = af;
            }
            catch (Exception e)
            {
                
            }
        }

        /** add new global function (shared among all agents in the JVM) */
        public static void AddFunction(Type c)
        {
            try
            {
                ArithFunctionTerm af = c.NewInstance();
                string error = FunctionRegister.CheckFunctionName(af.GetName());
                if (error == null)
                    functions[af.GetName()] = af; 
            }
            catch (Exception)
            {
                
            }
        }

        public static String CheckFunctionName(string fName)
        {
            if (functions[fName] != null)
                return "Can not register the function " + fName + "  twice!";
            else if (fName.IndexOf(".") < 0)
                return "The function " + fName + " was not registered! A function must have a '.' in its name.";
            else if (fName.StartsWith("."))
                return "The function " + fName + " was not registered! An user function name can not start with '.'.";
            else
                return null;
        }

        public static ArithFunctionTerm getFunction(string function, int arity)
        {
            ArithFunctionTerm af = functions[function];

            if (af != null && af.CheckArity(arity))
                return af;
            else
                return null;
        }

        public Agent.Agent Process(Pred directive, Agent.Agent outerContent, Agent.Agent innerContent)
        {
            if (outerContent == null)
                return null;
            try
            {
                string id = ((IStringTerm)directive.GetTerm(0)).GetString();
                if (directive.GetArity() == 1)
                {
                    // it is implemented in java
                    //HACER: Funcion AddFunctio de Agent
                    outerContent.AddFunction((Class<ArithFunction>)Class.forName(id));
                }
                else if (directive.GetArity() == 3)
                {
                    // is is implemented in AS
                    int arity = (int)((INumberTerm)directive.GetTerm(1)).Solve();
                    String predicate = ((IStringTerm)directive.GetTerm(2)).GetString();
                    outerContent.AddFunction(id, arity, predicate);
                }
            }
            catch (Exception e)
            {
                
            }
            return null;
        }
    }
}