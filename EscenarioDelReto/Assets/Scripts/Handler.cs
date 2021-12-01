using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Random=UnityEngine.Random;

public class Handler : MonoBehaviour
{
    // Use this for initialization
    public GameObject[] carPrefab;
    public GameObject parkingSlot;
    System.Threading.Thread SocketThread;
    volatile bool keepReading = false;
    string data = null;
    bool callAgentController = true;
    bool createParkings = true;

    void Start()
    {
        Application.runInBackground = true;
        startServer();
    }

    void Update()
    {
        if (data != null && callAgentController){
            callAgentController = false;
            agentController();
        }
    }

    void startServer()
    {
        SocketThread = new System.Threading.Thread(networkCode);
        SocketThread.IsBackground = true;
        SocketThread.Start();
    }

    private string getIPAddress()
    {
        IPHostEntry host;
        string localIP = "";
        host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                localIP = ip.ToString();
            }

        }
        return localIP;
    }


    Socket listener;
    Socket handler;

    void networkCode()
    {
        try {
            // Data buffer for incoming data.
            byte[] bytes = new Byte[1024];

            // host running the application.
            //Create EndPoint
            IPAddress IPAdr = IPAddress.Parse("127.0.0.1"); // DirecciÃ³n IP
            IPEndPoint localEndPoint = new IPEndPoint(IPAdr, 1101);

            // Create a TCP/IP socket.
            listener = new Socket(AddressFamily.InterNetwork,SocketType.Stream, ProtocolType.Tcp);

            listener.Bind(localEndPoint);
            listener.Listen(10);
       
			while (true) { 		
                // Program is suspended while waiting for an incoming connection.
                Debug.Log("Waiting for Connection");     //It works

                handler = listener.Accept();
                Debug.Log("Client Connected");     //It doesn't work

                byte[] SendBytes = System.Text.Encoding.Default.GetBytes("I will send key");
                handler.Send(SendBytes); // dar al cliente

                int bytesRec;
					
                // Read incomming stream into byte arrary. 					
                while ((bytesRec = handler.Receive(bytes)) != 0) {
                    var incommingData = new byte[bytesRec];
                    Array.Copy(bytes, 0, incommingData, 0, bytesRec);
                    // Convert byte array to string message.
                    string serverMessage = System.Text.Encoding.ASCII.GetString(incommingData);
                    data = serverMessage;
                    Debug.Log("Message received as: " + serverMessage);
                }
                Debug.Log("Total data: " + data);
			}         
		}         
		catch (SocketException socketException) {             
			Debug.Log("Socket exception: " + socketException);         
		}
    }

    
    void agentController(){
        
        if(createParkings){
            createParkings = false;

            Debug.Log("Entre");

            string[] parkingsPos = data.Split('%');

            foreach(var park in parkingsPos){
                string[] cor = park.Split(',');
                Instantiate(parkingSlot, new Vector3(float.Parse(cor[0]) * 10 + 4f, 4.28f, float.Parse(cor[1]) * 10 + 3.5f), Quaternion.identity);
            }

            data = null;
            callAgentController = true;
            return;
        }
 
        Debug.Log("HOLA -----------" + data);

        string[] paths = data.Split('!');
        Debug.Log("CANTIDAD PATHS: " + (paths.Length - 1));
        
        // Iteracion sobre paths, cantidad de instancias a crear
        for(int i = 0; i < paths.Length - 1; i++)
        {
            // Obtencion de las coordenadas enviadas desde Python (para 1 instancia)
            string[] cords = paths[i].Split('/');

            int numSteps = cords.Length;
            Vector3[] path = new Vector3[numSteps];
            
            // Creacion del path para la instancia de la iteracion actual
            int idx = 0;
            for(int j = 0; j < cords.Length - 1; j++)
            {
                
                string[] cor = cords[j].Split(',');
                //System.Console.WriteLine($"{float.Parse(cor[0])} - {float.Parse(cor[1])}");

                path[idx] = new Vector3(float.Parse(cor[0]) * 10, 6.2f, float.Parse(cor[1]) * 10);
                
                //Debug.Log(path[idx]);

                idx += 1;
            }

            int vInt = Random.Range(0, 4);
            Debug.Log("VEHICLE: " + vInt);

            GameObject car = Instantiate(carPrefab[vInt], path[0], Quaternion.identity);
            car.GetComponent<Movement>().path = path;
        }

        data = null;
        callAgentController = true;

    }

    void stopServer()
    {
        keepReading = false;

        //stop thread
        if (SocketThread != null)
        {
            SocketThread.Abort();
        }

        if (handler != null && handler.Connected)
        {
            handler.Disconnect(false);
            Debug.Log("Disconnected!");
        }
    }

    void OnDisable()
    {
        stopServer();
    }
}