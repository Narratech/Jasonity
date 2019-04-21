using Assets.Code.AsSemantics;
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

/**
  <p>Internal action: <b><code>.send</code></b>.
  <p>Description: sends a message to an agent.
  <p>Parameters:<ul>
  <li>+ receiver (atom, string, or list): the receiver of the
  message. It is the unique name of the agent that will receive the
  message (or list of names).<br/>
  <li>+ ilf (atom): the illocutionary force of the message (tell,
  achieve, ...).<br/>
  <li>+ message (literal): the content of the message.<br/>
  <li><i>+ answer</i> (any term [optional]): the answer of an ask
  message (for performatives askOne, askAll, and askHow).<br/>
  <li><i>+ timeout</i> (number [optional]): timeout (in milliseconds)
  when waiting for an ask answer.<br/>
  </ul>
  <p>Messages with an <b>ask</b> illocutionary force can optionally have
  arguments 3 and 4. In case they are given, <code>.send</code> suspends the
  intention until an answer is received and unified with <code>arg[3]</code>,
  or the message request times out as specified by
  <code>arg[4]</code>. Otherwise, the intention is not suspended and the
  answer (which is a tell message) produces a belief addition event as usual.
  <p>Examples (suppose that agent <code>jomi</code> is sending the
  messages):<ul>
  <li> <code>.send(rafael,tell,value(10))</code>: sends <code>value(10)</code>
  to the agent named <code>rafael</code>. The literal
  <code>value(10)[source(jomi)]</code> will be added as a belief in
  <code>rafael</code>'s belief base.</li>
  <li> <code>.send(rafael,achieve,go(10,30)</code>: sends
  <code>go(10,30)</code> to the agent named <code>rafael</code>. When
  <code>rafael</code> receives this message, an event
  <code>&lt;+!go(10,30)[source(jomi)],T&gt;</code> will be added in
  <code>rafael</code>'s event queue.</li>
  <li> <code>.send(rafael,askOne,value(beer,X))</code>: sends
  <code>value(beer,X)</code> to the agent named rafael. This askOne is an
  asynchronous ask since it
  does not suspend jomi's intention. If rafael has, for instance, the literal
  <code>value(beer,2)</code>
  in its belief base, this belief is automatically sent back to jomi. Otherwise an event
  like <code>+?value(beer,X)[source(self)]</code> is generated in rafael's side
  and the result of this query is then sent to jomi. In the jomi's side, the rafael's answer
  is added in the jomi's belief base and an event like
  <code>+value(beer,10)[source(rafael)]</code> is generated.</li>
  <li> <code>.send(rafael,askOne,value(beer,X),A)</code>: sends
  <code>value(beer,X)</code> to the agent named <code>rafael</code>. This askOne
  is a synchronous ask, it suspends <code>jomi</code>'s intention until
  <code>rafael</code>'s
  answer is received. The answer (something like <code>value(beer,10)</code>)
  unifies with <code>A</code>.</li>
  <li> <code>.send(rafael,askOne,value(beer,X),A,2000)</code>: as in the
  previous example, but agent <code>jomi</code> waits for 2 seconds. If no
  message is received by then, <code>A</code> unifies with
  <code>timeout</code>.</li>
  </ul>
*/

namespace Assets.Code.Stdlib
{
    public class SendStdLib:InternalAction
    {
        override public bool CanBeUsedInContext()
        {
            return false;
        }

        private bool lastSendWasSynAsk = false;

        override public int GetMinArgs()
        {
            return 3;
        }
        override public int GetMaxArgs()
        {
            return 5;
        }

        override protected void CheckArguments(ITerm[] args)
        {
            base.CheckArguments(args); // check number of arguments
            if (!args[0].IsAtom() && !args[0].IsList() && !args[0].IsString())
                throw JasonityException.CreateWrongArgument(this,"TO parameter ('"+args[0]+"') must be an atom, a string or a list of receivers!");

            if (! args[1].IsAtom())
                throw JasonityException.CreateWrongArgument(this,"illocutionary force parameter ('"+args[1]+"') must be an atom!");
        }

