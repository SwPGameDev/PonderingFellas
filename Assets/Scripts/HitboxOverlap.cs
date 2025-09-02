using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HitboxOverlap : MonoBehaviour
{
    public List<Collider> heldColliders;
    public LayerMask layerMask;

    InputAction action;

    private void Start()
    {
        action = InputSystem.actions.FindAction("Interact");
    }

    private void Update()
    {
        if (action.WasPressedThisFrame())
        {
            List<Collider> colliders = GetHitColliders();
            foreach(Collider col in colliders)
            {
                Debug.Log(col.name);
            }
        }
    }

    private void FixedUpdate()
    {
        heldColliders.Clear();
    }

    private void OnTriggerStay(Collider other)
    {
        if ((layerMask.value & (1 << other.transform.gameObject.layer)) > 0)
        {
            heldColliders.Add(other);
        }
    }


    public List<Collider> GetHitColliders()
    {
        List<Collider> colliders = heldColliders;
        return colliders;
    }
}