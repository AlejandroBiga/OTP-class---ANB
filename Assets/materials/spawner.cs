using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawner : MonoBehaviour
{
    public Transform spawnPoint;
    


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) 
        {
            spawnObject();

        }
    }

    private void spawnObject()
    {
        GameObject obj = poolobject.SharedInstance.GetPooledObject();

        if (obj != null)
        {
            Transform SpawnPoint = spawnPoint.transform;
            obj.transform.position = spawnPoint.position;
            obj.transform.rotation = spawnPoint.rotation;
            obj.SetActive(true);


        }

    }

}
