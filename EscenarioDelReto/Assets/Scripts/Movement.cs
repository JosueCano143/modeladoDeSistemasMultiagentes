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
    private int step = 1;
    private float speed;
    public float timeStep = 1.0f;
    private Vector3 current;
    private Vector3 next;

    /* void Awake(){


    } */


    // Start is called before the first frame update
    void Start()
    {
        speed = 10/timeStep;
        InvokeRepeating("stepController", 0.0f, timeStep);
    }

    // Update is called once per frame
    void Update()
    {
        if (next != current){
            transform.Translate(new Vector3(0,0,speed*Time.deltaTime));
        }
    }

    void stepController(){
        current = path[step];
        transform.position = current;
        step += 1;
        if(step != path.Length-1){
            next = path[step];
            transform.LookAt(next);
        } else{
            Destroy(gameObject,.0f);
            Debug.Log("Recorrido Finalizado");
            Debug.Log("Destruccion de Agente");
        }
    }
}