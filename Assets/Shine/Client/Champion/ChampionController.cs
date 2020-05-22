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

    public float MovementSpeed;
    public bool Moving { get; private set; }
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
    }

    private void Update()
    {
        anim.SetFloat("Speed", velocity.magnitude);

        if (velocity.magnitude > 1e-05 && !Moving)
            Moving = true;
        if (velocity.magnitude < 0.1f && Moving)
            Moving = false;

        isIdleShooting = !Moving && Shooting;

        if (isIdleShooting)
            canMove = false;
        else
            canMove = true;

        if ((Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.Alpha1)) && !Shooting)
        {
            StartCoroutine(Shoot(0.5f));
        }
    }

    private void FixedUpdate()
    {
        if (IsMine)
        {
            if (canMove)
            {
                x = Input.GetAxis("Horizontal");
                y = Input.GetAxis("Vertical");
                velocity = new Vector3(x, y, 0f);
                transform.position += velocity * Time.deltaTime * MovementSpeed;
            }

            if (velocity.x < 0 && !flipped)
            {
                flipped = true;
                transform.Rotate(0f, 180f, 0f);
            }
            if (velocity.x > 0 && flipped)
            {
                transform.Rotate(0f, 180f, 0f);
                flipped = false;
            }
        }
        // for debug purposes lel
        else
        {
            //velocity = new Vector3(Vector2.right.x, Vector2.right.y, 0f);
            //transform.position += velocity * Time.deltaTime * MovementSpeed;
        }
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
