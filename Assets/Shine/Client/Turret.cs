using UnityEngine;
using System.Collections.Generic;
using TMPro;

//Repair kit

public enum TurretState {
    Idle,
    Active,
    Shooting,
    Broken
}

public class Turret : MonoBehaviour, IDamageable
{
    public float CurrentHealth;
    public float MaxHealth;
    public float PhysicalDamage;
    public float MagicalDefense;
    public float PhysicalDefense;
    public EntityType EntityType;
    
    public Transform CurrentTarget;
    public float SearchRadius;
    public bool Friendly;

    public TurretState TurretState;
    public TextMeshPro ProgressLabel;

    private const int MAX_PERSUASION = 100;
    private float persuasion = 0f;
    private float captureInterval = 10f;

    void Awake()
    {
        CurrentHealth = 100;
        MaxHealth = 100;
        PhysicalDefense = 50;
        EntityType = EntityType.Turret;

        ProgressLabel = GetComponentInChildren<TextMeshPro>();
        ProgressLabel.gameObject.SetActive(false);

        SearchRadius = 7f;
    }

    public void TakeDamage(Damage damage)
    {
        if (damage.Type == DamageType.Physical) // TODO: Remove this and pass through damage stats - if the value is 0 it won't do anything anyways
        {
            float damageToApply = damage.Amount * (100 / (100 + PhysicalDefense));
            Debug.Log("Damage to apply before rounding: " + damageToApply);

           // float roundedValue = Mathf.RoundToInt
            //Debug.Log("Damage after rounding: " + roundedValue);
            //CurrentHealth -= damageToApply;
            //Debug.Log("Turret HP: " + CurrentHealth + "/" + MaxHealth);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(this.transform.position, SearchRadius);
    }

    private void Update()
    {
        switch(TurretState)
        {
            case TurretState.Idle:
            {
                //Has anyone stood in the space long enough to activate this turret?
                Collider2D[] colliders = Physics2D.OverlapCircleAll(this.transform.position, SearchRadius /*layerMask (Player)*/);
                
                List<GameObject> playerObjects = new List<GameObject>();
                for(int i = 0; i < colliders.Length; i++)
                {
                    if(colliders[i].tag == "Player")
                    {
                        playerObjects.Add(colliders[i].gameObject);
                        ProgressLabel.gameObject.SetActive(true);
                    }
                }

                if(playerObjects.Count > 1)
                {
                    ProgressLabel.color = Color.gray;
                    ProgressLabel.text = "Contested!";
                } 
                else if(playerObjects.Count > 0)
                {
                    persuasion += captureInterval * Time.deltaTime;       

                    ProgressLabel.color = Color.yellow;
                    ProgressLabel.text = "Malephar Capturing (" + (int)persuasion + "%)";
  
                    if(persuasion >= MAX_PERSUASION)
                    {
                        Friendly = true;
                        TurretState = TurretState.Active;
                        ProgressLabel.color = Color.blue;
                        ProgressLabel.text = "Malephar's Turret (100 health)";
                    }
                }
                else
                {
                    if(persuasion - (captureInterval * Time.deltaTime) > 0)
                    {
                        persuasion -= captureInterval * Time.deltaTime;
                        ProgressLabel.color = (persuasion % 2 == 0) ? Color.white : Color.red;
                        ProgressLabel.text = "Malephar Losing (" + (int)persuasion + "%)";
                    } 
                    else if (persuasion > 0)
                    {
                        ProgressLabel.gameObject.SetActive(false);
                        persuasion = 0;
                    }
                }
            } break;

            case TurretState.Active:
            {
                Debug.Log("Turret is Friendly?: " + Friendly);
                //Is there anyone standing in attack radius?
                //If so, are they friendly?
                //If not, find a target and run contested check
            } break;

            case TurretState.Shooting:
            {
                //Check for new target
                //Check attack speed timer, attack current target if cooled down
            } break;

            case TurretState.Broken:
            {
                //REPAIR KITS
            } break;
        }

        //Debug.Log("Turret State: " + TurretState);
    }
}
