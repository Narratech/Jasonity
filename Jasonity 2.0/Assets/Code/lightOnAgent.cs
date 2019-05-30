using Assets.Code.BDI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lightOnAgent : MonoBehaviour
{

    public string ASLSource;
    public string agName;
    private Agent agent;

    void Start()
    {
        agent = new Agent(agName, ASLSource);
    }

    public Agent GetAgent()
    {
        return agent;
    }
}
