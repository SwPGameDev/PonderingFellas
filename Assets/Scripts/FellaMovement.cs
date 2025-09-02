using UnityEngine;
using UnityEngine.AI;

public class FellaMovement : MonoBehaviour
{
    public FellaBehavior fellaBehavior;

    public NavMeshAgent navAgent;
    public Vector3 Destination { get; private set; }
    public GameObject Target { get; private set; }

    public bool refreshDest = true;
    public float destCooldown = 0.35f;
    public float _destTimer = 0;
    public float maxStamina;
    public float currentStamina;

    public float speed;

    [Header("Animations")]
    // Mesh
    public GameObject mesh;
    public float bounceAmplitude;
    public float bounceSpeed;
    public float heightOffset;
    float randOffset;
    float distanceToDest;

    private void Awake()
    {
        navAgent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        randOffset = Random.Range(-1f, 1f);
        navAgent.speed = speed;
    }

    void Update()
    {
        if (refreshDest)
        {
            if (_destTimer < destCooldown)
            {
                _destTimer += Time.deltaTime;
            }
            else
            {
                _destTimer = 0;
                TryRefreshDest();
            }
        }

        if (Destination != null)
        {
            distanceToDest = Vector3.Distance(transform.position, Destination);
        }


        // Animations
        if (distanceToDest <= 1.5f)
        {
            //idle animation
            mesh.transform.localPosition = Vector3.MoveTowards(mesh.transform.localPosition, new Vector3(0, heightOffset, 0), Time.deltaTime * 1);
        }
        else if (distanceToDest <= 3f)
        {
            mesh.transform.localPosition = new(0, Mathf.Abs(Mathf.Sin((Time.time + randOffset) * bounceSpeed) * (bounceAmplitude / 2)) + heightOffset, 0);
        }
        else
        {
            mesh.transform.localPosition = new(0, Mathf.Abs(Mathf.Sin((Time.time + randOffset) * bounceSpeed) * bounceAmplitude) + heightOffset, 0);
        }
    }

    public void SetTarget(GameObject newTarget)
    {
        Target = newTarget;
    }

    public void SetDestination(Vector3 newDest)
    {
        Destination = newDest;
    }

    public void TryRefreshDest()
    {
        if (Target != null)
        {
            navAgent.destination = Target.transform.position;
        }
        else
        {
            navAgent.destination = Destination;
        }
    }
}
