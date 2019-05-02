using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Code.AsSyntax;
using Assets.Code.Exceptions;
using Assets.Code.functions;
using Assets.Code.Logic;
using Assets.Code.AsSyntax.directives;
using Assets.Code.parser;
using Assets.Code.Mas2J;
using Assets.Code.ReasoningCycle;
using Assets.Code.Runtime;
using BDIMaAssets.Code.ReasoningCycle;
using BDIManager.Beliefs;
using BDIManager.Desires;
using BDIManager.Intentions;
using System.Reflection;
using Assets.Code.Util;
using Assets.Code.AsSemantics;

/**
 * The agent class has the belief base and the library plan
 */
namespace Assets.Code.BDIAgent
{
    public class Agent
    {
        private BeliefBase bb = null;
        private PlanLibrary pl = null;
        private Reasoner reasoner = null;
        private string aslSource = null;
        //The ones in the source code
        private List<Literal> initialGoals = null;
        private List<Literal> initialBeliefs = null;
        private Dictionary<string, InternalAction> internalActions = null;
        private Dictionary<string, ArithFunction> functions = null;
        private bool hasCustomSelOp = true;
        //private static ScheduledExecutorService scheduler = null; //I don't know how to do this

        public Agent()
        {
            CheckCustomSelectOption();
        }


        public static Agent Create(AgentArchitecture agArch, string agClass, ClassParameters bbPars, string asSrc, Settings stts)  
        {
            try
            {
                //Agent ag = (Agent) Class.forName(agClass).newInstance(); //???
                Agent ag = new Agent();
                Reasoner r = new Reasoner(ag, null, agArch, stts);
                BeliefBase bb = null;
                if (bbPars == null)
                {
                    bb = new BeliefBase();
                } else
                {
                    //bb = (BeliefBase) Class.forName(bbPars.getClassName()).newInstance();
                }

                ag.SetBB(bb);
                ag.InitAg();

                if (bbPars != null)
                {
                    bb.Init(ag, bbPars.GetParametersArray());
                }

                ag.Load(asSrc);
                return ag;
            }
            catch (Exception e)
            {
                throw new JasonityException("as2j: error creating the customised Agent class! - " + agClass, e);
            }
        }

        public void InitAg()
        {
            if (bb == null)
            {
                new BeliefBase();
            }

            if (pl == null)
            {
                pl = new PlanLibrary();
            }

            if (initialGoals == null)
            {
                initialGoals = new List<Literal>();
            }

            if (initialBeliefs == null)
            {
                initialBeliefs = new List<Literal>();
            }

            if (internalActions == null)
            {
                internalActions = new Dictionary<string, InternalAction>();
            }

            //if (! "false".equals(Config.get().getProperty(Config.START_WEB_MI))) MindInspectorWeb.get().registerAg(this);
        }

        public void InitAg(string asSrc)
        {
            InitAg();
            Load(asSrc);
        }

        //Creemos que GetResource es un método de Java que no implementamos nosotros
        internal static object GetResource(string v)
        {
            throw new NotImplementedException();
        }

        public void Load(string asSrc)
        {
            try
            {
                bool parsingOk = true;
                if(asSrc != null && !string.IsNullOrEmpty(asSrc))
                {
                    asSrc = asSrc.Replace("\\\\", "/");

                    if (asSrc.StartsWith(SourcePath.CRPrefix))
                    {
                        ParseAs(Agent.GetResource(asSrc.Substring(SourcePath.CRPrefix.Length)).openStream() , asSrc); //I don't know what this is
                    } else
                    {
                        parsingOk = ParseAs(new StreamReader(asSrc));
                    }
                }

                if (parsingOk)
                {
                    if (GetPL().HasMetaEventPlans())
                    {
                        GetReasoner().AddDesireListener(new Desire(GetReasoner()));
                    }

                    AddInitialBelsFromProjectInBB();
                    AddInitialBelsInBB();
                    AddInitialDesiresFromProjectInBB();
                    AddInitialDesiresInReasoner();
                    FixAgInIAandFunctions(this);
                }

                LoadKQMLPlans();
                AddInitialBelsInBB();
                SetASLSrc(asSrc);
            } catch (Exception e)
            {
                //logger.log(Level.SEVERE, "Error loading code from " + asSrc, e);
                throw new JasonityException("Error loading code from " + asSrc + " ---- " + e);
            }
        }

