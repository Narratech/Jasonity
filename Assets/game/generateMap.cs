using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class generateMap : MonoBehaviour
{
    public int height, width;
    public GameObject or;
    public GameObject bot;
    public int numberOfBots = 2;
    public Camera n;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("create");
        
    }
    IEnumerator create(){
        for(int i=0; i < width; i++){
            for(int j=0; j < height; j++){
                Debug.Log(i + " " + j);
                Instantiate(or, new Vector3(i, j, 0), Quaternion.identity);   
            }
        }
        n.transform.position = new Vector3((width-1.0f)/2, (height-1.0f)/2, -10);
        n.orthographicSize = width*0.28125f;
        for(int z = 0; z < numberOfBots; z++){
            int y = Random.Range(0,height);
            int x = Random.Range(0,width);
            GameObject b = Instantiate(bot, new Vector3(x, y, -1), Quaternion.identity);
            b.GetComponent<randomlinealmovement>().width = width; 
            b.GetComponent<randomlinealmovement>().height = height; 
        }
        yield return new WaitForSeconds(.1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
