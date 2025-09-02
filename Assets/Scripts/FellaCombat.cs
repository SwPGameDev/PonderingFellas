using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FellaCombat : MonoBehaviour
{
    public FellaBehavior fellaBehavior;

    // Stats
    public GameObject target;
    public Team.TeamID currentTeam;
    public float damage;
    public float attackCooldown;
    public float _attackTimer = 0;
    public float range;
    public bool inRange;
    public bool canAttack = true;

    public float distanceToTarget;

    public float maxHP;
    public float currentHP;

    public Collider hitBox;


    // Animation
    public GameObject weapon;
    public Animator anim;

    private void Start()
    {
        currentHP = maxHP;
        
    }

    private void Update()
    {
        if (fellaBehavior.dead == true)
        {
            StopAllCoroutines();
            //anim.SetTrigger("Death");
        }

        if (target != null)
        {
            distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

            if (canAttack && distanceToTarget <= range)
            {
                Attack();
            }
            
        }

        if (!canAttack)
        {
            if (_attackTimer < attackCooldown)
            {
                _attackTimer += Time.deltaTime;
            }
            else
            {
                _attackTimer = 0;
                canAttack = true;
            }

        }


    }



    void Attack()
    {
        anim.SetTrigger("Attack");
        StartCoroutine(DelayedHit());
        canAttack = false;
    }

    IEnumerator DelayedHit()
    {
        yield return new WaitForSeconds(attackCooldown * 0.5f);
        List<Collider> hitColliders = hitBox.GetComponent<HitboxOverlap>().heldColliders;

        foreach (Collider collider in hitColliders)
        {
            collider.GetComponent<FellaCombat>().TakeHit(damage);
        }
    }

    public void TakeHit(float damageParam)
    {
        currentHP -= damageParam;
        if (currentHP <= 0)
        {
            currentHP = 0;
            fellaBehavior.Die();
        }
    }

    public void TakeHeal(float healParam)
    {

    }
}
