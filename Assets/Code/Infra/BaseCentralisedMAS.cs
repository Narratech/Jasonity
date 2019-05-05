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

    public boolean IsDebug() {
        return debug;
    }

    public static BaseCentralisedMAS GetRunner() {
        return runner;
    }

    public RuntimeServicesInfraTier GetRuntimeServices() {
        return new CentralisedRuntimeServices(runner);
    }

    /* 
    public CentralisedExecutionControl getControllerInfraTier() {
        return control;
    }
*/
    public CentralisedEnvironment GetEnvironmentInfraTier() {
        return env;
    }

    public void AddAg(CentralisedAgArch ag) {
        ags.Put(ag.getAgName(), ag);
    }
    public CentralisedAgArch DelAg(String agName) {
        return ags.Remove(agName);
    }

    public CentralisedAgArch GetAg(String agName) {
        return ags.Get(agName);
    }

    public IMap<String,CentralisedAgArch> GetAgs() {
        return ags;
    }
    public int GetNbAgents() {
        return ags.size();
    }

    public void Finish(){
         Application.Quit();
    }
    
}