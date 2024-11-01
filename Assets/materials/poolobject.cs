using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class poolobject : MonoBehaviour
{
    public static poolobject SharedInstance;
    public List<GameObject> ballsPrefabs;
    public GameObject objectToPool;
    public int amountToPool;

    private void Awake()
    {
        SharedInstance = this;
    }
    void Start()
    {
        ballsPrefabs = new List<GameObject>();
        for (int i = 0; i < amountToPool; i++)
        {
            GameObject tmp = Instantiate(objectToPool);
            tmp.SetActive(false);
            ballsPrefabs.Add(tmp);
        }
    }

    public GameObject GetPooledObject()
    {
        for (int i = 0; i < amountToPool; i++)
        {
            if (!ballsPrefabs[i].activeInHierarchy)
            {
                return ballsPrefabs[i];
            }
        }

        GameObject newball = Instantiate(objectToPool);
        newball.SetActive(false);
        ballsPrefabs.Add(newball);
        return newball;

       
    }


}
