using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Code.AsSyntax;
using Assets.Code.Infra;

public class BaseCentralisedMAS{

    protected static BaseCentralisedMAS runner        = null;
    protected static string             urlPrefix     = "";
    protected static bool            readFromJAR   = false;
    protected static bool            debug         = false;

    protected EnvironmentInfraTier        env         = null;
    //protected CentralisedExecutionControl   control     = null; //No tiene uso?
    protected Dictionary<string,CentralisedAgArch> ags         = new Dictionary<string,CentralisedAgArch>();

    public bool IsDebug() {
        return debug;
    }

    public static BaseCentralisedMAS GetRunner() {
        return runner;
    }

    public CentralisedRuntimeServices GetRuntimeServices() {
        return new CentralisedRuntimeServices(runner);
    }

    /* 
    public CentralisedExecutionControl getControllerInfraTier() {
        return control;
    }
*/
    public EnvironmentInfraTier GetEnvironmentInfraTier() {
        return env;
    }

    public void AddAg(CentralisedAgArch ag) {
        ags.Add(ag.GetAgName(), ag);
    }
    public CentralisedAgArch DelAg(string agName) {
        CentralisedAgArch agArch = ags[agName];
        ags.Remove(agName);
        return agArch;
    }

    public CentralisedAgArch GetAg(string agName) {
        return ags[agName];
    }

    public Dictionary<string,CentralisedAgArch> GetAgs() {
        return ags;
    }
    public int GetNbAgents() {
        return ags.Count;
    }

    public void Finish(){
         Application.Quit();
    }
    
}