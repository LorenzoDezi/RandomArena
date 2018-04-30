using FPSDemo.Scripts.Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace FPSDemo.Scripts.Enemy.Remy
{
    public class RemyMovement : EnemyMovement
    {
        [HideInInspector]
        public bool isAttacking;
        [HideInInspector]
        public bool hasAttacked;
        bool isRunningOut;
        Vector3 runDestination;

        Transform playerTransform;
        //the distance of the point to run to after the attack
        [SerializeField]
        [Tooltip("The distance of the point at which run after the attack")]
        float distanceToRun;

        protected override void Start()
        {
            playerTransform = playerHealth.transform;
            isAttacking = true;
            hasAttacked = false;
            isRunningOut = false;
        }

        protected override void Update()
        {

            if (isAttacking && playerHealth.currentHealth > 0 && enemyHealth.CurrentHealth > 0)
            {
                //Attacking the player
                nav.SetDestination(playerTransform.position);
                anim.SetFloat("Speed", 1);
            }
            else if (playerHealth.currentHealth > 0 && enemyHealth.CurrentHealth > 0)
            {
                if (hasAttacked && !isRunningOut)
                {
                    NavMeshHit hit;
                    if (NavMesh.SamplePosition(Random.insideUnitSphere * distanceToRun, out hit, 10,
                        (int)Mathf.Pow(2, LevelManager.Manager.NavMeshIndex)))
                    {
                        //After attack, Remy runs away in a random position far "distanceToRun" value
                        //from the current position
                        runDestination = hit.position;
                        nav.SetDestination(runDestination);
                        isRunningOut = true;
                        hasAttacked = false;
                    }
                    else
                    {
                        //If Remy can't run away, it attacks again
                        nav.SetDestination(playerTransform.position);
                        isAttacking = true;
                        hasAttacked = false;
                    }
                }
                else
                {
                    //Remy has run out and it is ready to attack the player again
                    if (Vector3.Distance(transform.position, runDestination) < 2)
                    {
                        isRunningOut = false;
                        isAttacking = true;
                    }
                }
            }
            else
            {
                //Either the player or Remy dies, Remy stopped
                nav.isStopped = true;
                anim.SetFloat("Speed", 0);
            }

        }
    }
}


