using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Code.AsSyntax;

public class Environment : MonoBehaviour{

    private IList<Literal> percepts = new ArrayList<Literal>();
    private IDictionary<string,List<Literal>>  agPercepts = new ConcurrentHashMap<String, List<Literal>>();
    private bool isRunning = true;
    private ISet<string> uptodateAgs = new HashSet<string>();

    //protected ExecutorService executor; // the thread pool used to execute actions

    public void Start(){

    }

    /**
     * Called before the MAS execution with the args informed in
     * .mas2j project, the user environment could override it.
     */
    public void init(string[] args) {
    }
    
    /**
     * Called just before the end of MAS execution, the user
     * environment could override it.
     */
    public void stop() {
        isRunning = false;
        //executor.shutdownNow();
    }

    public boolean isRunning() {
        return isRunning;
    }


    public Collection<Literal> getPercepts(String agName) {
        // check whether this agent needs the current version of perception
        if (uptodateAgs.contains(agName)) {
            return null;
        }
        // add agName in the set of updated agents
        uptodateAgs.add(agName);

        int size = percepts.size();
        IList<Literal> agl = agPercepts.get(agName);
        if (agl != null) {
            size += agl.size();
        }
        Collection<Literal> p = new ArrayList<Literal>(size);

        if (!percepts.isEmpty()) { // has global perception?
            p.addAll(percepts);
        }
        if (agl != null) { // add agent personal perception
            p.addAll(agl);
        }

        return p;
    }
    /**
     *  Returns a copy of the perception for an agent.
     *
     *  It is the same list returned by getPercepts, but
     *  doesn't consider the last call of the method.
     */
    public IList<Literal> consultPercepts(String agName) {
        int size = percepts.size();
        IList<Literal> agl = agPercepts.get(agName);
        if (agl != null) {
            size += agl.size();
        }
        IList<Literal> p = new ArrayList<Literal>(size);

        if (!percepts.isEmpty()) { // has global perception?
                p.addAll(percepts);
        }
        if (agl != null) { // add agent personal perception
                p.addAll(agl);
        }
        return p;
    }
    /** Adds a perception for all agents */
    public void addPercept(Literal[] perceptions) {
        if (perceptions != null) {
            foreach (Literal per in perceptions) {
                if (!percepts.contains(per)) {
                    percepts.add(per);
                }
            }
            uptodateAgs.clear();
        }
    }
        /** Removes a perception from the common perception list */
    public boolean removePercept(Literal per) {
        if (per != null) {
            uptodateAgs.clear();
            return percepts.remove(per);
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
    public int removePerceptsByUnif(Literal per) {
        int c = 0;
        if (! percepts.isEmpty()) { // has global perception?
            Iterator<Literal> i = percepts.iterator();
            while (i.hasNext()) {
                Literal l = i.next();
                if (new Unifier().unifies(l,per)) {
                    i.remove();
                    c++;
                }
            }
            if (c>0) uptodateAgs.clear();
        }
        return c;
    }
    /** Clears the list of global percepts */
    public void clearPercepts() {
        if (!percepts.isEmpty()) {
            uptodateAgs.clear();
            percepts.clear();
        }
    }
    /** Returns true if the list of common percepts contains the perception <i>per</i>. */
    public boolean containsPercept(Literal per) {
        if (per != null) {
            return percepts.contains(per);
        }
        return false;
    }
    /** Adds a perception for a specific agent */
    public void addPercept(String agName, Literal[] per) {
        if (per != null && agName != null) {
            IList<Literal> agl = agPercepts.get(agName);
            if (agl == null) {
                agl = Collections.synchronizedList(new ArrayList<Literal>());
                agPercepts.put( agName, agl);
            }
            foreach (Literal p in per) {
                if (!agl.contains(p)) {
                    uptodateAgs.remove(agName);
                    agl.add(p);
                }
            }
        }
    }
    /** Removes a perception for an agent */
    public boolean removePercept(String agName, Literal per) {
        if (per != null && agName != null) {
            IList<Literal> agl = agPercepts.get(agName);
            if (agl != null) {
                uptodateAgs.remove(agName);
                return agl.remove(per);
            }
        }
        return false;
    }
    /** Removes from an agent perception all percepts that unifies with <i>per</i>.
     *  @return the number of removed percepts.
     */
    public int removePerceptsByUnif(String agName, Literal per) {
        int c = 0;
        if (per != null && agName != null) {
            IList<Literal> agl = agPercepts.get(agName);
            if (agl != null) {
                    Iterator<Literal> i = agl.iterator();
                    while (i.hasNext()) {
                        Literal l = i.next();
                        if (new Unifier().unifies(l,per)) {
                            i.remove();
                            c++;
                        }
                    }
                if (c>0) uptodateAgs.remove(agName);
            }
        }
        return c;
    }
    public boolean containsPercept(String agName, Literal per) {
        if (per != null && agName != null) {
            IList agl = (IList)agPercepts.get(agName);
            if (agl != null) {
                return agl.contains(per);
            }
        }
        return false;
    }
     /** Clears the list of percepts of a specific agent */
    public void clearPercepts(String agName) {
        if (agName != null) {
            IList<Literal> agl = agPercepts.get(agName);
            if (agl != null) {
                uptodateAgs.remove(agName);
                agl.clear();
            }
        }
    }
     /** Clears all perception (from common list and individual perceptions) */
    public void clearAllPercepts() {
        clearPercepts();
        foreach (String ag in agPercepts.keySet())
            clearPercepts(ag);
    }
        /**
     * Executes an action on the environment. This method is probably overridden in the user environment class.
     */
    public boolean executeAction(String agName, Structure act) {
        Debug.Log("The action "+act+" done by "+agName+" is not implemented in the default environment.");
        return false;
    }

        /**
     * Sets the infrastructure tier of the environment (saci, jade, centralised, ...)
     *
    public void setEnvironmentInfraTier(EnvironmentInfraTier je) {
        environmentInfraTier = je;
    }
    public EnvironmentInfraTier getEnvironmentInfraTier() {
        return environmentInfraTier;
    }
    
    
    public void informAgsEnvironmentChanged(Collection<String> agents) {
        if (environmentInfraTier != null) {
            environmentInfraTier.informAgsEnvironmentChanged(agents);
        }
    }
    
    /**
     * Called by the agent infrastructure to schedule an action to be
     * executed on the environment
     
    public void scheduleAction(String agName, Structure action, Object infraData) {
        executor.execute(new Runnable() {
            public void run() {
                try {
                    boolean success = executeAction(agName, action);
                    environmentInfraTier.actionExecuted(agName, action, success, infraData); // send the result of the execution to the agent
                } catch (Exception ie) {
                    if (!(ie instanceof InterruptedException)) {
                        logger.log(Level.WARNING, "act error!",ie);
                    }
                }
            }
        });
    }
    */
}