using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateCar : MonoBehaviour
{

    public GameObject prefabCar;
    bool create = true;
    int AgentLimits = 5;
    // Start is called before the first frame update
    void Start()
    {
        create = true;
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if(create){    
            if(Handler.data.Length > AgentLimits && create){
                create = false;
                StartCoroutine(aux());
                GameObject car = Instantiate(prefabCar, new Vector3(-80.0f, 6.2f, -0.1f), Quaternion.identity);
                Debug.Log("CREANDO INSTANCIA");
                car.GetComponent<Movement>().path = Handler.path;
                Handler.data = "";
            }
        }

        */
    }

    IEnumerator aux(){
        yield return new WaitForSeconds(2);
    }
}
