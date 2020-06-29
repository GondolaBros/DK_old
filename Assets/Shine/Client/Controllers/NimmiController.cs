using UnityEngine;

public class NimmiController : MonoBehaviour
{
    private const float TWENTY = 20f;
    private const float FORTY_FIVE = 45f;

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

    private void LateUpdate()
    {
        float currentRotation = transform.rotation.eulerAngles.y;
        // currentRotation.y = currentRotation.y + Mathf.Ceil(-currentRotation.y / 360f) * 360f;
        // transform.rotation = Quaternion.Euler(currentRotation);
        if(currentRotation == 360)
        {
            currentRotation = 0;
            transform.rotation = Quaternion.AngleAxis(currentRotation, Vector3.up);//Quaternion.Euler(currentRotation);
        }
    }

    private void HandleRotation()
    {
        float horizontalAngle = movementInput.x == 1 ? 90f : movementInput.x == -1 ? 270f : 0;
        float verticalAngle = movementInput.y == -1 ? 180f : movementInput.y == 1 && movementInput.x < 0 ? 360f : 0;

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

        targetRotation = Quaternion.AngleAxis(worldRotationAngle, Vector3.up);
        //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
    }

    private void SetPhysicsConstraints()
    {
        Debug.Log("TargetRotation Y " + targetRotation.eulerAngles.y);
        Debug.Log("CurrentRotation Y " + transform.rotation.eulerAngles.y);

        Debug.Log("Vector Difference: " + Mathf.Abs(targetRotation.eulerAngles.y - transform.rotation.eulerAngles.y));
        float rotationDifference = Mathf.Abs(targetRotation.eulerAngles.y - transform.rotation.eulerAngles.y);
        if(rotationDifference < 45f || rotationDifference > 315f)
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
            //Debug.Log("IsGrounded");
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
