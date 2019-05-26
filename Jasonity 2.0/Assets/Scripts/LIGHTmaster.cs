using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LIGHTmaster : MonoBehaviour
{
    public GameObject light;
    public GameObject apagar;
    public GameObject encender;
    // Start is called before the first frame update
    void Start()
    {
        apagar.SetActive(false);
        encender.SetActive(false);
        StartCoroutine("Switch");
    }

    IEnumerator Switch(){
        while(true){
            apagar.SetActive(true);
            yield return new WaitForSeconds(3);
            light.SetActive(false);
            apagar.SetActive(false);
            yield return new WaitForSeconds(3);
            encender.SetActive(true);
            yield return new WaitForSeconds(3);
            light.SetActive(true);
            encender.SetActive(false);
            yield return new WaitForSeconds(3);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