        public void Load(StreamReader input, string sourceId)
        {
            try {
                ParseAs(input, sourceId);

                if (GetPL().HasMetaEventPlans())
                    GetReasoner().AddDesireListener(new Desire(GetReasoner()));

                AddInitialBelsInBB();
                AddInitialDesiresInReasoner();
                FixAgInIAandFunctions(this); // used to fix agent reference in functions used inside includes
            } catch (Exception e) {
                //e.printStackTrace();
                throw new JasonityException("Error loading plans from stream " + e);
            }
        }

        public void LoadKQMLPlans()
        {
            
            Config c = Config.Get();
            if (c.GetKqmlPlansFile().Equals(Message.kqmlDefaultPlans))
            {
                if (c.GetKqmlFunctor().Equals(Message.kqmlReceivedFunctor))
                {
                    string file = Message.kqmlDefaultPlans.Substring(Message.kqmlDefaultPlans.IndexOf("/"));
                    if (typeof(JasonityException).GetResource(file) != null) {
                        ParseAs(JasonityException.GetResource(file)); //, "kqmlPlans.asl");
                    } else {
                        
                    }
                }
            } else {
                // load from specified file
                try {
                    ParseAs(new StreamReader(c.GetKqmlPlansFile()));
                } catch (Exception e) {
                    
                }
            }
        }

        public void StopAg()
        {
            /*synchronized(bb.getLock()) {
                bb.stop();
            }*/
            
            foreach (InternalAction ia in internalActions.Values)
            {
                try
                {
                    ia.Destroy();
                }
                catch (Exception e)
                {
                   // e.printStackTrace();
                }
            }
        }

        public Agent Clone(AgentArchitecture arch)
        {
            Agent a = new Agent();

            //a.setLogger(arch);
            if (this.GetReasoner().GetSettings().Verbose() >= 0)
            {
                //a.logger.setLevel(this.getTS().getSettings().logLevel());
            }

            synchronized(GetBB().GetLock()) {
                a.bb = bb.Clone();
            }
            a.pl = pl.Clone();
            try
            {
                FixAgInIAandFunctions(a);
            }
            catch (Exception e)
            {
                //e.printStackTrace();
            }
            a.aslSource = aslSource;
            a.internalActions = new Dictionary<string, InternalAction>();
            a.SetReasoner(new Reasoner(a, GetReasoner().GetCircumstance().Clone(), arch, GetReasoner().GetSettings()));
            if (a.GetPL().HasMetaEventPlans())
            {
                a.GetReasoner().AddDesireListener(new Desire(a.GetReasoner()));
            }

            a.InitAg(); //for initDefaultFunctions() and for overridden/custom agent
            return a;
        }

        private void FixAgInIAandFunctions(Agent a)
        {
           
            synchronized (GetPL().getLock()) {
                foreach (Plan p in a.GetPL().GetPlans()) {
                    // search context
                    if (p.GetContext().GetType() == typeof(Literal))
                    {
                        FixAgInIAandFunctions(a, (Literal) p.GetContext());
                    }

                    // search body
                    if (p.GetBody().GetType() == typeof(Literal))
                    {
                        FixAgInIAandFunctions(a, (Literal) p.GetBody());
                    }
                }
            }
        }

        private void FixAgInIAandFunctions(Agent a, Literal l)
        {
            // if l is internal action/function
            if (l.GetType() == typeof(InternalActionLiteral))
            {
                ((InternalActionLiteral) l).SetIA(null); // reset the IA in the literal, the IA there will be updated next getIA call
            }
            if (l.GetType() == typeof(ArithFunctionTerm))
            {
                ((ArithFunctionTerm) l).SetAgent(a);
            }
            if (l.GetType() == typeof(Rule)) {
                ILogicalFormula f = ((Rule)l).GetBody();
                if (f.GetType() == typeof(Literal)) {
                    FixAgInIAandFunctions(a, (Literal) f);
                }
            }
            for (int i=0; i<l.GetArity(); i++) {
                if (l.GetTerm(i).GetType() == typeof(Literal))
                {
                    FixAgInIAandFunctions(a, (Literal)l.GetTerm(i));
                }
            }
        }

