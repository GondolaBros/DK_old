using UnityEngine;

public class CameraController1 : MonoBehaviour
{
    private Bounds mapBounds;
    private GameObject playerCache;

    private float camExtentV;
    private float camExtentH;
    private float leftBound;
    private float rightBound;
    private float bottomBound;
    private float topBound;

    public bool IsLocked;
    public float HoverOffset;
    public float ScrollSpeed;

    // Start is called before the first frame update
    void Awake()
    {
        mapBounds = GameObject.Find("Map_Level_1").GetComponent<SpriteRenderer>().bounds;
        playerCache = GameObject.Find("Champion");
        IsLocked = true;

        camExtentV = Camera.main.orthographicSize;
        camExtentH = camExtentV * Screen.width / Screen.height;

        leftBound = mapBounds.min.x + camExtentH;
        rightBound = mapBounds.max.x - camExtentH;
        bottomBound = mapBounds.min.y + camExtentV;
        topBound = mapBounds.max.y - camExtentV;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y) && !IsLocked)
        {
            IsLocked = true;
        }
        else if (Input.GetKeyDown(KeyCode.Y) && IsLocked)
        {
            IsLocked = false;
        }
    }

    private void LateUpdate()
    {
        Vector3 cameraPos = IsLocked ? new Vector3(playerCache.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z) :
            new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z);

        if (!IsLocked)
        {
            if (Input.mousePosition.x >= Screen.width - HoverOffset)
            {
                cameraPos += Vector3.right * Time.deltaTime * ScrollSpeed;
            }
            if (Input.mousePosition.x <= HoverOffset)
            {
                cameraPos += Vector3.left * Time.deltaTime * ScrollSpeed;
            }
        }

        cameraPos.x = Mathf.Clamp(cameraPos.x, leftBound, rightBound);
        cameraPos.y = Mathf.Clamp(cameraPos.y, bottomBound, topBound);

        transform.position = cameraPos;
    }
}







//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class CameraController : MonoBehaviour
//{
//    private float cameraSmooth = 100f;
//    private float cameraDistance = -8f;
//    private float adjustedDistance = -8f;

//    private float zoomSmooth = 10f;
//    private float maxZoom = -2f;
//    private float minZoom = -15f;

//    private float orbitXRotation = -20f;
//    private float orbitYRotation = -180f;

//    private Transform target;

//    Vector3 destination = Vector3.zero;
//    Vector3 adjustedDestination = Vector3.zero;
//    Vector3 camVelocity = Vector3.zero;

//    private void Update()
//    {
//        if (target != null)
//        {
//            HandleZoom();
//        }
//    }

//    private void FixedUpdate()
//    {
//        if (target == null)
//        {
//            GameObject playerObject;
//            if ((playerObject = GameObject.FindGameObjectWithTag("Player")) != null)
//            {
//                target = playerObject.transform.Find("CameraTarget");
//                MoveToTarget();
//                //camera collider logic
//            }
//            return;
//        }

//        MoveToTarget();
//        LookAtTarget();

//        //camera collider logic
//    }

//    private void MoveToTarget()
//    {
//        this.destination = Quaternion.Euler(orbitXRotation, orbitYRotation + target.eulerAngles.y, 0) * -Vector3.forward * cameraDistance;
//        this.destination += target.position;

//        Vector3 correctedDestination = destination;
//        //camera collider logic

//        transform.position = Vector3.SmoothDamp(transform.position, correctedDestination, ref camVelocity, Time.fixedDeltaTime);
//    }

//    private void LookAtTarget()
//    {
//        Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position);
//        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, cameraSmooth * Time.fixedDeltaTime);
//    }

//    private void HandleZoom()
//    {
//        cameraDistance += Input.GetAxis("Mouse Scrollwheel") * zoomSmooth;
//        cameraDistance = Mathf.Clamp(cameraDistance, minZoom, maxZoom);
//    }
//}
