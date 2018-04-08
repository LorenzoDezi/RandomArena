using FPSDemo.Scripts.Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPSDemo.Scripts.Enemy.MocapGuy
{
    public class MocapGuyMovement : EnemyMovement
    {
        [Header("Values")]
        [SerializeField]
        float attackDistance;
        [SerializeField]
        float attackTolerance;
        [SerializeField]
        float playerMovementTolerance;

        //The path. The mocapGuy follow this path during player surrounding
        Vector3[] path;
        //Path variables needed
        int nextPathPositionIndex;
        Vector3 previousPlayerPosition;
        float constantY;
        bool isAttacking = false;
        bool isReachingThePlayer = false;


        protected override void Start()
        {
            //Path setting
            constantY = this.transform.position.y;
            //The path build a square around the player, vertices are the positions to reach
            //MocapGuy attacks at each vertice of the square
            path = new Vector3[4];
            path[0] = new Vector3(player.position.x, constantY, player.position.z - attackDistance);
            path[1] = new Vector3(path[0].x + attackDistance, constantY, path[0].z);
            path[2] = new Vector3(player.position.x, constantY, player.position.z + attackDistance);
            path[3] = new Vector3(path[0].x - attackDistance, constantY, path[0].z);
            AdjustPath(path);

            //Adjusting various values
            base.Start();
            previousPlayerPosition = player.position;
            nextPathPositionIndex = 0;
            anim.speed = 1f;
            nav.updateRotation = true;
        }

        protected override void Update()
        {
            //Player alive, in movement
            if (Vector3.Distance(this.transform.position, player.transform.position) <= attackTolerance
                && !isAttacking
                && !enemyHealth.IsDead()
                && playerHealth.currentHealth > 0)
            {
                //Update the path only if the movement of the player is past a certain tolerance
                if (Vector3.Distance(previousPlayerPosition, player.position) > playerMovementTolerance 
                    || isReachingThePlayer)
                {
                    //Updating path
                    float diffX = player.position.x - previousPlayerPosition.x;
                    float diffZ = player.position.z - previousPlayerPosition.z;
                    UpdatePath(path, diffX, diffZ);
                    AdjustPath(path);

                    previousPlayerPosition = player.position;
                    //The next position to reach is determined by the minimum distance
                    float minDistance = float.MaxValue;
                    for (int i = 0; i < path.Length; i++)
                    {
                        float currentPointDistance = Vector3.Distance(this.transform.position, path[i]);
                        if (minDistance > currentPointDistance)
                        {
                            minDistance = currentPointDistance;
                            nextPathPositionIndex = i;
                        }
                    }
                    isReachingThePlayer = false;
                    nav.SetDestination(path[nextPathPositionIndex]);
                }

                //The path position is reached, than attack
                if (Vector3.Distance(transform.position, nav.destination) < 3)
                {
                    transform.LookAt(player.position);
                    anim.SetTrigger("Attack");
                    isAttacking = nav.isStopped = true;

                }

            }
            //MocapGuy is still far from the player
            else if (!isAttacking && !enemyHealth.IsDead() && playerHealth.currentHealth > 0)
            {
                isReachingThePlayer = true;
                nav.SetDestination(player.position);
            }
            //MocapGuy is dead
            else if (enemyHealth.IsDead())
            {
                nav.isStopped = true;
            }
            //During attack, MocapGuy doesn't lose sight of the player!
            else if (isAttacking)
            {
                transform.LookAt(player);
            }
            //The player is dead
            else if (playerHealth.currentHealth <= 0)
            {
                nav.isStopped = true;
                anim.SetFloat("Speed", 0);
            }

        }

        /// <summary>
        /// The MocapGuy has finished to attack. Called at a certain point of the attack animation
        /// </summary>
        public void HasAttacked()
        {
            isAttacking = nav.isStopped = false;
            nextPathPositionIndex = (nextPathPositionIndex + 1) % path.Length;
            nav.SetDestination(path[nextPathPositionIndex]);
        }

        /// <summary>
        /// Assuring path positions is on the navMesh
        /// </summary>
        /// <param name="path">The path to follow</param>
        private void AdjustPath(Vector3[] path)
        {
            for (int i = 0; i < path.Length; i++)
            {
                //We assure that each point is on the navMesh
                UnityEngine.AI.NavMeshHit nearHitOnNavMesh;
                UnityEngine.AI.NavMesh.SamplePosition(path[i], out nearHitOnNavMesh, 10,
                    (int)Mathf.Pow(2, LevelManager.Manager.NavMeshIndex));
                path[i] = nearHitOnNavMesh.position;
            }
        }

        private void UpdatePath(Vector3[] path, float diffX, float diffZ)
        {
            for (int i = 0; i < path.Length; i++)
            {
                //We update the path with the new player position
                path[i].x += diffX;
                path[i].z += diffZ;
            }
        }
    }
}


