using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEditor.UIElements;

public class FellaBehavior : MonoBehaviour
{
    # region DEBUG
    public bool debug;

    private bool lastDebug;

    public GameObject debugLabels;
    public TextMeshPro stateLabel;
    public TextMeshPro HPLabel;
    public TextMeshPro StaminaLabel;
    public TextMeshPro SpeedLabel;

    public GameObject debugTarget;
    #endregion

    public enum FellaStates
    {
        Idle,
        MovingToRange,
        InRange,
        Marching,
        Wandering
    }

    public FellaStates currentState = FellaStates.Idle;

    private FellaCombat fellaCombat;
    private FellaMovement fellaMovement;

    private Rigidbody rb;
    private CapsuleCollider col;

    #region STATS

    [Header("Stats")]
    public Team.TeamID currentTeam = Team.TeamID.None;

    public LayerMask colLayerCheckMask;
    public float maxHP;
    public float visionRange;
    public float visionFOV;
    public float damage;
    public float attackCooldown;
    public float attackRange;
    public bool melee;

    // MOVEMENT
    public float rotateSpeed;

    public float moveSpeed;

    public float maxStamina;

    public float sinkDelay;
    #endregion

    [Space]
    public bool aggroed = false;

    public GameObject target;
    public Vector3 destination;
    public float distanceToTarget;

    public float targetCheckCooldown;
    public float _targetCheckTimer = 0;

    public bool dead;

    private void Awake()
    {
        fellaCombat = GetComponent<FellaCombat>();
        fellaMovement = GetComponent<FellaMovement>();

        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();

        fellaCombat.fellaBehavior = this;
        fellaMovement.fellaBehavior = this;

        ApplyStats();
    }

    private void Update()
    {
        if (dead == false)
        {
            #region Manual Testing Control
            if (FellaController.Instance.controlBoth)
            {
                currentState = FellaController.Instance.bothState;
            }
            else
            {
                if (currentTeam == Team.TeamID.Blue)
                {
                    currentState = FellaController.Instance.blueState;
                }
                else if (currentTeam == Team.TeamID.Red)
                {
                    currentState = FellaController.Instance.redState;
                }
            }
            #endregion

            // Behavior State
            // Move to point
            // Chill
            // Get in range

            // Order
            // Move
            // Attack
            // Idle
            // Wander


            if (target != null)
            {
                distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

                if (distanceToTarget > fellaCombat.range)
                {
                    fellaMovement.SetTarget(target);
                }
                else if (distanceToTarget <= fellaCombat.range)
                {
                    fellaMovement.SetDestination(transform.position);
                    fellaMovement.SetTarget(null);
                }




                //Rotate to look at target
                if (distanceToTarget < fellaCombat.range)
                {
                    Vector3 relativePos = target.transform.position - transform.position;
                    Quaternion lookRotation = Quaternion.LookRotation(relativePos.normalized, Vector3.up);

                    Vector3 euler = lookRotation.eulerAngles;
                    euler.x = 0f;
                    euler.z = 0f;

                    Quaternion targetRotation = Quaternion.Euler(euler);

                    transform.localRotation = Quaternion.RotateTowards(transform.localRotation, targetRotation, rotateSpeed * Time.deltaTime);
                    //
                }

                if (target.GetComponent<FellaBehavior>().dead == true)
                {
                    target = null;
                    fellaCombat.target = null;
                }
                else
                {
                    if (_targetCheckTimer < targetCheckCooldown)
                    {
                        _targetCheckTimer += Time.deltaTime;
                    }
                    else
                    {
                        _targetCheckTimer = 0;
                        fellaCombat.target = target;
                    }
                }
            }
            else
            {
                // Target check

                if (_targetCheckTimer < targetCheckCooldown)
                {
                    _targetCheckTimer += Time.deltaTime;
                }
                else
                {
                    _targetCheckTimer = 0;
                    //Debug.Log("Nearby Enemies: " + GetNearbyEnemies(visionRange, colLayerCheckMask));

                    //Debug.Log("Sorted in FOV: " + SortArrayIfInFOV(GetNearbyEnemies(visionRange, colLayerCheckMask)));

                    //Debug.Log("Closest Enemy: " + GetClosestFromList(SortArrayIfInFOV(GetNearbyEnemies(visionRange, colLayerCheckMask))));

                    GameObject closestEnemy = GetClosestFromList(SortArrayIfInFOV(GetNearbyEnemies(visionRange, colLayerCheckMask)));
                    if (closestEnemy != null)
                    {
                        target = closestEnemy;
                    }
                }
            }
        }

        if (lastDebug != debug)
        {
            debugLabels.SetActive(debug);
        }
        lastDebug = debug;
    }

    private void ApplyStats()
    {
        fellaCombat.currentTeam = currentTeam;
        fellaCombat.maxHP = maxHP;
        fellaCombat.range = attackRange;
        fellaCombat.damage = damage;
        fellaCombat.attackCooldown = attackCooldown;
        fellaCombat.melee = melee;

        fellaMovement.speed = moveSpeed;
        fellaMovement.maxStamina = maxStamina;

        //float visionRange;
        //float visionFOV;
    }

