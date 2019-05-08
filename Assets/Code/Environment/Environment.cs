using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Code.AsSyntax;
using Assets.Code.Utilities;
using Assets.Code.ReasoningCycle;
using System;

public class Environment : MonoBehaviour {

    private IList<Literal> percepts = new List<Literal>();
    private IDictionary<string,List<Literal>>  agPercepts = new Dictionary<string, List<Literal>>();
    private bool isRunning = true;
    private ISet<string> uptodateAgs = new HashSet<string>();

    // /** the infrastructure tier for environment (Centralised, Saci, ...) */
    private EnvironmentInfraTier environmentInfraTier = null;

    protected ScheduledExecutor executor; // the thread pool used to execute actions

    public void Start(){

    }

    /**
     * Called before the MAS execution with the args informed in
     * .mas2j project, the user environment could override it.
     */
    public void Init(string[] args) {
    }
    
    /**
     * Called just before the end of MAS execution, the user
     * environment could override it.
     */
    public void Stop() {
        isRunning = false;
        //executor.shutdownNow();
    }

    public bool IsRunning() {
        return isRunning;
    }


    public List<Literal> GetPercepts(string agName) {
        // check whether this agent needs the current version of perception
        if (uptodateAgs.Contains(agName)) {
            return null;
        }
        // add agName in the set of updated agents
        uptodateAgs.Add(agName);

        int size = percepts.Count;
        IList<Literal> agl = agPercepts[agName];
        if (agl != null) {
            size += agl.Count;
        }
        List<Literal> p = new List<Literal>(size);

        if (percepts.Count > 0) { // has global perception?
            foreach (Literal l in percepts)
            {
                p.Add(l);
            }
        }
        if (agl != null) { // add agent personal perception
            foreach (Literal l in agl)
            {
                p.Add(l);
            }
        }

        return p;
    }
    /**
     *  Returns a copy of the perception for an agent.
     *
     *  It is the same list returned by getPercepts, but
     *  doesn't consider the last call of the method.
     */
    public IList<Literal> ConsultPercepts(string agName) {
        int size = percepts.Count;
        IList<Literal> agl = agPercepts[agName];
        if (agl != null) {
            size += agl.Count;
        }
        IList<Literal> p = new List<Literal>(size);

        if (percepts.Count > 0) { // has global perception?
            foreach (Literal l in percepts)
            {
                p.Add(l);
            }
        }
        if (agl != null) { // add agent personal perception
            foreach (Literal l in agl)
            {
                p.Add(l);
            }
        }
        return p;
    }
    /** Adds a perception for all agents */
    public void AddPercept(Literal[] perceptions) {
        if (perceptions != null) {
            foreach (Literal per in perceptions) {
                if (!percepts.Contains(per)) {
                    percepts.Add(per);
                }
            }
            uptodateAgs.Clear();
        }
    }
        /** Removes a perception from the common perception list */
    public bool RemovePercept(Literal per) {
        if (per != null) {
            uptodateAgs.Clear();
            return percepts.Remove(per);
        }
        return false;
    }
    /** Removes all percepts from the common perception list that unifies with <i>per</i>.
     *
     *  Example: removePerceptsByUnif(Literal.parseLiteral("position(_)")) will remove
     *  all percepts that unifies "position(_)".
     *
     *  @return the number of removed percepts.
     */
    public int RemovePerceptsByUnif(Literal per) {
        int c = 0;
        if (percepts.Count > 0) { // has global perception?
            IEnumerator<Literal> i = percepts.GetEnumerator();
            while (i.MoveNext()) {
                Literal l = i.Current;
                if (new Unifier().Unifies(l,per)) {

                    percepts.Remove(l);
                    //i.remove(); //Borrar el elemento apuntado por el enumerator
                    c++;
                }
            }
            if (c>0) uptodateAgs.Clear();
        }
        return c;
    }
    /** Clears the list of global percepts */
    public void ClearPercepts() {
        if (percepts.Count > 0) {
            uptodateAgs.Clear();
            percepts.Clear();
        }
    }
    /** Returns true if the list of common percepts contains the perception <i>per</i>. */
    public bool ContainsPercept(Literal per) {
        if (per != null) {
            return percepts.Contains(per);
        }
        return false;
    }
    /** Adds a perception for a specific agent */
    public void AddPercept(string agName, Literal[] per) {
        if (per != null && agName != null) {
            IList<Literal> agl = agPercepts[agName];
            if (agl == null) {
                agl = new List<Literal>();
                agPercepts.Add(agName, (List<Literal>)agl);
            }
            foreach (Literal p in per) {
                if (!agl.Contains(p)) {
                    uptodateAgs.Remove(agName);
                    agl.Add(p);
                }
            }
        }
    }
    /** Removes a perception for an agent */
    public bool RemovePercept(string agName, Literal per) {
        if (per != null && agName != null) {
            IList<Literal> agl = agPercepts[agName];
            if (agl != null) {
                uptodateAgs.Remove(agName);
                return agl.Remove(per);
            }
        }
        return false;
    }
    /** Removes from an agent perception all percepts that unifies with <i>per</i>.
     *  @return the number of removed percepts.
     */
    public int RemovePerceptsByUnif(string agName, Literal per) {
        int c = 0;
        if (per != null && agName != null) {
            IList<Literal> agl = agPercepts[agName];
            if (agl != null) {
                IEnumerator<Literal> i = agl.GetEnumerator();
                while (i.MoveNext()) {
                    Literal l = i.Current;
                    if (new Unifier().Unifies(l,per)) {

                        agl.Remove(l);
                        //i.remove();
                        c++;
                    }
                }
                if (c>0) uptodateAgs.Remove(agName);
            }
        }
        return c;
    }
    public bool ContainsPercept(string agName, Literal per) {
        if (per != null && agName != null) {
            IList agl = (IList)agPercepts[agName];
            if (agl != null) {
                return agl.Contains(per);
            }
        }
        return false;
    }
     /** Clears the list of percepts of a specific agent */
    public void ClearPercepts(string agName) {
        if (agName != null) {
            IList<Literal> agl = agPercepts[agName];
            if (agl != null) {
                uptodateAgs.Remove(agName);
                agl.Clear();
            }
        }
    }
     /** Clears all perception (from common list and individual perceptions) */
    public void ClearAllPercepts() {
        ClearPercepts();
        foreach (string ag in agPercepts.Keys)
            ClearPercepts(ag);
    }

