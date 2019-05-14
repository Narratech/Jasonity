namespace Assets.Code.Environment
{

    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using Assets.Code.AsSyntax;
    using Assets.Code.Mas2J;
    using System;
    using Assets.Code.Exceptions;
    using BDIMaAssets.Code.ReasoningCycle;
    using Assets.Code.Infra;
    using Assets.Code.Runtime;

    public class EnvironmentInfraTier
    {
        /** the user customisation class for the environment */
        private Environment userEnv;
        private BaseCentralisedMAS masRunner = BaseCentralisedMAS.GetRunner();
        private bool running = true;

        public EnvironmentInfraTier(ClassParameters userEnvArgs, BaseCentralisedMAS masRunner)
        {
            this.masRunner = masRunner;
            if (userEnvArgs != null)
            {
                try
                {
                    userEnv = (Environment)Activator.CreateInstance(typeof(Environment));
                    userEnv.SetEnvironmentInfraTier(this);
                    userEnv.Init(userEnvArgs.GetParametersArray());
                }
                catch (Exception e)
                {
                    Debug.Log("Error in Centralised MAS environment creation");
                    Debug.Log(e);
                    throw new JasonityException("The user environment class instantiation '" + userEnvArgs + "' has failed!" + e.Message);
                }
            }
        }

        /** returns true if the infrastructure environment is running */
        public bool IsRunning()
        {
            return running;
        }

        /** called before the end of MAS execution, it just calls the user environment class stop method. */
        public void Stop()
        {
            running = false;
            userEnv.Stop();
        }
        public void SetUserEnvironment(Environment env)
        {
            userEnv = env;
        }
        public Environment GetUserEnvironment()
        {
            return userEnv;
        }

        /** called by the agent infra arch to perform an action in the environment */
        public void Act(string agName, ExecuteAction action)
        {
            if (running)
            {
                userEnv.ScheduleAction(agName, action.GetActionTerm(), action);
            }
        }
        /**
         * Sends a message to the given agents notifying them that the environment has changed
         * (called by the user environment). If no agent is informed, the notification is sent
         * to all agents.
         */
        public void InformAgsEnvironmentChanged(params string[] agents)
        {
            if (agents.Length == 0)
            {
                foreach (CentralisedAgArch ag in masRunner.GetAgs().Values)
                {
                    ag.GetReasoner().GetUserAgArch().WakeUpSense();
                }
            }
            else
            {
                foreach (string agName in agents)
                {
                    CentralisedAgArch ag = masRunner.GetAg(agName);
                    if (ag != null)
                    {
                        ag.WakeUpSense();
                    }
                    else
                    {
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
         * @deprecated use the informAgsEnvironmentChanged with string... parameter
         */
        public void InformAgsEnvironmentChanged(List<string> agents)
        {
            if (agents == null)
            {
                InformAgsEnvironmentChanged();
            }
            else
            {
                foreach (string agName in agents)
                {
                    CentralisedAgArch ag = masRunner.GetAg(agName);
                    if (ag != null)
                    {
                        ag.GetReasoner().GetUserAgArch().WakeUpSense();
                    }
                    else
                    {
                        Debug.Log("Error sending message notification: agent " + agName + " does not exist!");
                    }
                }
            }
        }

        /** Gets an object with infrastructure runtime services */
        public IRuntimeServices GetRuntimeServices()
        { //Esto es una interfaz, supongo que tenemos que convertirla en una clase
            return new CentralisedRuntimeServices(masRunner);
        }




        /** called by the user implementation of the environment when the action was executed */
        public void ActionExecuted(string agName, Structure actTerm, bool success, object infraData)
        {
            ExecuteAction action = (ExecuteAction)infraData;
            action.SetResult(success);
            CentralisedAgArch ag = masRunner.GetAg(agName);
            if (ag != null) // the agent may was killed
                ag.ActionExecuted(action);
        }
    }
}