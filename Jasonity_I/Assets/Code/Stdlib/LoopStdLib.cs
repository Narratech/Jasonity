using Assets.Code.AsSyntax;
using Assets.Code.BDIAgent;
using Assets.Code.Exceptions;
using Assets.Code.ReasoningCycle;
using BDIManager.Intentions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Assets.Code.AsSyntax.PlanBodyImpl;

/**
Implementation of <b>while</b>.
<p>Syntax:
<pre>
  while ( <i>logical formula</i> ) {
     <i>plan_body</i>
  }
</pre>
</p>
<p>while <i>logical formula</i> holds, the <i>plan_body</i> is executed.</p>
<p>Example:
<pre>
+event : context
  <- ....
     while(vl(X) & X > 10) { // where vl(X) is a belief
       .print("value > 10");
       -+vl(X+1);
     }
     ....
</pre>
The unification resulted from the evaluation of the logical formula is used only inside the loop,
i.e., the unification after the while is the same as before.
</p>
*/

namespace Assets.Code.Stdlib
{
    public class LoopStdLib:InternalAction
    {
        private static InternalAction singleton = null;
        public static InternalAction Create()
        {
            if (singleton == null)
                singleton = new LoopStdLib();
            return singleton;
        }

        override public ITerm[] PrepareArguments(Literal body, Unifier un)
        {
            return body.GetTermsArray();
        }

        override public int GetMinArgs()
        {
            return 2;
        }
        override public int GetMaxArgs()
        {
            return 2;
        }

        override protected void CheckArguments(ITerm[] args)
        {
            base.CheckArguments(args); // check number of arguments
            if ( !(args[0].GetType() == typeof(ILogicalFormula)))
                throw JasonityException.CreateWrongArgument(this,"first argument (test) must be a logical formula.");
            if ( !args[1].IsPlanBody())
                throw JasonityException.CreateWrongArgument(this,"second argument must be a plan body term.");
        }

        public override object Execute(Reasoner reasoner, Unifier un, ITerm[] args)
        {
            IntendedPlan ip = reasoner.GetCircumstance().GetSelectedIntention().Peek();
            IPlanBody whileia = ip.GetCurrentStep();

            // if the IA has a backup unifier, use that (it is an object term)
            if (args.Length == 2)
            {
                // first execution of while
                CheckArguments(args);
                // add backup unifier in the IA
                whileia = new PlanBodyImpl(BodyType.Body_Type.internalAction, (ITerm)whileia.GetBodyTerm().Clone()); // Como uso el Clone de C# lo que clono son object que luego hay que castear...
                whileia.Add(ip.GetCurrentStep().GetBodyNext());
                ((Structure)whileia.GetBodyTerm()).AddTerm(new ObjectTermImpl(un.Clone()));
            }
            else if (args.Length == 3)
            {
                // restore the unifier of previous iterations
                Unifier ubak = (Unifier)((IObjectTerm)args[2]).GetObject();
                un.Clear();
                un.Compose(ubak);
            }
            else
            {
                throw JasonityException.CreateWrongArgumentNb(this);
            }

            ILogicalFormula logExpr = (ILogicalFormula)args[0];
            // perform one iteration of the loop
            IEnumerator<Unifier> iu = logExpr.LogicalConsequence(reasoner.GetAgent(), un);
            if (iu.MoveNext())
            {
                un.Compose(iu.Current);

                // add in the current intention:
                // 1. the body argument and
                // 2. the while internal action after the execution of the body
                //    (to test the loop again)
                IPlanBody whattoadd = (IPlanBody)args[1].Clone();
                whattoadd.Add(whileia); // the add clones whileia
                whattoadd.SetAsBodyTerm(false);
                ip.InsertAsNextStep(whattoadd);
            }
            return true;
        }
    }
}
