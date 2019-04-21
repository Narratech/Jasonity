using Assets.Code.AsSyntax;
using Assets.Code.BDIAgent;
using Assets.Code.ReasoningCycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**
<p>Internal action: <b><code>.relevant_rules</code></b>.
<p>Description: gets all rules that can be used to prove some literal.
<p>Parameters:<ul>
<li>+ argument (literal): the argument to proof.</li>
<li>- rules (list of rule terms): the list of rules that prove the argument</li>
</ul>
<p>Example:<ul>
<li> <code>.relevant_rules(p(_),LP)</code>: unifies LP with a list of
all rules with head p/1.</li>
</ul>
*/

namespace Assets.Code.Stdlib
{
    public class RelevantRulesStdLib:InternalAction
    {
        override public int GetMinArgs()
        {
            return 2;
        }
        override public int GetMaxArgs()
        {
            return 2;
        }

        override public object Execute(Reasoner ts, Unifier un, ITerm[] args)
        {
        try {
                Literal pattern = (Literal)args[0];
                IListTerm result = new ListTermImpl();
                synchronized(ts.GetAgent().GetBB().GetLock()) {
                    IEnumerator<Literal> i = ts.GetAgent().GetBB().GetCandidateBeliefs(pattern, un);
                    while (i.MoveNext())
                    {
                        Literal l = i.Current;
                        if (l.IsRule())
                        {
                            if (un.Clone().Unifies(pattern, l))
                            {
                                l = l.Copy();
                                l.DelSources();
                                ((Rule)l).SetAsTerm(true);
                                result.Add(l);
                            }
                        }
                    }
                }
                return un.Unifies(args[1], result);
            } catch (Exception e) {
                //ts.GetLogger().Warning("Error in internal action 'get_rules'! "+e);
            }
            return false;
        }
    }
}
