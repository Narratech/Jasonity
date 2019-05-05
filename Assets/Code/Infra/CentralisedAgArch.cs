using Assets.Code.Mas2J;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Assets.Code.Exceptions;
using System.Threading;
using Assets.Code.ReasoningCycle;
using Assets.Code.Utilities;
using Assets.Code.AsSyntax;
using Assets.Code.BDIManager;
using BDIMaAssets.Code.ReasoningCycle;
using Assets.Code.Runtime;
using Assets.Code.AsSemantics;
using Assets.Code.BDIAgent;


//This provides an agent architecture where each agent has its ouwn thread. 
namespace Assets.Code.Infra
{
    public class CentralisedAgArch : AgentArchitecture, IRunnable
    {
        protected CentralisedEnvironment infraEnv = null; //Esto va a ser un EnvironmentInfraTier o un Environment, no me acuerdo
        private CentralisedExecutionControl infraControl = null;
        private BaseCentralisedMAS masRunner = BaseCentralisedMAS.GetRunner();
        private string agName = "";
        private volatile bool running = true;
        private ConcurrentQueue<Message> mbox = new ConcurrentQueue<Message>();
        //protected Logger logger = Logger.getLogger(CentralisedAgArch.class.getName());
        private static List<IMsgListener> msgListeners = null;
        //private Thread myThread = null; //Sin theads yay
        private object sleepSync = new object();
        private int sleepTime = 50;
        public static readonly int MAX_SLEEP = 1000;
        private object syncMonitor = new object();
        private volatile bool inWaitSyncMonitor = false;
        private RConf conf; //??

        private int cycles = 1;

        private int cyclesSense = 1;
        private int cyclesDeliberate = 1;
        private int cyclesAct = 1;

        public static void AddMsgListener(IMsgListener l)
        {
            if (msgListeners == null)
            {
                msgListeners = new List<IMsgListener>();
            }
            msgListeners.Add(l);
        }
        public static void RemoveMsgListener(IMsgListener l)
        {
            msgListeners.Remove(l);
        }

        /**
         * Creates the user agent architecture, default architecture is
         * jason.architecture.AgArch. The arch will create the agent that creates
         * the TS.
         */
        public void CreateArchs(List<string> agArchClasses, string agClass, ClassParameters bbPars, string asSrc, Settings stts, BaseCentralisedMAS masRunner)
        {
            try
            {
                    this.masRunner = masRunner;
                    Agent.Create(this, agClass, bbPars, asSrc, stts);
                    InsertAgentArchitecture(this);

                    CreateCustomArchs(agArchClasses);
            } catch (Exception e)
            {
                running = false;
                throw new JasonityException("as2j: error creating the agent class! - "+e.Message, e);
            }
        }

        /** init the agent architecture based on another agent */
        public void CreateArchs(List<string> agArchClasses, Agent ag, BaseCentralisedMAS masRunner)
        {
            try
            {
                this.masRunner = masRunner;
                SetReasoner(ag.Clone(this).GetReasoner());
                InsertAgentArchitecture(this);

                CreateCustomArchs(agArchClasses);

            } catch (Exception e)
            {
                running = false;
                throw new JasonityException("as2j: error creating the agent class! - ", e);
            }
        }


        public void StopAg()
        {
            running = false;
            Wake(); // so that it leaves the run loop
            //if (myThread != null)
            //{
                //myThread.Interrupt();
            //}
            GetReasoner().GetAgent().StopAg();
            GetUserAgArch().Stop(); // stops all archs
        }

        public void SetAgName(string name)
        {
            if (name.Equals("self"))
            {
                throw new JasonityException("an agent cannot be named 'self'!");
            }
            if (name.Equals("percept"))
            {
                throw new JasonityException("an agent cannot be named 'percept'!");
            }
            agName = name;
        }
        
        public String GetAgName()
        {
            return agName;
        }

        public AgentArchitecture GetUserAgArch()
        {
            return GetFirstAgentArchitecture();
        }

        public void SetEnvInfraTier(CentralisedEnvironment env)
        {
            infraEnv = env;
        }

        public CentralisedEnvironment GetEnvInfraTier()
        {
            return infraEnv;
        }