        public static ScheduledExecutorService GetScheduler()
        {
            if (scheduler == null)
            {
                int n;
                try
                {
                    string result;
                    Config.Get().TryGetValue(Config.NB_TH_SCH, out result);
                    n = int.Parse(result.ToString());
                }
                catch (Exception e)
                {
                    n = 2;
                }
                scheduler = Executors.NewScheduledThreadPool(n);
            }
            return scheduler;
        }

        public string GetASLSrc()
        {
            return aslSource;
        }

        public void SetASLSrc(string file)
        {
            if (file != null && file.StartsWith("./"))
                file = file.Substring(2);
            aslSource = file;
        }

        public bool ParseAs(TextReader asFile)
        {
            try
            {
                ParseAs((StreamReader)asFile, asFile.GetType().Name);
                //logger.fine("as2j: AgentSpeak program '" + asFile + "' parsed successfully!");
                return true;
            }
            catch (FileNotFoundException e)
            {
                //logger.log(Level.SEVERE, "as2j: the AgentSpeak source file '" + asFile + "' was not found!");
            }
            catch (ParseException e)
            {
                //logger.log(Level.SEVERE, "as2j: parsing error:" + e.getMessage());
            }
            catch (Exception e)
            {
                //logger.log(Level.SEVERE, "as2j: error parsing \"" + asFile + "\"", e);
            }
            return false;
        }

        public void ParseAs(StreamReader asIn, string sourceId)
        {
            as2j parser = new as2j(asIn);
            parser.SetASLSource(sourceId);
            parser.agent(this);
        }

        public InternalAction GetIA(string iaName) {
            if (iaName.ElementAt(0) == '.')
            {
                iaName = "jason.stdlib" + iaName;
            }
            InternalAction objIA = internalActions[iaName];
            if (objIA == null)
            {
                try
                {
                   MethodInfo create = iaName.GetType().GetMethod("create", null);
                   objIA = (InternalAction) create.Invoke(null, null); //???????
                } catch (Exception e)
                {
                    objIA = (InternalAction)iaName.GetType().NewInstance();
                }
                internalActions.Add(iaName, objIA);
            }
            return objIA;
        }

        public void SetIA(string iaName, InternalAction ia)
        {
            internalActions.Add(iaName, ia);
        }

        public void InitDefaultFunctions()
        {
            if (functions == null)
            {
                functions = new Dictionary<string, ArithFunction>();
            }
            //addFunction(Count.class, false);
            AddFunction(typeof(Count), false);
        }
        
        //Class<? extends ArithFunction> c ?? wtf is this
        public void AddFunction(Type c)
        {
            AddFunction(c, true);
        }

        //Class<? extends ArithFunction> c ?? wtf is this
        private void AddFunction(Type c, bool user)
        {
            try
            {
                ArithFunction af = c.NewInstance();
                string error = null;
                if (user)
                {
                    error = FunctionRegister.CheckFunctionName(af.GetName());
                }
                if (error != null)
                {
                    //logger.warning(error);
                }
                else
                {
                    functions.Add(af.GetName(), af);
                }
            }
            catch (Exception e)
            {
                //logger.log(Level.SEVERE, "Error registering function " + c.getName(), e);
            }
        }

        public void AddFunction(string function, int arity, string literal)
        {
            try
            {
                string error = FunctionRegister.CheckFunctionName(function);
                if (error != null)
                {
                    //logger.warning(error);
                }
                else
                {
                    functions.Add(function, new RuleToFunction(literal, arity));
                }
            }
            catch (Exception e)
            {
                //logger.log(Level.SEVERE, "Error registering function " + literal, e);
            }
        }

        public ArithFunction GetFunction(string function, int arity)
        {
            if (functions == null)
            {
                return null;
            }
            ArithFunction af = functions[function];
            if (af == null || !af.CheckArity(arity))
            {
                af = FunctionRegister.GetFunction(function, arity);
            }
            if (af != null && af.CheckArity(arity))
            {
                return af;
            }
            else
            {
                return null;
            }
        }

        public void AddInitialBel(Literal b)
        {
            initialBeliefs.Add(b);
        }
        public List<Literal> GetInitialBels()
        {
            return initialBeliefs;
        }

        public void AddInitialBelsInBB()
        {
            for (int i=initialBeliefs.Count-1; i >=0; i--)
                AddInitBel(initialBeliefs.ElementAt(i));
            initialBeliefs.Clear();
        }

