using UnityEngine;

public class ProjectileBehavior : MonoBehaviour
{
    public float damage;
    public LayerMask layerMask;
    public Team.TeamID teamToHit;

    Collider col;
    Rigidbody rb;

    private void Start()
    {
        col = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();

    }

    private void FixedUpdate()
    {
        // Raycast fixedupdate tick * flyspeed
        // Check hit obstacles or enemy
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check do hit
        // Stick into collider at contact point
    }


}
