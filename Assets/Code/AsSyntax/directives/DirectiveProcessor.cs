using System.Collections.Generic;
using System;
using Assets.Code.AsSyntax;

namespace Assets.Code.Logic.AsSyntax.directives
{
    public class DirectiveProcessor {

        private static Dictionary<string, Class> directives = new Dictionary<string, Class>();
        private static Dictionary<string, IDirective> instances = new Dictionary<string, IDirective>();
        private static Dictionary<string, IDirective> singletons = new Dictionary<string, IDirective>();

        public static void RegisterDirective(string id, Class d)
        {
            //CAMBIAR: Funcion put para diccionarios
            directives.Put(id, d);
        }

        public static IDirective GetDirective(string id)
        {
            //CAMBIAR: Función get para diccionarios
            IDirective d = singletons.Get(id);

            if (d != null)
            {
                return d;
            }

            //CAmbiar: Funcion get para diccionarios
            Class c = directives.Get(id);
            try
            {
                //CAMBIAR: NewInstance para Object
                d = (IDirective)c.NewInstance();

                if (d.IsSingleton())
                {
                    //CAMBIAR: Funcion put para diccionarios
                    singletons.Put(id, d);
                }
                return d;
            }

            catch (Exception e)
            {
                e.ToString();
            }
            return null;
        }

        //CAMBIAR: Usar object para class ????
        static DirectiveProcessor() {
            RegisterDirective("include", Include);
            RegisterDirective("register_function", FunctionRegister);
            RegisterDirective("namespace", NameSpace);
        }


        public IDirective GetInstance(Pred directive)
        {
        return GetInstance(directive.GetFunctor());
        }

        public IDirective GetInstance(string id)
        {
            IDirective d = instances.Get(id);
            if (d != null)
                return d;

            d = singletons.Get(id);
            if (d != null)
                return d;

            // create the instance
            object c = directives.Get(id);

            try
            {
                d = (IDirective)c.NewInstance();
                if (d.IsSingleton())
                    singletons.Put(id, d);
                else
                    instances.Put(id, d);
                return d;
            }
            catch (Exception e)
            {
                e.ToString();
            }
            return null;
        }
    }
}