        protected void AddInitialBelsFromProjectInBB()
        {
            string sBels = GetReasoner().GetSettings().GetUserParameter(Settings.INIT_BELS);
            if (sBels != null)
            {
                try
                {
                    foreach (ITerm t in AsSyntax.AsSyntax.ParseList("[" + sBels + "]"))
                    {
                        AddInitBel(((Literal)t).ForceFullLiteralImpl());
                    }
                }
                catch (Exception e)
                {
                    //logger.log(Level.WARNING, "Initial beliefs from project '[" + sBels + "]' is not a list of literals.");
                }
            }
        }

        private void AddInitBel(Literal b)
        {
            if (!b.IsRule() && !b.IsGround())
            {
                b = new Rule(b, Literal.LTrue);
            }
            if (!b.HasSource())
            {
                b.AddAnnot(BeliefBase.TSelf);
            }
            if (b.IsRule())
            {
                GetBB().Add(b);
            } else
            {
                b = (Literal) b.Capply(null); 
                AddBel(b);
            }
        }

        public void AddInitialDesires(Literal g)
        {
            initialGoals.Add(g);
        }

        public List<Literal> GetInitialDesires()
        {
            return initialGoals;
        }

        public void AddInitialDesiresInReasoner()
        {
            foreach (Literal d in initialGoals)
            {
                d.MakeVarsAnnon();
                if (!d.HasSource())
                    d.AddAnnot(BeliefBase.TSelf);
                GetReasoner().GetCircumstance().AddAchvGoal(d, Intention.emptyInt);
            }
            initialGoals.Clear();
        }

        protected void AddInitialDesiresFromProjectInBB()
        {
            string sGoals = GetReasoner().GetSettings().GetUserParameter(Settings.INIT_GOALS);
            if (sGoals != null)
            {
                try
                {
                    foreach (ITerm t in AsSyntax.AsSyntax.ParseList("[" + sGoals + "]"))
                    {
                        Literal g = ((Literal)t).ForceFullLiteralImpl();
                        g.MakeVarsAnnon();
                        if (!g.HasSource())
                        {
                            g.AddAnnot(BeliefBase.TSelf);
                        }
                        GetReasoner().GetCircumstance().AddAchvGoal(g, Intention.emptyInt);
                    }
                }
                catch (Exception e)
                {
                    //logger.log(Level.WARNING, "Initial goals from project '[" + sGoals + "]' is not a list of literals.");
                }
            }
        }

        public void ImportComponents(Agent a)
        {
            if (a != null)
            {
                foreach (Literal b in a.initialBeliefs)
                {
                    AddInitialBel(b);
                    try
                    {
                        FixAgInIAandFunctions(this, b);
                    } catch (Exception e)
                    {
                            //e.printStackTrace();
                    }
                }

                foreach (Literal d in a.initialGoals)
                {
                    AddInitialDesires(d);
                }

                foreach (Plan p in a.GetPL().GetPlans())
                {
                    GetPL().Add(p, false);
                }

                try
                {
                    FixAgInIAandFunctions(this);
                }
                catch (Exception e)
                {
                    //e.printStackTrace();
                }

                if (GetPL().HasMetaEventPlans())
                {
                    GetReasoner().AddDesireListener(new DesireStdlib(GetReasoner()));
                }
            }
        }

        public bool SocAcc(Message m)
        {
            return true;
        }

        public bool KillAcc(string agName)
        {
            return true;
        }

        public Event SelectEvent(Queue<Event> events)
        {
            return events.Dequeue();
        }

        public Option SelectOption(List<Option> options)
        {
            if (options != null && !(options.Count == 0))
            {
                Option o = options.ElementAt(0);
                options.RemoveAt(0);
                return o;
            }
            else
            {
                return null;
            }
        }

        public Intention SelectIntention(Queue<Intention> intentions)
        {
            return intentions.Dequeue();
        }

        public Message SelectMessage(Queue<Message> messages)
        {
            return messages.Dequeue();
        }

        public ExecuteAction SelectAction(List<ExecuteAction> actList)
        {
            //synchronized(actList) {
                IEnumerator<ExecuteAction> i = actList.GetEnumerator();
                while (i.MoveNext())
                {
                    ExecuteAction a = i.Current;
                    if (!a.GetIntention().IsSuspended())
                    {
                        i.Dispose();
                        return a;
                    }
                }
            //}
            return null;
        }

