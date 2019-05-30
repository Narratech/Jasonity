using UnityEngine;
using System.Collections;

public interface IAction 
{
    void Act(params string[] parameters);
}
