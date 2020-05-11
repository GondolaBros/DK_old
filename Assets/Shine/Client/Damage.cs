using UnityEngine;

public class Damage
{
    public DamageType Type { get; private set; }
    public float Amount { get; private set; }

    public Damage(DamageType type, int amount)
    {
        this.Type = type;
        this.Amount = amount;
    }
}

public enum DamageType
{
    Physical = 0x0,
    Magical = 0x1
}