        public void SetReasoner(Reasoner reasoner)
        {
            this.reasoner = reasoner;
            if (reasoner.GetSettings().Verbose() >= 0)
            {
                //logger.setLevel(ts.getSettings().logLevel());
            }
        }

        public Reasoner GetReasoner()
        {
            return reasoner;
        }

        public void SetBB(BeliefBase bb)
        {
            this.bb = bb;
        }

        public BeliefBase GetBB()
        {
            return bb;
        }

        public void SetPL(PlanLibrary pl)
        {
            this.pl = pl;
        }

        public PlanLibrary GetPL()
        {
            return pl;
        }

        public int Buf(List<Literal> percepts)
        {
            if (percepts == null)
            {
                return 0;
            }

            int adds = 0;
            int dels = 0;

            HashSet<StructureWrapperForLiteral> perW = new HashSet<StructureWrapperForLiteral>();
            IEnumerator<Literal> iper = percepts.GetEnumerator();
            while (iper.MoveNext())
            {
                perW.Add(new StructureWrapperForLiteral(iper.Current));
            }

            IEnumerator<Literal> perceptsInBB = GetBB().GetPercepts();
            while (perceptsInBB.MoveNext())
            {
                Literal l = perceptsInBB.Current;
                if (l.SubjectToBUF() && !perW.Remove(new StructureWrapperForLiteral(l)))
                { 
                    dels++;
                    perceptsInBB.Dispose(); 

                    Trigger te = new Trigger(TEOperator.del, TEType.belief, l);
                    if (reasoner.GetCircumstance().HasListener() || pl.HasCandidatePlan(te))
                    {
                        l = AsSyntax.AsSyntax.CreateLiteral(l.GetFunctor(), l.GetTermsArray());
                        l.AddAnnot(BeliefBase.TPercept);
                        te.SetLiteral(l);
                        reasoner.GetCircumstance().AddEvent(new Event(te, Intention.emptyInt));
                    }
                }
            }
            
            foreach (StructureWrapperForLiteral lw in perW)
            {
                try
                {
                    Literal lp = lw.GetLiteral().Copy().ForceFullLiteralImpl();
                    lp.AddAnnot(BeliefBase.TPercept);
                    if (GetBB().Add(lp))
                    {
                        adds++;
                        reasoner.UpdateEvents(new Event(new Trigger(TEOperator.add, TEType.belief, lp), Intention.emptyInt));
                    }
                }
                catch (Exception e)
                {
                    //logger.log(Level.SEVERE, "Error adding percetion " + lw.getLiteral(), e);
                }
            }

            return adds + dels;
        }

        public bool Believes(ILogicalFormula bel, Unifier un)
        {
            try
            {
                IEnumerator<Unifier> iun = bel.LogicalConsequence(this, un);
                if (iun != null && iun.MoveNext())
                {
                    un.Compose(iun.Current);
                    return true;
                }
            }
            catch (Exception e)
            {
                //logger.log(Level.SEVERE, "** Error in method believes(" + bel + "," + un + ").", e);
            }
            return false;
        }

        public Literal FindBel(Literal bel, Unifier un)
        {
            //synchronized(bb.getLock()) {
                IEnumerator<Literal> relB = bb.GetCandidateBeliefs(bel, un);
                if (relB != null)
                {
                    while (relB.MoveNext())
                    {
                        Literal b = relB.Current;

                        // recall that order is important because of annotations!
                        if (!b.IsRule() && un.Unifies(bel, b))
                        {
                            return b;
                        }
                    }
                }
                return null;
            //}
        }

        public List<Literal>[] Brf(Literal beliefToAdd, Literal beliefToDel, Intention i)
        {
            return Brf(beliefToAdd, beliefToDel, i, false);
        }

