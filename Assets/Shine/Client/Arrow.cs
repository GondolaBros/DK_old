using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float Speed;
    public float TimeLeft;

    private Rigidbody2D physics;
    private ParticleSystem lightningEffect;

    // Start is called before the first frame update
    void Awake()
    {
        physics = GetComponent<Rigidbody2D>();
        lightningEffect = GetComponentInChildren<ParticleSystem>();

        physics.velocity = transform.right * Speed;
    }

    private void Update()
    {
        TimeLeft -= Time.deltaTime;
        if (TimeLeft <= 0.0f)
        {
            lightningEffect.Play();
            Destroy(gameObject, 0.05f);
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
