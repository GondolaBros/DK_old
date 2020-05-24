using System.Collections;
using UnityEngine;

public class ChampionController : MonoBehaviour
{
    private float rotateVelocity = 200f;
    private Quaternion targetRotation;
    private Animator anim = null;

    public bool Shooting { get; private set; }
    public bool IsMine { get; private set; }

    // Start is called before the first frame update
    void Awake()
    {
        anim = GetComponent<Animator>();

        // We're setting this to prepare for network behaviour... aswell as identifying bots from player objects
        if (this.tag == "Player")
            IsMine = true;
        else
            IsMine = false;

            Debug.Log(gameObject.name);
    }

    

    void FixedUpdate()
    {
        Vector3 inputVector = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        //Set animation state based on input (idle, walking, running, strafing)
        //SetAnimationState(movementVector);

        //Rotate();

        anim.SetFloat("MovementX", inputVector.x);
        anim.SetFloat("MovementZ", inputVector.z);
    }

    
    void Rotate()
    {
        //Get rotation input
        float rotationInput = Input.GetAxis("Horizontal");

        targetRotation = Quaternion.AngleAxis(rotateVelocity * rotationInput * Time.deltaTime, Vector3.up) * targetRotation;
        transform.rotation = targetRotation;
    }

    
    void SetAnimationState(Vector3 input)
    {
        bool strafing = (input.x != 0.0f);
        bool running = (input != Vector3.zero && Input.GetKey(KeyCode.LeftShift));
        bool walking = (input != Vector3.zero && !strafing && !running);

        
        anim.SetBool("IsWalking", walking);
        anim.SetBool("IsRunning", running);
        anim.SetBool("IsStrafing", strafing);
    }
        

    private IEnumerator Shoot(float cooldown)
    {
        if (IsMine)
        {
            Shooting = true;
            anim.SetBool("Shooting", Shooting);

            //Instantiate(ArrowPrefab, FirePoint.position, FirePoint.rotation);

            yield return new WaitForSeconds(cooldown);

            Shooting = false;
            anim.SetBool("Shooting", Shooting);
        }
    }
}