        public override object Execute(Reasoner ts, Unifier un, ITerm[] args)
        {
            CheckArguments(args);
            ITerm to = args[0];
            ITerm ilf = args[1];
            ITerm pcnt = args[2];

            // create a message to be sent
            Message m = new Message(ilf.ToString(), ts.GetUserAgArch().GetAgentName(), null, pcnt);

            // async ask has a fourth argument and should suspend the intention
            lastSendWasSynAsk = m.IsAsk() && args.Length > 3;
            if (lastSendWasSynAsk)
            {
                m.SetSyncAskMsgID();
                ts.GetCircumstance().AddPendingIntention(m.GetMsgID(), ts.GetCircumstance().GetSelectedIntention());
            }

            // (un)tell or unknown performative with 4 args is a reply to
            if ((m.IsTell() || m.IsUntell() || !m.IsKnownPerformative()) && args.Length > 3)
            {
                ITerm mid = args[3];
                if (!mid.IsAtom())
                {
                    throw new JasonityException("The Message ID ('" + mid + "') parameter of the internal action 'send' is not an atom!");
                }
                m.SetInReplyTo(mid.ToString());
            }

            // send the message
            if (to.IsList())
            {
                foreach (ITerm t in (IListTerm)to)
                {
                    DelegateSendToArch(t, ts, m);
                }
            }
            else
            {
                DelegateSendToArch(to, ts, m);
            }

            if (lastSendWasSynAsk && args.Length == 5)
            {
                // get the timeout deadline
                ITerm tto = args[4];
                if (tto.IsNumeric())
                {
                //    Agent.GetScheduler().schedule(new Runnable()
                //    {
                //        public void run()
                //        {
                //            // if the intention is still in PI, brings it back to C.I with the timeout
                //            Intention intention = ts.GetCircumstance().RemovePendingIntention(m.GetMsgId());
                //            if (intention != null)
                //            {
                //                // unify "timeout" with the fourth parameter of .send
                //                Structure send = (Structure)intention.Peek().RemoveCurrentStep();
                //                ITerm timeoutAns = null;
                //                if (to.IsList())
                //                {
                //                    VarTerm answers = new VarTerm("AnsList___" + m.GetMsgId());
                //                    Unifier un = intention.Peek().GetUnif();
                //                    timeoutAns = un.Get(answers);
                //                    if (timeoutAns == null)
                //                        timeoutAns = new ListTermImpl();
                //                }
                //                else
                //                {
                //                    timeoutAns = new Atom("timeout");
                //                }
                //                intention.Peek().GetUnif().Unifies(send.GetTerm(3), timeoutAns);
                //                // add the intention back in C.I
                //                ts.GetCircumstance().ResumeIntention(intention);
                //                ts.GetUserAgArch().WakeUpAct();
                //            }
                //        }
                //}, (long)((NumberTerm)tto).Solve(), TimeUnit.MILLISECONDS);
                }
                else
                {
                  throw new JasonityException("The 5th parameter of send must be a number (timeout) and not '" + tto + "'!");
                }
            }

            return true;
        }

        private void DelegateSendToArch(ITerm to, Reasoner ts, Message m)
        {
            if (!to.IsAtom() && !to.IsString())
                throw new JasonityException("The TO parameter ('"+to+"') of the internal action 'send' is not an atom!");

            string rec = null;
            if (to.IsString())
                rec = ((IStringTerm) to).GetString();
            else if (to.IsAtom())
        	    rec = ((Atom) to).GetFunctor(); // remove annotations 
    	    else
                rec = to.ToString();
            if (rec.Equals("self"))
                rec = ts.GetUserAgArch().GetAgentName();
            m.SetReceiver(rec);
            ts.GetUserAgArch().SendMessage(m);
        }

        override public bool SuspendIntention()
        {
            return lastSendWasSynAsk;
        }
    }
}