    /**
     * Executes an action on the environment. This method is probably overridden in the user environment class.
     */
    public virtual bool ExecuteAction(string agName, Structure act) {
        Debug.Log("The action "+act+" done by "+agName+" is not implemented in the default environment.");
        return false;
    }

    /**
    * Sets the infrastructure tier of the environment (saci, jade, centralised, ...)
    */
    public void SetEnvironmentInfraTier(EnvironmentInfraTier je) {
        environmentInfraTier = je;
    }

    public EnvironmentInfraTier GetEnvironmentInfraTier() {
        return environmentInfraTier;
    }
        
    public void InformAgsEnvironmentChanged(List<string> agents) {
        if (environmentInfraTier != null) {
            environmentInfraTier.InformAgsEnvironmentChanged(agents);
        }
    }
    
    /**
     * Called by the agent infrastructure to schedule an action to be
     * executed on the environment
     */
    public void ScheduleAction(string agName, Structure action, object infraData) {
        executor.Execute(new CustomRunnable(agName, action, this, environmentInfraTier, infraData));
    }
    private class CustomRunnable : IRunnable{

        string agName;
        Structure action;
        Environment e;
        EnvironmentInfraTier envInfraTier;
        object infraData;

        public CustomRunnable(string agName, Structure action, Environment e, EnvironmentInfraTier envInfraTier, object infraData)
        {
            this.agName = agName;
            this.action = action;
            this.e = e;
            this.envInfraTier = envInfraTier;
            this.infraData = infraData;
        }

        public void Run() {
                try {
                    bool success = e.ExecuteAction(agName, action);
                    envInfraTier.ActionExecuted(agName, action, success, infraData); // send the result of the execution to the agent
                } catch (Exception ie) {
                    if (!(ie.GetType()==typeof(Exception))) {
                        //Debug.Log(Level.WARNING, "act error!",ie);
                    }
                }
            }
    }
}