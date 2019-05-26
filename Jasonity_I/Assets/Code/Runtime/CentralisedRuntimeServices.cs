using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Code.AsSyntax;
using Assets.Code.Runtime;
using Assets.Code.Infra;
using Assets.Code.BDIAgent;

public class CentralisedRuntimeServices : IRuntimeServices{ 

    protected BaseCentralisedMAS masRunner;

    public CentralisedRuntimeServices(BaseCentralisedMAS masRunner) {
        this.masRunner = masRunner;
    }

    protected CentralisedAgArch NewAgInstance() {
        return new CentralisedAgArch();
    }

    public string CreateAgent(string agName, string agSource, string agClass, List<string> archClasses, Settings stts, Agent father) {
        Debug.Log("Creating centralised agent " + agName + " from source " + agSource + " (agClass=" + agClass + ", archClass=" + archClasses + ", settings=" + stts);

        if (stts == null)
            stts = new Settings();

        string prefix = null;
        if (father != null && father.GetASLSrc().StartsWith(SourcePath.CRPrefix))
            prefix = SourcePath.CRPrefix + "/";
        //agSource = masRunner.GetProject().GetSourcePaths().FixPath(agSource, prefix); //el proyect creo que es algo con unity. Sep, esto es unity

        string nb = "";
            int n = 1;
            while (masRunner.GetAg(agName+nb) != null)
                nb = "_" + (n++);
            agName = agName + nb;

            CentralisedAgArch agArch = NewAgInstance();
            agArch.SetAgName(agName);
            //agArch.CreateArchs(ap.GetAgArchClasses(), ap.agClass.GetClassName(), ap.getBBClass(), agSource, stts, masRunner); //esto creo que no hace falta porque no tenemos cosas personalizadas
            agArch.SetEnvInfraTier(masRunner.GetEnvironmentInfraTier());
            agArch.SetControlInfraTier(masRunner.GetControllerInfraTier()); //Esto esta comentado en masrunner
            
            // if debug mode is active, set up new agent to be synchronous and visible for ExecutionControlGUI
            if (masRunner.IsDebug()) {
                stts.SetVerbose(2);
                stts.SetSync(true);
                //agArch.GetLogger().SetLevel(Level.FINE);
                //agArch.GetTS().GetLogger().SetLevel(Level.FINE);
                //agArch.GetTS().GetAg().GetLogger().SetLevel(Level.FINE);
            }

            masRunner.AddAg(agArch);

        Debug.Log("Agent " + agName + " created!");
        return agName;
    }

    public void StartAgent(string agName) {
        // create the agent thread
        CentralisedAgArch agArch = masRunner.GetAg(agName);
        //Thread agThread = new Thread(agArch); //Yo quitaria los threads
        agArch.ReasoningCycleStarting();
    }

    public AgentArchitecture Clone(Agent source, List<string> archClasses, string agName) {
        // create a new infra arch
        CentralisedAgArch agArch = NewAgInstance();
        agArch.SetAgName(agName);
        agArch.SetEnvInfraTier(masRunner.GetEnvironmentInfraTier());
        agArch.SetControlInfraTier(masRunner.GetControllerInfraTier()); 
        masRunner.AddAg(agArch);

        agArch.CreateArchs(archClasses, source, masRunner);

        StartAgent(agName);
        return agArch.GetUserAgArch();
    }

    public IEnumerable<string> GetAgentsNames() {
        return masRunner.GetAgs().Keys;
    }

    public int GetAgentsQty() {
        return masRunner.GetAgs().Keys.Count;
    }

    public bool KillAgent(string agName, string byAg) {
        Debug.Log("Killing centralised agent " + agName);
        CentralisedAgArch ag = masRunner.GetAg(agName);
        if (ag != null && ag.GetReasoner().GetAgent().KillAcc(byAg)) {
            ag.StopAg();
            masRunner.DelAg(agName);
            return true;
        }
        return false;
    }

    public void StopMAS() {
        masRunner.Finish();
    }
  
    public void DfRegister(string agName, string service, string type) {
        masRunner.DfRegister(agName, service);
    }
 
    public void DfDeRegister(string agName, string service, string type) {
        masRunner.DfDeRegister(agName, service);
    }
     
    public IEnumerable<string> DfSearch(string service, string type) {
        return masRunner.DfSearch(service);
    }
  
    public void DfSubscribe(string agName, string service, string type) {
        masRunner.DfSubscribe(agName, service);
    }

    public string GetNewAgentName(string name)
    {
        throw new System.NotImplementedException();
    }

    public void Clone(Agent agent, object p, string agName)
    {
        throw new System.NotImplementedException();
    }
}