using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private float cameraSmooth = 100f;
    private float cameraDistance = -8f;
    private float adjustedDistance = -8f;

    private float zoomSmooth = 10f;
    private float maxZoom = -2f;
    private float minZoom = -15f;

    private Transform target;



    private void Update()
    {
        if(target != null)
        {
            HandleZoom();
        }
    }

    private void FixedUpdate()
    {
        if (target == null)
        {
            GameObject playerObject;
            if ((playerObject = GameObject.FindGameObjectWithTag("Player")) != null)
            {
                target = playerObject.transform.Find("CameraTarget");
                //MoveToTarget();
                //camera collider logic
            }
            return;
        }



 


        //camera collider logic
    }

    private void MoveToTarget()
    {

    }


    private void HandleZoom()
    {
        cameraDistance += Input.GetAxis("Mouse ScrollWheel") * zoomSmooth;
        cameraDistance = Mathf.Clamp(cameraDistance, minZoom, maxZoom);
    }
}
