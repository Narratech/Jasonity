using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Code.AsSyntax;

public class EnvironmentInfraTier{
    /** the user customisation class for the environment */
    private Environment userEnv;
    private BaseCentralisedMAS masRunner = BaseCentralisedMAS.getRunner();
    private boolean running = true;

    public EnvironmentInfraTier(ClassParameters userEnvArgs, BaseCentralisedMAS masRunner){
        this.masRunner = masRunner;
        if (userEnvArgs != null) {
            try {
                userEnv = (Environment) getClass().getClassLoader().loadClass(userEnvArgs.getClassName()).newInstance();
                userEnv.setEnvironmentInfraTier(this);
                userEnv.init(userEnvArgs.getParametersArray());
            } catch (Exception e) {
                Debug.Log("Error in Centralised MAS environment creation");
                Debug.Log(e);
                throw new JasonityException("The user environment class instantiation '"+userEnvArgs+"' has failed!"+e.getMessage());
            }
        }
    }

    /** returns true if the infrastructure environment is running */
    public boolean IsRunning() {
        return running;
    }

    /** called before the end of MAS execution, it just calls the user environment class stop method. */
    public void Stop() {
        running = false;
        userEnv.Stop();
    }
    public void SetUserEnvironment(Environment env) {
        userEnv = env;
    }
    public Environment GetUserEnvironment() {
        return userEnv;
    }

    /** called by the agent infra arch to perform an action in the environment */
    public void Act(String agName, ActionExec action) {
        if (running) {
            userEnv.scheduleAction(agName, action.getActionTerm(), action);
        }
    }
    /**
     * Sends a message to the given agents notifying them that the environment has changed
     * (called by the user environment). If no agent is informed, the notification is sent
     * to all agents.
     */
    public void informAgsEnvironmentChanged(String[] agents){
        if (agents.length == 0) {
            foreach (CentralisedAgArch ag in masRunner.GetAgs().Values()) {
                ag.getTS().getUserAgArch().wakeUpSense();
            }
        } else {
            foreach (String agName in agents) {
                CentralisedAgArch ag = masRunner.getAg(agName);
                if (ag != null) {
                        ag.WakeUpSense();
                } else {
                    Debug.Log("Error sending message notification: agent " + agName + " does not exist!");
                }
            }
        }
    }

    /**
     * Sends a message to a set of agents notifying them that the environment has changed.
     * The collection has the agents' names.
     * (called by the user environment).
     *
     * @deprecated use the informAgsEnvironmentChanged with String... parameter
     */
    public void InformAgsEnvironmentChanged(Collection<String> agents){
         if (agentsToNotify == null) {
            informAgsEnvironmentChanged();
        } else {
            foreach (String agName in agentsToNotify) {
                CentralisedAgArch ag = masRunner.GetAg(agName);
                if (ag != null) {
                    ag.getTS().GetUserAgArch().WakeUpSense();
                } else {
                    Debug.Log("Error sending message notification: agent " + agName + " does not exist!");
                }
            }
        }
    }

    /** Gets an object with infrastructure runtime services */
    public RuntimeServices getRuntimeServices(){
       return new RuntimeServices(masRunner);
    }




    /** called by the user implementation of the environment when the action was executed */
    public void ActionExecuted(String agName, Structure actTerm, boolean success, Object infraData){
        ActionExec action = (ActionExec)infraData;
        action.SetResult(success);
        CentralisedAgArch ag = masRunner.GetAg(agName);
        if (ag != null) // the agent may was killed
            ag.ActionExecuted(action);
    }

}