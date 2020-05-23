using System.Collections;
using UnityEngine;

public class ChampionController : MonoBehaviour
{
    private float x, y;
    private bool flipped;
    private bool canMove;
    private bool isIdleShooting;

    private Animator anim;
    private Vector3 velocity;

    public Transform FirePoint;
    public GameObject ArrowPrefab;

    public bool Moving { get; private set; }
    public bool Shooting { get; private set; }
    public bool IsMine { get; private set; }

    // Maximum turn rate in degrees per second.
    public float turningRate = 50; 
    // Rotation we should blend towards.
    private Quaternion _targetRotation = Quaternion.identity;
    // Call this when you want to turn the object smoothly.

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

    private void Update()
    {
        if (IsMine)
        {

            y = Input.GetAxis("Vertical");
            velocity = new Vector3(0, y, 0);

            //Quaternion newRotation = transform.rotation 

            anim.SetFloat("MovementAxis", velocity.magnitude);

            //transform.Rotate(0, x*4, 0);

            if ((Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.Alpha1)) && !Shooting)
            {
                //StartCoroutine(Shoot(0.5f));
            }
        }
    }

    private void LateUpdate()
    {
        x = Input.GetAxis("Horizontal");
        transform.rotation *= Quaternion.Euler(Vector3.up * x * 90 * Time.deltaTime);
    }

    private IEnumerator Shoot(float cooldown)
    {
        if (IsMine)
        {
            Shooting = true;
            anim.SetBool("Shooting", Shooting);

            Instantiate(ArrowPrefab, FirePoint.position, FirePoint.rotation);

            yield return new WaitForSeconds(cooldown);

            Shooting = false;
            anim.SetBool("Shooting", Shooting);
        }
    }
}
