namespace Assets.Code.Scripts {

    using System;
    using Code.AsSyntax;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : Code.Environment.Environment
    {
    public GameObject lightbulb;
    public GameObject lightOnAgent;
    public GameObject lightOffAgent;

    private Lamp lamp;
    private LightOnAgent lightOnAgentScript;
    private LightOffAgent lightOffAgentScript;


    //Literals
    /*
    public static readonly Literal lightOn = AsSyntax.ParseLiteral("light_on(lamp)");
    public static readonly Literal lightOff = AsSyntax.ParseLiteral("light_off(lamp)");
    public static readonly Literal sleepLightOn = AsSyntax.ParseLiteral("sleep_light_on(lightOnAgent)");
    public static readonly Literal sleepLightOff = AsSyntax.ParseLiteral("sleep_light_off(lightOffAgent)");
    */

    // Start is called before the first frame update
    public override void Start()
    {
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
