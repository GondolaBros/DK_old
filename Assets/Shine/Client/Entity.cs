using UnityEngine;

public abstract class Entity
{
    public SpriteRenderer SpriteRenderer { get; private set; }
    public Animator Animator { get; private set; }
    public EntityType Type { get; private set;}
    public string Name { get; private set; }
    public int CurrentHealth { get; private set; }
    public int MaxHealth { get; private set; }
    public int PhysicalDamage { get; private set; }
    public int PhysicalDefense { get; private set; }
    public int MagicalDamage { get; private set; }
    public int MagicalDefense { get; private set; }
}

public enum EntityType
{ 
    Champion = 0x0,
    Unit = 0x1,
    Castle = 0x2,
    Turret = 0x3
}