        public List<Literal>[] Brf(Literal beliefToAdd, Literal beliefToDel, Intention i, bool addEnd) 
        {
            // This class does not implement belief revision! It
            // is supposed that a subclass will do it.
            // It simply add/del the belief.

            int position = 0; 
            if (addEnd)
            {
                position = 1;
            }

            List<Literal>[] result = null;
            //synchronized(bb.getLock())
            //{
                try
                {
                    if (beliefToAdd != null)
                    {
                        //if (logger.isLoggable(Level.FINE)) logger.fine("Doing (add) brf for " + beliefToAdd);

                        if (GetBB().Add(position, beliefToAdd))
                        {
                            result = new List<Literal>[2];
                            result[0] = SingletonList(beliefToAdd);
                            result[1] = new List<Literal>();
                            //if (logger.isLoggable(Level.FINE)) logger.fine("brf added " + beliefToAdd);
                        }
                    }

                    if (beliefToDel != null)
                    {
                        Unifier u = null;
                        try
                        {
                            u = i.Peek().GetUnif(); // get from current intention
                        }
                        catch (Exception e)
                        {
                            u = new Unifier();
                        }

                        //if (logger.isLoggable(Level.FINE)) logger.fine("Doing (del) brf for " + beliefToDel + " in BB=" + believes(beliefToDel, u));

                        bool removed = GetBB().Remove(beliefToDel);
                        if (!removed && !beliefToDel.IsGround())
                        { // then try to unify the parameter with a belief in BB
                            IEnumerator<Literal> il = GetBB().GetCandidateBeliefs(beliefToDel.GetPredicateIndicator());
                            if (il != null)
                            {
                                while (il.MoveNext())
                                {
                                    Literal linBB = il.Current;
                                    if (u.Unifies(linBB, beliefToDel))
                                    {
                                        il.Dispose();
                                        beliefToDel = (Literal)beliefToDel.Capply(u);
                                        removed = true;
                                        break;
                                    }
                                }
                            }
                        }

                        if (removed)
                        {
                            //if (logger.isLoggable(Level.FINE)) logger.fine("Removed:" + beliefToDel);
                            if (result == null)
                            {
                                result = new List<Literal>[2];
                                result[0] = new List<Literal>();
                            }
                            result[1] = SingletonList(beliefToDel);
                        }
                    }
                }
                catch (Exception e)
                {
                    //logger.log(Level.WARNING, "Error at BRF.", e);
                }
            //}
            return result;
        }

        public bool AddBel(Literal bel)
        {
            if (!bel.HasSource())
            {
                bel.AddAnnot(BeliefBase.TSelf);
            }
            List<Literal>[] result = Brf(bel, null, Intention.emptyInt);
            if (result != null && reasoner != null)
            {
                reasoner.UpdateEvents(result, Intention.emptyInt);
                return true;
            } else
            {
                return false;
            }
        }

        public bool DelBel(Literal bel)
        {
            if (!bel.HasSource())
            {
                bel.AddAnnot(BeliefBase.TSelf);
            }
            List<Literal>[] result = Brf(null, bel, Intention.emptyInt);
            if (result != null && reasoner != null)
            {
                reasoner.UpdateEvents(result, Intention.emptyInt);
                return true;
            } else
            {
                return false;
            }
        }

        public void Abolish(Literal bel, Unifier un)
        {
            List<Literal> toDel = new List<Literal>();
            if (un == null)
            {
                un = new Unifier();
            }
            //synchronized(bb.getLock())
            //{
            IEnumerator<Literal> il = GetBB().GetCandidateBeliefs(bel, un);
            if (il != null)
            {
                while (il.MoveNext())
                {
                    Literal inBB = il.Current;
                    if (!inBB.IsRule())
                    {
                        if (un.Clone().UnifiesNoUndo(bel, inBB))
                        {
                            toDel.Add(inBB);
                        }
                    }
                }
            }
            foreach (Literal l in toDel)
            {
                DelBel(l);
            }
            //}
        }

        private void CheckCustomSelectOption()
        {
           /* hasCustomSelOp = false;
            foreach (Method m in this.getClass().getMethods())
            {
                if (!m.getDeclaringClass().equals(Agent.class) && m.getName().equals("selectOption")) {
                hasCustomSelOp = true;
                }
            }*/
        }

        public bool HasCustomSelectOption()
        {
            return hasCustomSelOp;
        }

        public override string ToString()
        {
            return "Agent from " + GetASLSrc();
        }

        internal Option HasCustomSelectOption(object v)
        {
            throw new NotImplementedException();
        }

        internal Event SelectEvent(IEnumerable<Event> enumerable)
        {
            throw new NotImplementedException();
        }
    }
}

