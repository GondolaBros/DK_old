using UnityEngine;

public class Turret : MonoBehaviour, IDamageable
{
    public float CurrentHealth;
    public float MaxHealth;
    public float PhysicalDamage;
    public float MagicalDefense;
    public float PhysicalDefense;

    public EntityType EntityType;

    void Awake()
    {
        CurrentHealth = 100;
        MaxHealth = 100;
        PhysicalDefense = 50;
        EntityType = EntityType.Turret;
    }

    public void TakeDamage(Damage damage)
    {
        if (damage.Type == DamageType.Physical)
        {
            float damageToApply = damage.Amount * (100 / (100 + PhysicalDefense));
            Debug.Log("Damage to apply before rounding: " + damageToApply);

           // float roundedValue = Mathf.RoundToInt
            //Debug.Log("Damage after rounding: " + roundedValue);
            //CurrentHealth -= damageToApply;
            //Debug.Log("Turret HP: " + CurrentHealth + "/" + MaxHealth);
        }
    }
}
