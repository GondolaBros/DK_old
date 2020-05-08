using UnityEngine;

public class CameraController : MonoBehaviour
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
