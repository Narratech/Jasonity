﻿using UnityEngine;
using System.Collections;
using Assets.Code.Utilities;

namespace Assets.Code.Actions
{
    public class TurnOnLight : IRunnable
    {
        private GameObject lamp;

        public TurnOnLight(GameObject l)
        {
            lamp = l;
        }

        public void Run() 
        {
            Renderer r = lamp.GetComponent<Renderer>();
            r.material.color = Color.yellow;
        }
    }
}