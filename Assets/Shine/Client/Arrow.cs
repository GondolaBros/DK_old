using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float Speed = 20f;
    public Rigidbody2D physics;
    public float TimeLeft = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        physics.velocity = transform.right * Speed;
    }

    private void Update()
    {
        TimeLeft -= Time.deltaTime;
        if (TimeLeft <= 0.0f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            Turret turret = collision.GetComponent<Turret>();
            if (turret != null)
            {
                if (turret.EntityType == EntityType.Turret)
                {
                    turret.TakeDamage(new Damage(DamageType.Physical, 67));
                }
            }
            Destroy(gameObject);
        }
    }
}
