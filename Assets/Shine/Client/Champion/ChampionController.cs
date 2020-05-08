using System.Collections;
using UnityEngine;

public class ChampionController : MonoBehaviour
{
    private Animator anim;
    private Vector3 velocity;

    private float x, y;
    private bool flipped;

    public Transform FirePoint;
    public GameObject ArrowPrefab;

    public float MovementSpeed;
    public bool Moving { get; private set; }
    public bool Shooting { get; private set; }

    // Start is called before the first frame update
    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (velocity.magnitude > 1e-05 && !Moving)
            Moving = true;
        if (velocity.magnitude < 0.1f && Moving)
            Moving = false;

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

        anim.SetFloat("Speed", velocity.magnitude);

        // If we click the left mouse button to shoot, we're moving, and we aren't already shooting,
        // then start the couroutine to shoot; we want this because we need a flag that determines when a user is shooting, and we dont
        // want them to spam it!
        if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Alpha1)) && Moving && !Shooting)
        {
            StartCoroutine(Shoot(0.5f));
        }
    }

    private IEnumerator Shoot(float cooldown)
    {
        Shooting = true;
        anim.SetBool("RunNShoot", Shooting);

        Instantiate(ArrowPrefab, FirePoint.position, FirePoint.rotation);
        yield return new WaitForSeconds(cooldown);

        Shooting = false;
        anim.SetBool("RunNShoot", Shooting);
    }

    private void FixedUpdate()
    {
        x = Input.GetAxis("Horizontal");
        y = Input.GetAxis("Vertical");
        velocity = new Vector3(x, y, 0f);
        transform.position += velocity * Time.deltaTime * MovementSpeed;
    }
}
