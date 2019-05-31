using UnityEngine;

namespace Assets.Code.Actions
{
    public class TurnOnLight
    {
        public void Run(GameObject lamp)
        {
            Lamp l = lamp.GetComponent<Lamp>();
            Light li = lamp.GetComponent<Light>();
            li.intensity = 1;
            l.SetLightOn();
        }
    }
}