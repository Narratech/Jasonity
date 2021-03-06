using System.Collections.Generic;
using System;
using Assets.Code.functions;
using Assets.Code.BDIAgent;

namespace Assets.Code.AsSyntax.directives
{
    public class FunctionRegister : DefaultDirective, IDirective {

        private static Dictionary<string, ArithFunction> functions = new Dictionary<string, ArithFunction>();

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
                ArithFunction af = (ArithFunction)Activator.CreateInstance(typeof(ArithFunction));
                functions.Add(af.GetName(), af);
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
                ArithFunction af = (ArithFunction)Activator.CreateInstance(typeof(ArithFunction));
                string error = CheckFunctionName(af.GetName());
                if (error == null)
                    functions.Add(af.GetName(), af); 
            }
            catch (Exception)
            {
                
            }
        }

        public static string CheckFunctionName(string fName)
        {
            ArithFunction aux;
            functions.TryGetValue(fName, out aux);
            if (aux != null)
                return "Can not register the function " + fName + "  twice!";
            else if (fName.IndexOf(".") < 0)
                return "The function " + fName + " was not registered! A function must have a '.' in its name.";
            else if (fName.StartsWith("."))
                return "The function " + fName + " was not registered! An user function name can not start with '.'.";
            else
                return null;
        }

        public static ArithFunction GetFunction(string function, int arity)
        {
            ArithFunction af;
            functions.TryGetValue(function, out af);

            if (af != null && af.CheckArity(arity))
                return af;
            else
                return null;
        }

        public override Agent Process(Pred directive, Agent outerContent, Agent innerContent)
        {
            if (outerContent == null)
                return null;
            try
            {
                string id = ((IStringTerm)directive.GetTerm(0)).GetString();
                if (directive.GetArity() == 1)
                {
                    // it is implemented in java
                    //outerContent.AddFunction((Class<ArithFunction>)Class.forName(id));
                    outerContent.AddFunction(Type.GetType(id));
                }
                else if (directive.GetArity() == 3)
                {
                    // is is implemented in AS
                    int arity = (int)((INumberTerm)directive.GetTerm(1)).Solve();
                    string predicate = ((IStringTerm)directive.GetTerm(2)).GetString();
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