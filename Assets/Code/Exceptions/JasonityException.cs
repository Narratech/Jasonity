using Assets.Code.AsSyntax;
using Assets.Code.BDIAgent;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Exceptions
{
    class JasonityException : Exception
    {
        public static readonly ITerm WRONG_ARGS = new Atom("wrong_arguments");
        public static readonly ITerm UNKNOWN_ERROR = new Atom("unknown");
        private static readonly ITerm defaultError = new Atom("internal_action");
        private ITerm error = defaultError;
        private IListTerm errorAnnots = null;

        public JasonityException() { }

        public JasonityException(string msg) : base(msg) { }

        public JasonityException(string msg, ITerm error) : base(msg)
        {
            this.error = error;
        }

        public JasonityException(string msg, Exception cause) : base(msg, cause)
        {

        }

        public JasonityException(string msg, ITerm error, Exception cause) : base(msg, cause)
        {
            this.error = error;
        }

        public void AddErrorAnnot(ITerm t)
        {
            if (errorAnnots == null)
            {
                errorAnnots = new ListTermImpl();
            }
            errorAnnots.Append(t);
        }

        public IListTerm GetErrorTerms()
        {
            IListTerm e = CreateBasicErrorAnnots(error, Message);
            if (errorAnnots != null)
            {
                e.Concat(errorAnnots.CloneLT());
            }
            return e;
        }

        public static JasonityException CreateWrongArgumentNb(InternalAction ia)
        {
            string msg;
            if (ia.GetMinArgs() == ia.GetMaxArgs())
            {
                if (ia.GetMinArgs() == 1)
                {
                    msg = " One argument is expected.";
                } else
                {
                    msg = " " + ia.GetMinArgs() + " arguments are expected.";
                }
            } else
            {
                msg = " From " + ia.GetMinArgs() + " to " + ia.GetMaxArgs() + " arguments are expected.";
            }
            return new JasonityException("The internal action '" + ia.GetMaxArgs() + " arguments are expected.");
        }

        public static JasonityException CreateWrongArgument(InternalAction ia, string reason)
        {
            return new JasonityException("Wrong argument for internal action '" + ia.GetType().Name + "': " + reason, WRONG_ARGS);
        }

        public static IListTerm CreateBasicErrorAnnots(string id, string msg)
        {
            return CreateBasicErrorAnnots(new Atom(id), msg);
        }
        public static IListTerm CreateBasicErrorAnnots(ITerm id, string msg)
        {
            return AsSyntax.AsSyntax.CreateList(
                       AsSyntax.AsSyntax.CreateStructure("error", id),
                       AsSyntax.AsSyntax.CreateStructure("error_msg", AsSyntax.AsSyntax.CreateString(msg)));
        }

        public static StreamReader GetResource(string resourceName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            StreamReader textReader = new StreamReader(assembly.GetManifestResourceStream(resourceName));
            //string result = textReader.ReadToEnd();
            textReader.Close();

            //return result;
            return textReader;
        }
    }
}
