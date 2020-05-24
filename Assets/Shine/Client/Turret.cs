using UnityEngine;
using System.Collections.Generic;
using TMPro;

//Repair kit

public enum TurretState {
    Idle,
    Captured,
    Broken
}

public class Turret : MonoBehaviour, IDamageable
{
    public float CurrentHealth;
    public float MaxHealth;
    public float PhysicalDamage;
    public float MagicalDefense;
    public float PhysicalDefense;

    public float CaptureInterval;
    public float SearchRadius;

    public EntityType EntityType;
    public TurretState TurretState;
    public TextMeshPro ProgressLabel;
    public Transform ShootPosition;

    private GameObject whosCapturingMe;
    private const int MAX_PERSUASION = 100;
    private float persuasion = 0f;

    void Awake()
    {
        CurrentHealth = 100;
        MaxHealth = 100;
        PhysicalDefense = 50;
        EntityType = EntityType.Turret;

        ProgressLabel = GetComponentInChildren<TextMeshPro>();
        ProgressLabel.gameObject.SetActive(false);

        whosCapturingMe = null;

        Debug.Log("Turret");
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
        Gizmos.DrawWireSphere(this.transform.position, SearchRadius);
    }

    private void Update()
    {
        switch(TurretState)
        {
            case TurretState.Idle:
            {
                //Has anyone stood in the space long enough to activate this turret?
                Collider[] colliders = Physics.OverlapSphere(this.transform.position, SearchRadius /*layerMask (Player)*/);
                
                List<GameObject> playerObjects = new List<GameObject>();
                
                for(int i = 0; i < colliders.Length; i++)
                {
                    if (colliders[i].tag == "Player" || colliders[i].tag == "Enemy")
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
                    persuasion += CaptureInterval * Time.deltaTime;  
                    ProgressLabel.color = Color.yellow;
                    
                    playerObjects.ForEach(delegate (GameObject obj)
                    {
                        if (whosCapturingMe && obj.name != whosCapturingMe.name)
                        { 
                            persuasion = 0;
                        }

                        whosCapturingMe = obj;
                        ProgressLabel.text = obj.name + " Capturing (" + (int)persuasion + "%)";
                    });
    
                    if(persuasion >= MAX_PERSUASION)
                    {
                        TurretState = TurretState.Captured;
                        ProgressLabel.color = Color.cyan;
                        ProgressLabel.text = whosCapturingMe.name + " Captured (HP:100)";
                    }
                }
                else
                {
                    if(persuasion - (CaptureInterval * Time.deltaTime) > 0)
                    {
                        persuasion -= CaptureInterval * Time.deltaTime;
                        ProgressLabel.color = (persuasion % 2 == 0) ? Color.white : Color.red;
                        ProgressLabel.text = whosCapturingMe.name + " Losing (" + (int)persuasion + "%)";
                    }
                    else if (persuasion > 0)
                    {
                        ProgressLabel.gameObject.SetActive(false);
                        persuasion = 0;
                    }
                }
            } break;

            case TurretState.Captured:
            {
                Collider[] colliders = Physics.OverlapSphere(this.transform.position, SearchRadius);

                for (int i = 0; i < colliders.Length; i++)
                {
                    // Only shoot if its the other person who captured me
                    if (colliders[i].name != whosCapturingMe.name)
                    {
                        // Unity Vector cookbook shows hwo to truly calculate distance between vectors using a 'heading'
                        Vector2 heading = colliders[i].gameObject.transform.position - ShootPosition.transform.position;
                        float distance = heading.magnitude;
                        Vector2 direction = heading / distance;
                        
                        RaycastHit2D rayInfo = Physics2D.Raycast(this.ShootPosition.transform.position, direction, SearchRadius);
                        if (rayInfo)
                        {
                            Debug.Log("Hit: " + rayInfo.transform.name);
                        }
                        break;
                    }
                }
            } break;

            case TurretState.Broken:
            {
                //REPAIR KITS
            } break;
        }
    }
}
