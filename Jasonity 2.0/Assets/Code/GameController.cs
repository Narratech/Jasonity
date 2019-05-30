using Assets.Code.BDI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public List<GameObject> environment = new List<GameObject>();
    public GameObject lightOnAgent_GO;
    public GameObject lightOffAgent_GO;

    private lightOnAgent lightOnAgent;
    private lightOffAgent lightOffAgent;
    private Agent agLightOn;
    private Agent agLightOff;
    private bool lightOnReasonedLastCycle;
    private float time = 5;
    private float maxTime = 5;

    // Start is called before the first frame update
    void Start()
    {
        lightOnAgent = lightOnAgent_GO.GetComponent<lightOnAgent>();
        agLightOn = lightOnAgent.GetAgent();
        lightOffAgent = lightOffAgent_GO.GetComponent<lightOffAgent>();
        agLightOff = lightOffAgent.GetAgent();
        lightOnReasonedLastCycle = false;
    }
    
    // Update is called once per frame
    void Update()
    {
        if (time >= maxTime)
        {
            if (lightOnReasonedLastCycle)
            {
                Debug.Log("---------------------------");
                Debug.Log("Agente luz encendida espera");
                agLightOff.Run();
                lightOnReasonedLastCycle = false;
            }
            else
            {
                Debug.Log("---------------------------");
                Debug.Log("Agente luz apagada espera");
                agLightOn.Run();
                lightOnReasonedLastCycle = true;
            }
            time = 0;
        }
        time += Time.deltaTime;
    }
}
