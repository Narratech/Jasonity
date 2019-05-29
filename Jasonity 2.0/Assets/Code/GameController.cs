using Assets.Code.BDI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{

    public List<GameObject> environment = new List<GameObject>();
    public GameObject lightOnAgent_GO;
    public GameObject lightOffAgent;

    private lightOnAgent lightOnAgent;
    private Agent agLightOn;
    private Agent agLightOff;

    // Start is called before the first frame update
    void Start()
    {
        lightOnAgent = lightOnAgent_GO.GetComponent<lightOnAgent>();
        agLightOn = lightOnAgent.GetAgent();
    }

    // Update is called once per frame
    void Update()
    {
        if(!agLightOn.IsReasoning())
        {
            agLightOn.Run();
        }
    }
}
