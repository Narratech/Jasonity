using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class randomlinealmovement : MonoBehaviour
{
    public float speed = 1.0f;
    public int width, height;
    private bool done = true;
    private bool lastWasVert = false;
    private Vector3 target;
    public GameObject image;
    public TextMesh text;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(done){
            if(lastWasVert){
                //movimito horizontal
                int x = Random.Range(0,width);
                target = new Vector3(x, this.transform.position.y, -0.52f);
                done = false;
                lastWasVert = false;
            }else{
                //movimiento vertical
                int y = Random.Range(0,height);
                target = new Vector3(this.transform.position.x, y, -0.52f);
                done = false;
                lastWasVert = true;
            }
            StartCoroutine("showMessage");
        }else{
            // Move our position a step closer to the target.
            float step =  speed * Time.deltaTime; // calculate distance to move
            transform.position = Vector3.MoveTowards(transform.position, target, step);

            // Check if the position of the cube and sphere are approximately equal.
            if (Vector3.Distance(transform.position, target) < 0.001f)
            {
                done = true;
            }
        }
    }
    IEnumerator showMessage(){
        text.text = "I'm going to\n" + target.x + "," + target.y + "!";
        image.SetActive(true);
        yield return new WaitForSeconds(3.1f);
        image.SetActive(false);
    }
}
