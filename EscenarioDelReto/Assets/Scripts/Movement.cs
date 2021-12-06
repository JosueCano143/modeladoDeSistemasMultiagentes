using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;


public class Movement : MonoBehaviour
{
    // Inicializacion de variables.
    public Vector3[] path;
    private int step = 1;
    private float speed;
    public float timeStep = 1.0f;
    private Vector3 current;
    private Vector3 next;

    // Función Start, llamada al comienzo de la simulación.
    void Start()
    {
        speed = 10/timeStep;
        // Llamada repetitica de la funcion stepController cada segundo.
        InvokeRepeating("stepController", 0.0f, timeStep);
    }

    // Función Update, llamada en cada frame.
    void Update()
    {
        // Reakizacion de la tranlación del agente al siguiente punto del patrón.
        if (next != current){
            transform.Translate(new Vector3(0,0,speed*Time.deltaTime));
        }
    }

    // Funcion que controla la dirección y posicion de los agentes durante el recorrido de su patron.
    void stepController(){
        current = path[step];
        transform.position = current;
        step += 1;
        if(step != path.Length-1){
            next = path[step];
            transform.LookAt(next);
        } else{

            // Eliminacion del agente una vez que haya finalizado su recorrido.
            Destroy(gameObject,.0f);
            Debug.Log("Recorrido Finalizado. Destruccion de Vehículo");
        }
    }
}