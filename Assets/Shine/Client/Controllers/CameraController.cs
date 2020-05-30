using UnityEngine;

public class CameraController : MonoBehaviour
{
    private float cameraSmooth = 100f;
    private float cameraDistance = -8f;
    private float adjustedDistance = -8f;

    private float zoomSmooth = 10f;
    private float maxZoom = -1f;
    private float minZoom = -4f;

    private float orbitXRotation = -20f;
    private float orbitYRotation = -180f;

    private Vector3 cameraOffset;

    private Transform target;

    private void Awake()
    {
        this.target = GameObject.FindGameObjectWithTag("Player").transform.Find("CameraTarget");
        this.transform.rotation = Quaternion.Euler(65f, 90f, 0f);
        this.cameraOffset = new Vector3(-2.774f, 5.14f, 0f);
    }

    private void Update()
    {
        HandleZoom();
    }

    private void FixedUpdate()
    {
        Vector3 newPosition = target.position + cameraOffset;
        newPosition.x = Mathf.Clamp(newPosition.x, 1f, 50f);
        newPosition.z = Mathf.Clamp(newPosition.z, 10f, 30f);
        transform.position = newPosition;
    }

    private void HandleZoom()
    {
        float cameraDistance = this.cameraOffset.x;
        cameraDistance += Input.GetAxis("Mouse ScrollWheel") * zoomSmooth;
        cameraDistance = Mathf.Clamp(cameraDistance, minZoom, maxZoom);
        this.cameraOffset.x = cameraDistance;
    }
}