using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float Speed = 20f;
    public Rigidbody2D physics;

    // Start is called before the first frame update
    void Start()
    {
        physics.velocity = transform.right * Speed;
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
                    turret.TakeDamage(new Damage(DamageType.Physical, 5));
                }
            }
            Destroy(gameObject);
        }
    }
}
