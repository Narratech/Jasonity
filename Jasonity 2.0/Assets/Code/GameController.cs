using Assets.Code.BDI;
using Assets.Code.Syntax;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public List<GameObject> objList = new List<GameObject>();
    public static List<GameObject> environment = new List<GameObject>();
    public GameObject lightOnAgent_GO;
    public GameObject lightOffAgent_GO;

    private lightOnAgent lightOnAgent;
    private lightOffAgent lightOffAgent;
    private Agent agLightOn;
    private Agent agLightOff;
    private bool lightOnReasonedLastCycle;
    private float time = 5;
    private float maxTime = 5;

    public TextMesh textMesh;
    public static TextMesh textMeshS;
    bool reasoning = false;
    

    // Start is called before the first frame update
    void Start()
    {
        lightOnAgent = lightOnAgent_GO.GetComponent<lightOnAgent>();
        agLightOn = new Agent(lightOnAgent.agName, lightOnAgent.ASLSource);
        lightOffAgent = lightOffAgent_GO.GetComponent<lightOffAgent>();
        agLightOff = new Agent(lightOffAgent.agName, lightOffAgent.ASLSource);
        lightOnReasonedLastCycle = false;
        foreach(GameObject g in objList)
        {
            environment.Add(g);
        }
        textMeshS = GameObject.Find("Text").GetComponent<TextMesh>();   
    }

    // Update is called once per frame
    void Update()
    {
        if(!reasoning)
        {
            StartCoroutine(Run());
        }
       
        //if (lightOnReasonedLastCycle)
        //{
        //    Reasoner r = agLightOff.GetReasoner();
        //    textMeshS.text = agLightOff.GetName() + ": Voy a percibir mi entorno";
        //    Dictionary<string, string> p = r.Perceive();
        //    Wait();
        //    GameController.textMeshS.text = agLightOff.GetName() + ": Voy a actualizar mi base de creencias";
        //    r.UpdateBeliefs(p);
        //    //Desire d = ag.SelectDesire();
        //    Wait();
        //    textMeshS.text = agLightOff.GetName() + ": Voy a seleccionar un plan";
        //    Plan i = r.SelectPlan();
        //    Wait();
        //    textMeshS.text = agLightOff.GetName() + ": Voy a actuar";
        //    r.Act(i);
        //    lightOnReasonedLastCycle = false;
        //}
        //else
        //{
        //    Reasoner r = agLightOn.GetReasoner();
        //    textMeshS.text = agLightOn.GetName() + ": Voy a percibir mi entorno";
        //    Dictionary<string, string> p = r.Perceive();
        //    Wait();
        //    GameController.textMeshS.text = agLightOn.GetName() + ": Voy a actualizar mi base de creencias";
        //    r.UpdateBeliefs(p);
        //    //Desire d = ag.SelectDesire();
        //    Wait();
        //    textMeshS.text = agLightOn.GetName() + ": Voy a seleccionar un plan";
        //    Plan i = r.SelectPlan();
        //    Wait();
        //    textMeshS.text = agLightOn.GetName() + ": Voy a actuar";
        //    r.Act(i);
        //    lightOnReasonedLastCycle = true;
        //}
    }

    private void Wait()
    {
        while(time < maxTime)
        {
            time += Time.deltaTime;
        }
    }


    private IEnumerator Run()
    {
        if (lightOnReasonedLastCycle && !reasoning)
        {
            reasoning = true;
            Reasoner r = agLightOff.GetReasoner();
            textMeshS.text = agLightOff.GetName() + ": Voy a percibir mi entorno";
            Dictionary<string, string> p = r.Perceive();
            yield return new WaitForSeconds(3);
            textMeshS.text = agLightOff.GetName() + ": Voy a actualizar mi base de creencias";
            r.UpdateBeliefs(p);
            //Desire d = ag.SelectDesire();
            yield return new WaitForSeconds(3);
            textMeshS.text = agLightOff.GetName() + ": Voy a seleccionar un plan";
            Plan i = r.SelectPlan();
            yield return new WaitForSeconds(3);
            textMeshS.text = agLightOff.GetName() + ": Voy a actuar";
            r.Act(i);
            lightOnReasonedLastCycle = false;
            reasoning = false;
        }
        else if(!lightOnReasonedLastCycle && !reasoning)
        {
            reasoning = true;
            Reasoner r = agLightOn.GetReasoner();
            textMeshS.text = agLightOn.GetName() + ": Voy a percibir mi entorno";
            Dictionary<string, string> p = r.Perceive();
            yield return new WaitForSeconds(3);
            textMeshS.text = agLightOn.GetName() + ": Voy a actualizar mi base de creencias";
            r.UpdateBeliefs(p);
            //Desire d = ag.SelectDesire();
            yield return new WaitForSeconds(3);
            textMeshS.text = agLightOn.GetName() + ": Voy a seleccionar un plan";
            Plan i = r.SelectPlan();
            yield return new WaitForSeconds(3);
            textMeshS.text = agLightOn.GetName() + ": Voy a actuar";
            r.Act(i);
            lightOnReasonedLastCycle = true;
            reasoning = false;
        }
    }
}
//IEnumerator RunWithDelay()
//{

//    GameController.textMeshS.text = ag.GetName() + ": Voy a percibir mi entorno";
//    Dictionary<string, string> p = Perceive();
//    yield return new WaitForSeconds(3);
//    GameController.textMeshS.text = ag.GetName() + ": Voy a actualizar mi base de creencias";
//    UpdateBeliefs(p);
//    //Desire d = ag.SelectDesire();
//    yield return new WaitForSeconds(3);
//    GameController.textMeshS.text = ag.GetName() + ": Voy a seleccionar un plan";
//    Plan i = SelectPlan();
//    yield return new WaitForSeconds(3);
//    GameController.textMeshS.text = ag.GetName() + ": Voy a actuar";
//    Act(i);
//    ag.SetReasoning(false);
//    //yield return new WaitForSeconds(3);
//}