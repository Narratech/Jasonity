using System.Collections.Generic;
using System;
using Assets.Code.AsSyntax;

namespace Assets.Code.Logic.AsSyntax.directives
{
    public class FunctionRegister : DefaultDirective, IDirective {

        private static Dictionary<string, ArithFunctionTerm> functions = new Dictionary<string, ArithFunctionTerm>();

        //Hacer: Carpeta functions
        static FunctionRegister() {
            AddJasonFunction(Abs);
            AddJasonFunction(Max);
            AddJasonFunction(Min);
            AddJasonFunction(Sum);
            AddJasonFunction(StdDev);
            AddJasonFunction(Average);
            AddJasonFunction(Length);
            AddJasonFunction(Random);
            AddJasonFunction(Round);
            AddJasonFunction(Sqrt);
            AddJasonFunction(pi);
            AddJasonFunction(e);
            AddJasonFunction(floor);
            AddJasonFunction(ceil);
            AddJasonFunction(log);
            AddJasonFunction(time);
        }

        //CAMBIAR: No es con object se usa class
        private static void AddJasonFunction(Class c)
        {
            try
            {
                ArithFunctionTerm af = c.NewInstance();
                functions.Put(af.GetName(), af);
            }
            catch (Exception e)
            {
                
            }
        }

        /** add new global function (shared among all agents in the JVM) */
        //CAMBIAR: No es con object se usa class
        public static void AddFunction(object c)
        {
            try
            {
                ArithFunctionTerm af = c.NewInstance();
                string error = FunctionRegister.CheckFunctionName(af.GetName());
                if (error == null)
                    functions.Put(af.GetName(), af); 
            }
            catch (Exception)
            {
                
            }
        }

        public static String CheckFunctionName(string fName)
        {
            //CAMBIAR: Funcion equivalente get en diccionarios
            if (functions.Get(fName) != null)
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
            ArithFunctionTerm af = functions.Get(function);

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