using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lamp : MonoBehaviour
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
        return nameLamp + ":" + lightOn.ToString();
    }
}
