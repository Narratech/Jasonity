using UnityEngine;

namespace Assets.Code.Actions
{
    public class TurnOffLight
    {
        public void Run(GameObject lamp)
        {
            Lamp l = lamp.GetComponent<Lamp>();
            Light li = lamp.GetComponent<Light>();
            li.intensity = 0;
            l.SetLightOff();
        }
    }
}