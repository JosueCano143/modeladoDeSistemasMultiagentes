using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class Handler : MonoBehaviour
{
    // Use this for initialization
    public GameObject carPrefab;
    System.Threading.Thread SocketThread;
    volatile bool keepReading = false;
    string data = null;

    void Start()
    {
        Application.runInBackground = true;
        startServer();
    }

    void Update()
    {
        if (data != null){
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
                    data += serverMessage;
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
        Debug.Log("Data in controller: " + data);

        string[] words = data.Split('$');

        int numAgents = Int32.Parse(words[0]);
        data = words[1];

        string[] coordenadas = data.Split('/');

        int numSteps = coordenadas.Length;
        Vector3[] path = new Vector3[numSteps];

        int idx = 0;
        foreach (var word in coordenadas)
        {
            //System.Console.WriteLine($"{word}");
            String[] cor = word.Split(',');
            System.Console.WriteLine($"{float.Parse(cor[0])} - {float.Parse(cor[1])}");

            path[idx] = new Vector3(float.Parse(cor[0]), 6.2f, float.Parse(cor[1]));
            
            Debug.Log(path[idx]);

            idx += 1;
        }
        GameObject car = Instantiate(carPrefab, new Vector3(0.0f, 6.2f, 41.5f), Quaternion.identity);
        car.GetComponent<Movement>().path = path;

        data = null;
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