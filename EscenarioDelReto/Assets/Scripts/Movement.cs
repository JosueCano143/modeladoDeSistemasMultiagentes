using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;


public class Movement : MonoBehaviour
{
  
    public Vector3[] path;
    private int step = 0;
    public float speed = 5.0f;
    private Vector3 current;
    int numSteps = 0;

    /* void Awake(){


    } */


    // Start is called before the first frame update
    void Start()
    {

        foreach (var word in path)
        {
            Debug.Log("Path: " + word);
        }

        current = path[step];
        transform.LookAt(current);
        numSteps = path.Length;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(new Vector3(0,0,speed*Time.deltaTime));
        checkPosition();
        //transform.Translate(new Vector3(0,0,speed*Time.deltaTime));
        
    }

    void checkPosition(){
        //Debug.Log(transform.position.ToString() + ", " + current.ToString());
        float dis = Vector3.Distance(transform.position, current);
        if (dis < 0.15 && step < numSteps - 1){
            Debug.Log("Next Position = " + current);
            step += 1;
            current = path[step];
            transform.LookAt(current);

            if(current == path[numSteps - 1]){
                Destroy(gameObject,.5f);
                Debug.Log("Recorrido Finalizado");
                Debug.Log("Destruccion de Agente");
            }   
        }
    }
}