        public void SetControlInfraTier(CentralisedExecutionControl pControl)
        {
            infraControl = pControl;
        }

        public CentralisedExecutionControl GetControlInfraTier()
        {
            return infraControl;
        }

        /*public void SetThread(Thread t)
        {
            myThread = t;
            myThread.Name = agName;
        }*/

        /*public void StartThread()
        {
            myThread.Start();
        }*/

        public new bool IsRunning()
        {
            return running;
        }

        protected void Sense()
        {
            Reasoner reasoner = GetReasoner();

            int i = 0;
            do
            {
                reasoner.Sense(); // must run at least once, so that perceive() is called
            } while (running && ++i < cyclesSense && !reasoner.CanSleepSense());
        }

        //int sumDel = 0; int nbDel = 0;
        protected void Deliberate()
        {
            Reasoner reasoner = GetReasoner();
            int i = 0;
            while (running && i++ < cyclesDeliberate && !reasoner.CanSleepDeliberate())
            {
                reasoner.Deliberate();
            }
            //sumDel += i; nbDel++;
            //System.out.println("running del "+(sumDel/nbDel)+"/"+cyclesDeliberate);
        }

        //int sumAct = 0; int nbAct = 0;
        protected void Act()
        {
            Reasoner reasoner = GetReasoner();

            int i = 0;
            int ca = cyclesAct;
            if (ca != 1)
            { // not the default value, limit the value to the number of intentions
                ca = Math.Min(cyclesAct, reasoner.GetCircumstance().GetNbRunningIntentions());
                if (ca == 0)
                    ca = 1;
            }
            while (running && i++ < ca && !reasoner.CanSleepAct())
            {
                reasoner.Act();
            }
            //sumAct += i; nbAct++;
            //System.out.println("running act "+(sumAct/nbAct)+"/"+ca);
        }

        protected void ReasoningCycle()
        {
            Sense();
            Deliberate();
            Act();
        }

        public void Run()
        {
            Reasoner reasoner = GetReasoner();
            while (running)
            {
                if (reasoner.GetSettings().IsSync())
                {
                    WaitSyncSignal();
                    ReasoningCycle();
                    bool isBreakPoint = false;
                    try
                    {
                        isBreakPoint = reasoner.GetCircumstance().GetSelectedOption().GetPlan().HasBreakpoint();
                    }
                    catch (NullReferenceException e)
                    {
                        // no problem, there is no sel opt, no plan ....
                    }
                    InformCycleFinished(isBreakPoint, GetCycleNumber());
                }
                else
                {
                    GetUserAgArch().IncCycleNumber();
                    ReasoningCycle();
                    if (reasoner.CanSleep())
                        Sleep();
                }
            }
        }

        public void Sleep()
        {
            try
            {
                if (!GetReasoner().GetSettings().IsSync())
                {
                    //logger.fine("Entering in sleep mode....");
                    //synchronized(sleepSync) {
                        sleepSync.Wait(sleepTime); // wait for messages //Esto entiendo queno hace falta
                        if (sleepTime < MAX_SLEEP)
                            sleepTime += 100;
                    //}
                }
            }
            /*catch (ThreadInterruptedException e)
            {
            }*/
            catch (Exception e)
            {
                
            }
        }

        public override void Wake()
        {
            //synchronized(sleepSync) 
            //{
                sleepTime = 50;
                sleepSync.NotifyAll(); // notify sleep method //Entiendo que esto tampoco
            //}
        }

        public override void WakeUpSense()
        {
            Wake();
        }

        public override void WakeUpDeliberate()
        {
            Wake();
        }

        public override void WakeUpAct()
        {
            Wake();
        }

        public override List<Literal> Perceive()
        {
            base.Perceive();
            if (infraEnv == null) return null;
            List<Literal> percepts = infraEnv.GetUserEnvironment().GetPercepts(GetAgName());
            return percepts;
        }

    // this is used by the .send internal action in stdlib
        public void SendMsg(Message m)
        {
            if (m.GetSender() == null)
            {
                m.SetSender(GetAgName());
            }
            CentralisedAgArch rec = masRunner.GetAg(m.GetReceiver());

            if (rec == null)
            {
                if (IsRunning())
                    throw new ReceiverNotFoundException("Receiver '" + m.GetReceiver() + "' does not exist! Could not send " + m);
                else
                    return;
            }
            rec.ReceiveMsg(m.Clone()); // send a cloned message

            // notify listeners
            if (msgListeners != null)
                foreach (IMsgListener l in msgListeners)
                    l.MsgSent(m);
        }

