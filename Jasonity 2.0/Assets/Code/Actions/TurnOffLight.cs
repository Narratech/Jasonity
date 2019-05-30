using UnityEngine;
using System.Collections;
using Assets.Code.Utilities;

namespace Assets.Code.Actions
{
    public class TurnOffLight : IRunnable
    {
        private GameObject lamp;

        public TurnOffLight(GameObject l)
        {
            lamp = l;
        }

        public void Run()
        {
            Renderer r = lamp.GetComponent<Renderer>();
            r.material.color = Color.grey;
        }
    }
}