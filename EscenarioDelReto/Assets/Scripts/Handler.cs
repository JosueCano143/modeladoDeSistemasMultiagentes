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
    // Inicialización de variables.
    public GameObject[] carPrefab;
    public GameObject parkingSlot;
    System.Threading.Thread SocketThread;
    volatile bool keepReading = true;
    string data = null;
    bool callAgentController = true;
    bool simulationFinished = false;
    float time;

    // Funcion Start, llamada al comienza de la simulación.
    void Start()
    {
        Application.runInBackground = true;
        startServer();
    }

    // Funcion Update, llamada en cada frame de la simulación.
    void Update()
    {
        // Control de creación de agentes, mientras que aun se reciban datos desde python llamar a la funcion que controla agentes.
        if (data != null && callAgentController){
            callAgentController = false;
            agentController();
        }

        // Una vez que ya no se reciban datos y la simulación llegue a su step final se destruyen los agentes de estacionamiento.
        if (simulationFinished){
            GameObject[] parkings = GameObject.FindGameObjectsWithTag("ParkingLot");
            foreach (GameObject park in parkings){
                Destroy(park, .0f);
            }
            simulationFinished = false;
        }
    }

    // Funcion que inicializa el socket que funge como servidor. 
    void startServer()
    {
        SocketThread = new System.Threading.Thread(networkCode);
        SocketThread.IsBackground = true;
        SocketThread.Start();
    }

    // Funcion que obtiene la direccion IP.
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

    // Funcion que recibe la información la información a través del socket.
    void networkCode()
    {
        try {
            // Array de bytes para los datos entrantes.
            byte[] bytes = new Byte[1024];

            //Creación del EndPoint.
            IPAddress IPAdr = IPAddress.Parse("127.0.0.1"); // Dirección IP
            IPEndPoint localEndPoint = new IPEndPoint(IPAdr, 1101);

            // Creacion de TCP/IP socket.
            listener = new Socket(AddressFamily.InterNetwork,SocketType.Stream, ProtocolType.Tcp);

            listener.Bind(localEndPoint);
            listener.Listen(10);

            // Ciclo para la lectura de infomación.
			while (keepReading) {	
                
                Debug.Log("Waiting for Connection");

                handler = listener.Accept();
                Debug.Log("Client Connected");

                // Respuesta al cliente de conexion exitosa.
                byte[] SendBytes = System.Text.Encoding.Default.GetBytes("Successful Connection");
                handler.Send(SendBytes);

                int bytesRec;
					
                // Lectura de los datos entrantes.			
                while ((bytesRec = handler.Receive(bytes)) != 0) {
                    var incommingData = new byte[bytesRec];
                    Array.Copy(bytes, 0, incommingData, 0, bytesRec);
                    // Conversion del arreglo de bytes a string.
                    string serverMessage = System.Text.Encoding.ASCII.GetString(incommingData);
                    data = serverMessage;
                    Debug.Log("Message received as: " + serverMessage);
                }

                simulationFinished = true;
                Debug.Log("Simulación Finalizada");
                Debug.Log("Destrucción de Espacios de Estacionamiento");
			}         
		}         
		catch (SocketException socketException) {             
			Debug.Log("Socket exception: " + socketException);         
		}
    }

    // Funcion que se encarga de instanciar y asignar informacion a los agentes.
    void agentController(){

        // Manejo de los datos recibidos para la creacion de coordenadas.
        string[] totalData = data.Split(':');

        if (totalData[0] == "vehicles"){
            string[] paths = totalData[1].Split('!');
            
            // Iteracion sobre paths, cantidad de instancias a crear.
            for(int i = 0; i < paths.Length - 1; i++)
            {
                // Obtencion de las coordenadas enviadas desde Python (para 1 instancia).
                string[] cords = paths[i].Split('/');

                int numSteps = cords.Length;
                Vector3[] path = new Vector3[numSteps];
                
                // Creacion del path para la instancia de la iteracion actual.
                int idx = 0;
                for(int j = 0; j < cords.Length - 1; j++){     
                    string[] cor = cords[j].Split(',');
                    path[idx] = new Vector3(float.Parse(cor[0]) * 10, 6.2f, float.Parse(cor[1]) * 10);
                    idx += 1;
                }

                int vInt = Random.Range(0, 7);

                // Instanciacion de agentes.
                GameObject car = Instantiate(carPrefab[vInt], path[0], Quaternion.identity);
                car.GetComponent<Movement>().path = path;
                car.GetComponent<Movement>().timeStep = time;
            }
        } // Manejo de infomación inicial para la generacion de estacionamientos.
        else if (totalData[0] == "parkings"){
            string[] parkingsPos = totalData[1].Split('/');
            foreach (var park in parkingsPos){
                string[] cor = park.Split(',');

                // Instanciacion de los agentes estacionamiento.
                GameObject parking = Instantiate(parkingSlot, new Vector3(float.Parse(cor[0]) * 10 - 3f, 4.29f, float.Parse(cor[1]) * 10 -3f), Quaternion.identity);
                parking.tag = "ParkingLot";
            }
        }
        else if (totalData[0] == "timeStep"){
            float fTime = float.Parse(totalData[1]);
            time = fTime;
        }

        data = null;
        callAgentController = true;
    }

    // Funcion que aborta o desconecta el socket en dependencia a la situacion. 
    void stopServer(){
        keepReading = false;

        // Detencion del socket.
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

    void OnDisable(){
        stopServer();
    }
}