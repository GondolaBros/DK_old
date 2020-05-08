using UnityEngine;

public interface  IEntity
{
    EntityType Type { get; set;}
    int CurrentHealth { get; set; }
    int MaxHealth { get; set; }
    int PhysicalDamage { get; set; }
    int PhysicalDefense { get; set; }
    int MagicalDamage { get; set; }
    int MagicalDefense { get; set; }
}

public enum EntityType : byte
{ 
    Champion = 0x0,
    Unit = 0x1,
    Castle = 0x2,
    Turret = 0x3
}
