using FPSDemo.Scripts.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.AI;

namespace FPSDemo.Scripts.Enemy.Limana
{
    public class LimanaMovement : EnemyMovement
    {
        bool playerReached = false;
        [Header("Values")]
        [SerializeField]
        float distancePastThePlayer;
        [SerializeField]
        float approachingAcceleration = 15f;
        [SerializeField]
        float returningAcceleration = 10f;

        Vector3 destinationPastThePlayer;

        protected override void Start()
        {
            base.Start();
            //Limana never stops until death of player/limana itself
            anim.SetFloat("Speed", 1);
        }

        protected override void Update()
        {
            //Enemy and player alive
            if (!enemyHealth.IsDead() && playerHealth.currentHealth > 0)
            {
                anim.SetFloat("Direction", this.transform.TransformPoint(player.position).normalized.z);
                //Still trying to reach the player
                if (!playerReached)
                {
                    nav.acceleration = approachingAcceleration;
                    nav.SetDestination(player.position);
                    StartCoroutine(NextDestination(true));
                }
                else if (playerReached)
                {
                    //We simulate too much acceleration for limana, so its next destination should be 
                    //past the player, and then it can follow the player again
                    if (!nav.hasPath)
                    {
                        nav.speed = returningAcceleration;
                        NavMeshHit nearHitOnNavMesh;
                        if (NavMesh.SamplePosition(transform.position + transform.forward * distancePastThePlayer, 
                            out nearHitOnNavMesh, 10f,
                           (int)Mathf.Pow(2, LevelManager.Manager.NavMeshIndex)))
                        {
                            //Reaching the destination past the player
                            Debug.Log("Going past the player");
                            destinationPastThePlayer = nearHitOnNavMesh.position;
                            nav.SetDestination(destinationPastThePlayer);
                        }
                        else
                        {
                            //Attacking the player again
                            nav.SetDestination(player.position);
                        }

                    }
                    StartCoroutine(NextDestination(false));
                }
            }
            else
            {
                nav.isStopped = true;
                anim.SetFloat("Speed", -1);
            }
        }

        /// <summary>
        /// What is the next destination, player or not? Handling navigation path - calculation
        /// </summary>
        /// <param name="isAttackingDirectlyThePlayer"></param>
        /// <returns></returns>
        private IEnumerator NextDestination(bool isAttackingDirectlyThePlayer)
        {
            if (nav.pathPending)
                yield return null;
            if (nav.remainingDistance < 5)
            {
                if (isAttackingDirectlyThePlayer)
                {
                    playerReached = true;
                }
                else
                {
                    playerReached = false;
                }
                nav.ResetPath();
            }
            yield return null;
        }

    }
}



