using System.Collections.Generic;
using System;
using Assets.Code.AsSyntax;
using UnityEngine;

namespace Assets.Code.AsSyntax.directives
{
    public class DirectiveProcessor {

        private static Dictionary<string, Type> directives = new Dictionary<string, Type>();
        private static Dictionary<string, IDirective> instances = new Dictionary<string, IDirective>();
        private static Dictionary<string, IDirective> singletons = new Dictionary<string, IDirective>();

        public static void RegisterDirective(string id, Type d)
        {
            directives.Add(id, d);
        }

        public static IDirective GetDirective(string id)
        {
            IDirective d;
            singletons.TryGetValue(id, out d);

            if (d != null)
            {
                return d;
            }

            Type c;
            directives.TryGetValue(id, out c);
            try
            {
                d = (IDirective)Activator.CreateInstance(typeof(Type));

                if (d.IsSingleton())
                {
                    singletons[id] = d;
                }
                return d;
            }

            catch (Exception e)
            {
                e.ToString();
            }
            return null;
        }

        static DirectiveProcessor()
        {
           
            RegisterDirective("include", typeof(Include));
            RegisterDirective("register_function", typeof(FunctionRegister));
            RegisterDirective("namespace", typeof(NameSpace));
           
        }

        public IDirective GetInstance(Pred directive)
        {
            return GetInstance(directive.GetFunctor());
        }

        public IDirective GetInstance(string id)
        {
            IDirective d;
            instances.TryGetValue(id, out d);
            if (d != null)
                return d;

            singletons.TryGetValue(id, out d);
            if (d != null)
                return d;

            // create the instance
            Type c;
            directives.TryGetValue(id, out c);

            try
            {
                d = (IDirective)Activator.CreateInstance(c);
                if (d.IsSingleton())
                    singletons.Add(id, d);
                else
                    instances.Add(id, d);
                return d;
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
            return null;
        }
    }
}