using System.Collections;
using System.Collections.Generic;
using Assets.Code.Logic;
using UnityEngine;

public class CyclicTerm : MonoBehaviour
{
    private Literal pvl;
    private VarTerm varTerm;

    public CyclicTerm(Literal pvl, VarTerm varTerm)
    {
        this.pvl = pvl;
        this.varTerm = varTerm;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
