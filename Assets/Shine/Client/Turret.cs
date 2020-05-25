using UnityEngine;
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

    public float MaxCaptureTime;
    public float CaptureInterval;
    public float SearchRadius;

    public EntityType EntityType;
    public TurretState TurretState;
    public TextMeshPro ProgressLabel;
    public Transform ShootPosition;
    public Transform CrossbowPivot;

    private GameObject whosCapturingMe;
    private GameObject enemy;
    private const int MAX_PERSUASION = 100;
    private float persuasion = 0f;
    private float captureTimeLeft;


    void Awake()
    {
        EntityType = EntityType.Turret;

        ProgressLabel = GetComponentInChildren<TextMeshPro>();
        ProgressLabel.gameObject.SetActive(false);

        whosCapturingMe = null;
        enemy = null;

        captureTimeLeft = MaxCaptureTime;
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
                Collider[] colliders = Physics.OverlapSphere(this.transform.position, SearchRadius, LayerMask.GetMask("Player"));
                
                for(int i = 0; i < colliders.Length; i++)
                {
                    if (colliders[i].CompareTag("Player") || colliders[i].CompareTag("Enemy"))
                    {
                        ProgressLabel.gameObject.SetActive(true);
                    }
                }

                if(colliders.Length > 1)
                {
                    ProgressLabel.color = Color.yellow;
                    ProgressLabel.text = "Contested!";
                } 
                else if(colliders.Length > 0)
                {
                    persuasion += CaptureInterval * Time.deltaTime;  
                    ProgressLabel.color = Color.green;

                    for (int i = 0; i < colliders.Length; i++)
                    {
                        if (whosCapturingMe && colliders[i].name != whosCapturingMe.name)
                        {
                            persuasion = 0;
                        }

                        whosCapturingMe = colliders[i].gameObject;
                        ProgressLabel.text = colliders[i].name + " Capturing (" + (int)persuasion + "%)";
                    }
                   
                    if(persuasion >= MAX_PERSUASION)
                    {
                        persuasion = 0;
                        captureTimeLeft = MaxCaptureTime;
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
                        persuasion = 0;
                        ProgressLabel.gameObject.SetActive(false);
                    }
                }
            } break;
            
            case TurretState.Captured:
            {
                captureTimeLeft -= Time.deltaTime;

                if (captureTimeLeft > 0)
                {
                    ProgressLabel.text = whosCapturingMe.name + "'s capture duration: " + (int)captureTimeLeft;

                    Collider[] colliders = Physics.OverlapSphere(this.transform.position, SearchRadius, LayerMask.GetMask("Player"));

                    for (int i = 0; i < colliders.Length; i++)
                    {
                        // Only target the person who didnt capture the turret
                        if (colliders[i].name != whosCapturingMe.name)
                        {
                            enemy = colliders[i].gameObject;
                            Vector3 heading = colliders[i].gameObject.transform.position - ShootPosition.transform.position;
                            float distance = heading.magnitude;
                            Vector3 direction = heading / distance;

                            if (Physics.Raycast(ShootPosition.transform.position, direction, out RaycastHit hit, SearchRadius, LayerMask.GetMask("Player", "Terrain")))
                            {
                                //Debug.Log("Found an object: " + hit.transform.name + " - distance: " + hit.distance);
                            }
                            break;
                        }
                    }
                }
                else
                {
                    captureTimeLeft = 0;
                    TurretState = TurretState.Broken;
                    ProgressLabel.gameObject.SetActive(false);
                }
            } break;

            case TurretState.Broken:
            {
                ProgressLabel.gameObject.SetActive(true);
                ProgressLabel.text = enemy.name + " must repair the turret";
            }
            break;
        }
    }
}
