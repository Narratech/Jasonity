namespace Assets.Code.Scripts
{
    using Assets.Code.AsSyntax;
    using Assets.Code.BDIAgent;
    using global::BDIManager.Beliefs;
    using System;
    using UnityEngine;

    public class GameController : Code.Environment.Environment
    { 
        public GameObject lightbulb;
        public GameObject lightOnAgent;
        public GameObject lightOffAgent;

        private Lamp lamp;
        private LightOnAgent lightOnAgentScript;
        private LightOffAgent lightOffAgentScript;


        //literales  
        //public static readonly literal lighton = assyntax.parseliteral("light_on(lamp)");
        //public static readonly literal lightoff = assyntax.parseliteral("light_off(lamp)");
        //public static readonly literal sleeplighton = assyntax.parseliteral("sleep_light_on(lightonagent)");
        //public static readonly literal sleeplightoff = assyntax.parseliteral("sleep_light_off(lightoffagent)");

        public Agent bob; //aqui de alguna manera creo un agente que diga hola

        public BeliefBase bb;
        public PlanLibrary pl;
 
        // Start is called before the first frame update
        public override void Start()
        {
            bob = Agent.Create(new AgentArchitecture(), "", new Runtime.Settings()); //aqui de alguna manera creo un agente que diga hola
            bb = new BeliefBase();
            pl = new PlanLibrary();
            lamp = lightbulb.GetComponent<Lamp>();
            lightOnAgentScript = lightOnAgent.GetComponent<LightOnAgent>();
            lightOffAgentScript = lightOffAgent.GetComponent<LightOffAgent>();
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        /*Checks if the lamp is on*/
        private bool CheckLampOn()
        {
            return true;
        }

        public void ReceiveFinishedCycle(string v, bool breakpoint, int cycle)
        {
            throw new NotImplementedException();
        }
    }
}
