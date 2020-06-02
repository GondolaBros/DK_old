using UnityEngine;

public class NimmiController : MonoBehaviour
{
    private const float FORTY_FIVE = 45f;
    private const float GLOBAL_OFFSET = 90f;

    [SerializeField]
    private float rotateSpeed = 5f;
    [SerializeField]
    private float accelerateSpeed = 5f;
    private float worldRotationAngle = 0.0f;

    private Quaternion targetRotation;
    private bool isGrounded = false;

    private Animator anim;

    private Vector2 movementInput;

    private new Rigidbody rigidbody;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        movementInput = new Vector2();
    }

    private void Update()
    {
        float movementX = Input.GetAxisRaw("Horizontal"), movementY = Input.GetAxisRaw("Vertical");
        movementInput.x = movementX > 0 ? 1 : movementX < 0 ? -1 : 0;
        movementInput.y = movementY > 0 ? 1 : movementY < 0 ? -1 : 0;
    }

    private void FixedUpdate()
    {
        SetPhysicsConstraints();
        HandleRotation();
        HandleMovement();
        HandleJumping();
    }

    private void HandleRotation()
    {
        float horizontalAngle = movementInput.x == 1 ? 90 : movementInput.x == -1 ? 270 : 0;
        float verticalAngle = movementInput.y == -1 ? 180 : movementInput.y == 1 && movementInput.x < 0 ? 360 : 0;

        // float horizontalAngle = movementInput.x > 0 ? 90 : movementInput.x < 0 ? 270 : 0;
        // float verticalAngle = movementInput.y < -1 ? 180 : movementInput.y > 0 && movementInput.x < 0 ? 360 : 0;



        Debug.Log("Horizontal Axis = " + movementInput.x);
        Debug.Log("Vertical Axis = " + movementInput.y);
        Debug.Log("Horizontal Angle = " + horizontalAngle);
        Debug.Log("Vertical Angle = " + verticalAngle);


        if (movementInput.x != 0 && movementInput.y != 0)
        {
            if (horizontalAngle > verticalAngle)
            {
                worldRotationAngle = verticalAngle + FORTY_FIVE;
            }
            else if (horizontalAngle < verticalAngle)
            {
                worldRotationAngle = horizontalAngle + FORTY_FIVE;
            }
            else
            {
                worldRotationAngle = 0.0f;
            }
        }
        else if (movementInput.x != 0)
        {
            worldRotationAngle = horizontalAngle;
        }
        else if (movementInput.y != 0)
        {
            worldRotationAngle = verticalAngle;
        }

        targetRotation = Quaternion.AngleAxis(worldRotationAngle + GLOBAL_OFFSET, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime); //SmoothDamp?

        // Debug.Log("Transform.rotation=" + transform.rotation.eulerAngles.y);
        // Debug.Log("TargetRotation=" + targetRotation.eulerAngles.y);
        // Debug.Log("Return:" + (Mathf.Abs(transform.rotation.eulerAngles.y - targetRotation.eulerAngles.y) < 10f));
        Vector3 clampedRotation = transform.rotation.eulerAngles;
        clampedRotation.y = clampedRotation.y + Mathf.Ceil(-clampedRotation.y / 360f) * 360f;
        transform.rotation = Quaternion.Euler(clampedRotation);
    }

    private void SetPhysicsConstraints()
    {
        if(Mathf.Abs(transform.rotation.eulerAngles.y - targetRotation.eulerAngles.y) < FORTY_FIVE)
        {
            rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        }
        else
        {
            rigidbody.constraints = RigidbodyConstraints.FreezeRotation 
                | RigidbodyConstraints.FreezePositionX
                | RigidbodyConstraints.FreezePositionZ;   
        }       
    }

    private void HandleMovement()
    {
        float movementValue = 0f;
        if(Mathf.Abs(movementInput.magnitude - anim.GetFloat("MovementAxis")) > 0.01f)
        {
            movementValue = Mathf.Lerp(
                anim.GetFloat("MovementAxis"), 
                Mathf.Abs(movementInput.magnitude), 
                accelerateSpeed * Time.deltaTime
            );
        }
        else
        {
            movementValue = Mathf.Abs(movementInput.magnitude);
        }

        anim.SetFloat("MovementAxis", movementValue);

 
    }

    private void HandleJumping()
    {
        if(Physics.Raycast(transform.position, Vector3.down, 2f))
        {
            isGrounded = true;
            Debug.Log("IsGrounded");
        } 
        else
        {
            isGrounded = false;
        }

        if(isGrounded && Input.GetButtonDown("Jump"))
        {
            Debug.Log("Jump");
            anim.SetTrigger("Jump");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + (Vector3.down * 2f));
    }
}
