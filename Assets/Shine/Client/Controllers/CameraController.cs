using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private float cameraDistance = 5f;
    private float adjustedDistance = 5f;
    private float cameraZRotation = 65f;
    
    Vector3 destination = Vector3.zero;
    Vector3 adjustedDestination = Vector3.zero;

    private Vector3 cameraVelocity = Vector3.zero;
    private float cameraSmooth = .3f;

    private float zoomSmooth = 20f;
    private float maxZoom = 5f;
    private float minZoom = 1.5f;

    private Transform target;
    
    CameraCollider cameraCollider = new CameraCollider();

    public bool flag = true;


    private void Awake()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform.Find("CameraTarget");
        cameraCollider.Initialize(Camera.main);
        cameraCollider.UpdateCameraClipPoints(transform.position, transform.rotation, ref cameraCollider.AdjustedCameraClipPoints);
        cameraCollider.UpdateCameraClipPoints(destination, transform.rotation, ref cameraCollider.DesiredCameraClipPoints);
    }

    private void Update()
    {
        HandleZoom();
    }

    private void FixedUpdate()
    {
        FollowTarget();
        HandleCameraCollisions();
    }

    private void FollowTarget()
    {
        destination = Quaternion.Euler(cameraZRotation, 0, 0) * Vector3.back * cameraDistance;

        if(flag)
        {
            destination += target.position - (Vector3.back * Input.GetAxisRaw("Vertical") * 1.5f);
        }
        else
        {
            destination += target.position;
        }
        
        Vector3 correctedDestination = destination;
        if (cameraCollider.Colliding)
        {
            adjustedDestination = Quaternion.Euler(cameraZRotation, 0, 0) * Vector3.back * adjustedDistance;
            adjustedDestination += target.position;
            correctedDestination = adjustedDestination;
        }

        //Get screen edges
        //Screen coordinates to world-point
        //If world point collides with terrain 
        correctedDestination.x = Mathf.Clamp(correctedDestination.x, 10f, 50f);
        correctedDestination.z = Mathf.Clamp(correctedDestination.z, 1f, 100f);



        transform.position = Vector3.SmoothDamp(transform.position, correctedDestination, ref cameraVelocity, cameraSmooth);
        //transform.position = Vector3.Lerp(transform.position, correctedDestination, cameraSmooth * Time.deltaTime);
    }

    private void HandleCameraCollisions()
    {
        cameraCollider.UpdateCameraClipPoints(transform.position, transform.rotation, ref cameraCollider.AdjustedCameraClipPoints);
        cameraCollider.UpdateCameraClipPoints(destination, transform.rotation, ref cameraCollider.DesiredCameraClipPoints);
        cameraCollider.CheckColliding(target.position);
        adjustedDistance = cameraCollider.GetAdjustedDistanceWithRayFrom(target.position);    
        Debug.Log("Adjusted Distance" + adjustedDistance);
    }

    private void HandleZoom()
    {
        cameraDistance += Input.GetAxis("Mouse ScrollWheel") * zoomSmooth;
        cameraDistance = Mathf.Clamp(cameraDistance, minZoom, maxZoom);
        adjustedDistance = Mathf.Clamp(adjustedDistance, minZoom, maxZoom);
    }
}