using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lamp : MonoBehaviour, IEnvironmentObject
{
    private bool lightOn;
    public string nameLamp;

    void Start()
    {
        lightOn = true;
    }

    public override string ToString()
    {
        if (string.IsNullOrWhiteSpace(nameLamp))
            nameLamp = "Lampara";
        return nameLamp;
    }

    public string GetPercepts()
    {
        return ToString() + ":" + lightOn.ToString();
    }
}
