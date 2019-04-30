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
    */
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
}