using UnityEngine;

public class CameraController : MonoBehaviour
{
    private float cameraDistance = 6f;
    private float adjustedDistance = 6f;
    private float cameraZRotation = -65f;
    
    Vector3 destination = Vector3.zero;
    Vector3 adjustedDestination = Vector3.zero;

    private Vector3 cameraVelocity = Vector3.zero;
    private float cameraSmooth = 0.1f;

    private float zoomSmooth = 20f;
    private float maxZoom = 6f;
    private float minZoom = 2f;

    private Transform target;
    
    CameraCollider cameraCollider = new CameraCollider();


    private void Awake()
    {
        this.target = GameObject.FindGameObjectWithTag("Player").transform.Find("CameraTarget");
        this.cameraCollider.Initialize(Camera.main);
    }

    private void Update()
    {
        HandleZoom();
    }

    private void FixedUpdate()
    {
        FollowTarget();
        FocusOnTarget();
        HandleCameraCollisions();
    }

    private void FollowTarget()
    {
        destination = Quaternion.Euler(0, 0, cameraZRotation) * Vector3.left * cameraDistance;
        destination += target.position;// + (Vector3.right * Input.GetAxisRaw("Vertical") * 0.5f);

        Vector3 correctedDestination = destination;
        if (cameraCollider.Colliding)
        {
            adjustedDestination = Quaternion.Euler(0, 0, cameraZRotation) * Vector3.left * adjustedDistance;
            adjustedDestination += target.position;
            correctedDestination = adjustedDestination;
        }

        correctedDestination.x = Mathf.Clamp(correctedDestination.x, 1f, 50f);
        correctedDestination.z = Mathf.Clamp(correctedDestination.z, 10f, 30f);
        transform.position = Vector3.SmoothDamp(transform.position, correctedDestination, ref cameraVelocity, cameraSmooth);
    }

    private void FocusOnTarget()
    {
        Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, cameraSmooth * Time.deltaTime);
    }

    private void HandleCameraCollisions()
    {
        cameraCollider.UpdateCameraClipPoints(transform.position, transform.rotation, ref cameraCollider.AdjustedCameraClipPoints);
        cameraCollider.UpdateCameraClipPoints(destination, transform.rotation, ref cameraCollider.DesiredCameraClipPoints);
        cameraCollider.CheckColliding(target.position);
        adjustedDistance = cameraCollider.GetAdjustedDistanceWithRayFrom(target.position);    
    }

    private void HandleZoom()
    {
        cameraDistance += Input.GetAxis("Mouse ScrollWheel") * zoomSmooth;
        cameraDistance = Mathf.Clamp(cameraDistance, minZoom, maxZoom);
        adjustedDistance = Mathf.Clamp(adjustedDistance, minZoom, maxZoom);
    }
}