        public void ReceiveMsg(Message m)
        {
            mbox.Enqueue(m);
            WakeUpSense();
        }

        public new void Broadcast(Message m)
        {
            foreach (string agName in masRunner.GetAgs().Key)
            {
                if (!agName.Equals(GetAgName()))
                {
                    m.SetReceiver(agName);
                    SendMsg(m);
                }
            }
        }

        // Default procedure for checking messages, move message from local mbox to C.mbox
        public new void CheckMail()
        {
            Circumstance C = GetReasoner().GetCircumstance();
            Message im;
            mbox.TryDequeue(out im);
            while (im != null)
            {
                C.AddMsg(im);
                im = mbox.Poll(); //Mirar que hace el de java
            }
        }

        public List<Message> GetMBox()
        {
            return mbox.ToList();
        }

        /** called by the TS to ask the execution of an action in the environment */
        public override void Act(ExecuteAction action)
        {
            //if (logger.isLoggable(Level.FINE)) logger.fine("doing: " + action.getActionTerm());
            if (IsRunning())
            {
                if (infraEnv != null)
                {
                    infraEnv.Act(GetAgName(), action);
                }
                else
                {
                    action.SetResult(false);
                    action.SetFailureReason(new Atom("noenv"), "no environment configured!");
                    ActionExecuted(action);
                }
            }
        }

        public new bool CanSleep()
        {
            return mbox.IsEmpty && IsRunning();
        }


        /**
         * waits for a signal to continue the execution (used in synchronised
         * execution mode)
         */
        private void WaitSyncSignal()
        {
            try
            {
                //synchronized(syncMonitor) 
                //{
                    inWaitSyncMonitor = true;
                    syncMonitor.Wait();//esto creo que no hace falta
                    inWaitSyncMonitor = false;
                //}
            }
            /*catch (ThreadInterruptedException e)
            {
            }*/
            catch (Exception e)
            {

            }
        }

        /**
         * inform this agent that it can continue, if it is in sync mode and
         * waiting a signal
         */
        public void ReceiveSyncSignal() //Esto creo que no hace falta yay
        {
            try
            {
                //synchronized(syncMonitor) 
                //{
                    while (!inWaitSyncMonitor && IsRunning())
                    {
                        // waits the agent to enter in waitSyncSignal
                        syncMonitor.Wait(50);
                    }
                    syncMonitor.NotifyAll();
                //}
            }
            catch (ThreadInterruptedException e)
            {
            }
            catch (Exception e)
            {
                
            }
        }

        /**
         *  Informs the infrastructure tier controller that the agent
         *  has finished its reasoning cycle (used in sync mode).
         *
         *  <p><i>breakpoint</i> is true in case the agent selected one plan
         *  with the "breakpoint" annotation.
         */
        public void InformCycleFinished(bool breakpoint, int cycle)
        {
            infraControl.ReceiveFinishedCycle(GetAgName(), breakpoint, cycle);
        }

        public new RuntimeServices GetRuntimeServices()
        {
            return masRunner.GetRuntimeServices();
        }
        
        public void SetConf(RConf conf)
        {
            this.conf = conf;
        }

        public RConf GetConf()
        {
            return conf;
        }

        public int GetCycles()
        {
            return cycles;
        }

        public void SetCycles(int cycles)
        {
            this.cycles = cycles;
        }

        public int GetCyclesSense()
        {
            return cyclesSense;
        }

        public void SetCyclesSense(int cyclesSense)
        {
            this.cyclesSense = cyclesSense;
        }

        public int GetCyclesDeliberate()
        {
            return cyclesDeliberate;
        }

        public void SetCyclesDeliberate(int cyclesDeliberate)
        {
            this.cyclesDeliberate = cyclesDeliberate;
        }

        public int GetCyclesAct()
        {
            return cyclesAct;
        }

        public void SetCyclesAct(int cyclesAct)
        {
            this.cyclesAct = cyclesAct;
        }
    }
}
