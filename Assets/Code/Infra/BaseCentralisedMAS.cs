using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Code.AsSyntax;

public class BaseCentralisedMAS{

    protected static BaseCentralisedMAS runner        = null;
    protected static String             urlPrefix     = "";
    protected static boolean            readFromJAR   = false;
    protected static boolean            debug         = false;

    protected getEnvironmentInfraTier        env         = null;
    //protected CentralisedExecutionControl   control     = null; //No tiene uso?
    protected IMap<String,CentralisedAgArch> ags         = new ConcurrentHashMap<String,CentralisedAgArch>();

    public boolean isDebug() {
        return debug;
    }

    public static BaseCentralisedMAS getRunner() {
        return runner;
    }

    public RuntimeServicesInfraTier getRuntimeServices() {
        return new CentralisedRuntimeServices(runner);
    }

    /* 
    public CentralisedExecutionControl getControllerInfraTier() {
        return control;
    }
*/
    public CentralisedEnvironment getEnvironmentInfraTier() {
        return env;
    }

    public void addAg(CentralisedAgArch ag) {
        ags.Put(ag.getAgName(), ag);
    }
    public CentralisedAgArch delAg(String agName) {
        return ags.Remove(agName);
    }

    public CentralisedAgArch getAg(String agName) {
        return ags.Get(agName);
    }

    public IMap<String,CentralisedAgArch> getAgs() {
        return ags;
    }
    public int getNbAgents() {
        return ags.size();
    }

    public void finish(){
         Application.Quit();
    }
    
}