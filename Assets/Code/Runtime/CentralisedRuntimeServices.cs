using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Code.AsSyntax;

public class CentralisedRuntimeServices : RuntimeServices{

    protected BaseCentralisedMAS masRunner;

    public CentralisedRuntimeServices(BaseCentralisedMAS masRunner) {
        this.masRunner = masRunner;
    }

    protected CentralisedAgArch newAgInstance() {
        return new CentralisedAgArch();
    }

    public String createAgent(String agName, String agSource, String agClass, List<String> archClasses, Settings stts, Agent father) {
        Debug.Log("Creating centralised agent " + agName + " from source " + agSource + " (agClass=" + agClass + ", archClass=" + archClasses + ", settings=" + stts);

        if (stts == null)
            stts = new Settings();

        String prefix = null;
        if (father != null && father.GetASLSrc().StartsWith(SourcePath.CRPrefix))
            prefix = SourcePath.CRPrefix + "/";
        agSource = masRunner.GetProject().GetSourcePaths().FixPath(agSource, prefix);

        String nb = "";
            int n = 1;
            while (masRunner.GetAg(agName+nb) != null)
                nb = "_" + (n++);
            agName = agName + nb;

            CentralisedAgArch agArch = newAgInstance();
            agArch.SetAgName(agName);
            agArch.CreateArchs(ap.GetAgArchClasses(), ap.agClass.GetClassName(), ap.getBBClass(), agSource, stts, masRunner);
            agArch.SetEnvInfraTier(masRunner.GetEnvironmentInfraTier());
            agArch.SetControlInfraTier(masRunner.GetControllerInfraTier());
            
            // if debug mode is active, set up new agent to be synchronous and visible for ExecutionControlGUI
            if (masRunner.IsDebug()) {
                stts.SetVerbose(2);
                stts.SetSync(true);
                agArch.GetLogger().SetLevel(Level.FINE);
                agArch.GetTS().GetLogger().SetLevel(Level.FINE);
                agArch.GetTS().GetAg().GetLogger().SetLevel(Level.FINE);
            }

            masRunner.AddAg(agArch);

        Debug.Log("Agent " + agName + " created!");
        return agName;
    }

    public void startAgent(String agName) {
        // create the agent thread
        CentralisedAgArch agArch = masRunner.GetAg(agName);
        Thread agThread = new Thread(agArch);
        agArch.Start();
    }

    public AgArch clone(Agent source, List<String> archClasses, String agName) {
        // create a new infra arch
        CentralisedAgArch agArch = newAgInstance();
        agArch.SetAgName(agName);
        agArch.SetEnvInfraTier(masRunner.GetEnvironmentInfraTier());
        agArch.SetControlInfraTier(masRunner.GetControllerInfraTier());
        masRunner.AddAg(agArch);

        agArch.CreateArchs(archClasses, source, masRunner);

        startAgent(agName);
        return agArch.GetUserAgArch();
    }

    public Set<String> GSetAgentsNames() {
        return masRunner.GetAgs().KeySet();
    }

    public int GetAgentsQty() {
        return masRunner.GetAgs().KeySet().Size();
    }

    public boolean KillAgent(String agName, String byAg) {
        Debug.Log("Killing centralised agent " + agName);
        CentralisedAgArch ag = masRunner.GetAg(agName);
        if (ag != null && ag.GetTS().GetAg().KillAcc(byAg)) {
            ag.StopAg();
            masRunner.DelAg(agName);
            return true;
        }
        return false;
    }

    public void StopMAS() {
        masRunner.Finish();
    }
  
    public void DfRegister(String agName, String service, String type) {
        masRunner.DfRegister(agName, service);
    }
 
    public void DfDeRegister(String agName, String service, String type) {
        masRunner.DfDeRegister(agName, service);
    }
     
    public Collection<String> DfSearch(String service, String type) {
        return masRunner.DfSearch(service);
    }
  
    public void DfSubscribe(String agName, String service, String type) {
        masRunner.dfSubscribe(agName, service);
    }
    

}