    private List<Collider> GetNearbyEnemies(float radiusParam, LayerMask layerMaskParam)
    {
        int maxColliders = 100;
        Collider[] cols = new Collider[maxColliders];

        List<Collider> nearby = new();

        int numEnemiesInRange = Physics.OverlapSphereNonAlloc(transform.position, radiusParam, cols, layerMaskParam);

        if (numEnemiesInRange > 0)
        {
            foreach (Collider col in cols)
            {
                if (col != null && col.gameObject.GetComponent<FellaBehavior>().currentTeam != currentTeam)
                {
                    // DEBUG
                    //Debug.DrawLine(transform.position, col.gameObject.transform.position, Color.yellow, 0.5f);

                    nearby.Add(col);
                }
            }

            return nearby;
        }
        else
        {
            return null;
        }
    }

    private List<Collider> SortArrayIfInFOV(List<Collider> colsParam)
    {
        List<Collider> sortedCols = new();

        if (colsParam != null && colsParam.Count > 0)
        {
            foreach (Collider col in colsParam)
            {
                float Deg = Mathf.Rad2Deg * Mathf.Acos(Vector3.Dot(transform.forward, (col.transform.position - transform.position).normalized));
                //Debug.Log("Deg: " + Deg);

                if (Deg < visionFOV)
                {
                    // DEBUG
                    //Debug.DrawLine(transform.position, col.transform.position, Color.green, 0.5f);

                    sortedCols.Add(col);
                }
                else
                {
                    // DEBUG
                    //Debug.DrawLine(transform.position, col.transform.position, color: Color.red, 0.5f);
                }
            }
            return sortedCols;
        }
        return null;
    }

    private GameObject GetClosestFromList(List<Collider> colsParam)
    {
        GameObject closest = null;
        float lowestDistance = Mathf.Infinity;

        if (colsParam != null && colsParam.Count > 0)
        {
            foreach (Collider col in colsParam)
            {
                float distance = Vector3.Distance(transform.position, col.transform.position);
                if (distance < lowestDistance)
                {
                    lowestDistance = distance;
                    closest = col.gameObject;
                }
            }
            // DEBUG
            //Debug.DrawLine(transform.position, closest.transform.position, Color.cyan, 0.5f);

            return closest;
        }
        else
        {
            return null;
        }
    }

    private GameObject GetClosestEnemy(float radiusParam, LayerMask layerMaskParam)
    {
        int maxColliders = 25;
        Collider[] cols = new Collider[maxColliders];

        int numEnemiesInRange = Physics.OverlapSphereNonAlloc(transform.position, radiusParam, cols, layerMaskParam);

        GameObject closest = null;
        float lowestDistance = Mathf.Infinity;

        if (numEnemiesInRange > 0)
        {
            foreach (Collider col in cols)
            {
                if (col.GetComponent<FellaBehavior>().currentTeam != currentTeam)
                {
                    float Deg = Mathf.Rad2Deg * Mathf.Acos(Vector3.Dot(transform.forward, (col.transform.position - transform.position).normalized));
                    Debug.Log("Deg: " + Deg);

                    if (Deg < visionFOV)
                    {
                        float distance = Vector3.Distance(transform.position, col.transform.position);
                        if (distance < lowestDistance)
                        {
                            lowestDistance = distance;
                            closest = col.gameObject;
                        }
                    }
                }
            }
            return closest;
        }
        else
        {
            return null;
        }
    }

    public void Die()
    {
        dead = true;
        fellaCombat.enabled = false;
        fellaMovement.enabled = false;
        fellaMovement.navAgent.enabled = false;
        gameObject.layer = 0;
        rb.isKinematic = false;
        rb.AddForce((transform.up * 5) + Random.onUnitSphere, ForceMode.Impulse);

        col.height *= 0.5f;
        col.radius *= 0.2f;

        StartCoroutine(DelayedSink(sinkDelay));
    }

    private IEnumerator DelayedSink(float delayParam)
    {
        yield return new WaitForSeconds(delayParam);
        col.isTrigger = true;
        rb.isKinematic = true;

        float goalY = transform.position.y - 5;

        while (transform.position.y > goalY)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, goalY, transform.position.z), 1 * Time.deltaTime);
            yield return null;
        }

        Destroy(gameObject, 0.1f);
    }

    private void OnDrawGizmos()
    {
        if (debug)
        {
            // MOVEMENT

            if (fellaMovement)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, fellaMovement.navAgent.destination);
            }

            // FOV

            Gizmos.color = new Color(1, 0, 0, 0.25f);
            Gizmos.DrawLine(transform.position, transform.position + Quaternion.AngleAxis(visionFOV / 2, Vector3.up) * transform.forward * visionRange);

            Gizmos.color = new Color(1, 0, 0, 0.25f);
            Gizmos.DrawLine(transform.position, transform.position + Quaternion.AngleAxis(-1 * visionFOV / 2, Vector3.up) * transform.forward * visionRange);

            // COMBAT

            Gizmos.color = new Color(0, 0.55f, 0.55f, 0.25f);
            Gizmos.DrawWireSphere(transform.position, attackRange);

            Gizmos.color = new Color(0.85f, 0.6f, 0.2f, 0.25f);
            Gizmos.DrawWireSphere(transform.position, visionRange);

            if (target)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, target.transform.position);
            }

            // Labels

            stateLabel.text = currentState.ToString();
            if (fellaCombat != null && fellaMovement != null)
            {
                HPLabel.text = (fellaCombat.currentHP + " / " + maxHP).ToString();
                StaminaLabel.text = (fellaMovement.currentStamina + " / " + maxStamina).ToString();
            }
            if (rb != null)
            {
                SpeedLabel.text = rb.linearVelocity.magnitude.ToString();
            }
        }
    }
}