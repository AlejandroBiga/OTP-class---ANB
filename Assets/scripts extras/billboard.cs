using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class billboard : MonoBehaviour
{
    public Camera mainCamera;
    void Start()
    {
        CinemachineBrain brain = Camera.main.GetComponent<CinemachineBrain>();
        if (brain != null && brain.ActiveVirtualCamera != null)
        {
            mainCamera = Camera.main; 
        }

    }

    void Update()
    {
        Vector3 cameraPosition = mainCamera.transform.position;
        Vector3 objectPosition = transform.position;
        Vector3 direction = new Vector3(cameraPosition.x - objectPosition.x, 0, cameraPosition.z - objectPosition.z);
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}
