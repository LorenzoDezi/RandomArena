using UnityEngine;
using System.Collections;
using System;
using FPSDemo.Scripts.Player;
using FPSDemo.Scripts.Manager;

namespace FPSDemo.Scripts.Enemy
{
    public class EnemyMovement : MonoBehaviour
    {
        //Used to vary enemy speed, add randomness. TESTARE
        protected float IntervalFrenzySeconds = 3f;
        protected bool isFrenzy = false;
        protected float frenzyTimer = 0f;

        //Various elements
        protected Transform player;
        protected HealthController playerHealth;
        protected EnemyHealth enemyHealth;
        protected UnityEngine.AI.NavMeshAgent nav;

        //AudioSource for effects
        protected AudioSource enemyAudio1;
        protected AudioSource enemyAudio2;

        [Header("Animator")]
        [SerializeField]
        protected Animator anim;
        
        //Variable to handle target approach
        protected float currentDistanceApproach;
        [SerializeField]
        protected float firstApproachDistance = 10f;
        [SerializeField]
        protected float secondApproachDistance = 5f;
        protected bool[] approachStages = { false, false };


        void Awake()
        {
            player = GameObject.Find("MainPlayer").GetComponent<Transform>();
            enemyAudio1 = GetComponents<AudioSource>()[0];
            enemyAudio2 = GetComponents<AudioSource>()[1];
            playerHealth = player.GetComponent<HealthController>();
            enemyHealth = GetComponent<EnemyHealth>();
            nav = GetComponent<UnityEngine.AI.NavMeshAgent>();
            nav.updateRotation = true;
            anim = GetComponent<Animator>();
            if(anim == null)
            {
                //Fix for the poor limana model
                anim = GetComponentInChildren<Animator>();
            }
            currentDistanceApproach = 0f;
        }

        protected virtual void Start()
        {
            enemyAudio2.Play();
            if(anim != null)
                anim.SetFloat("Speed", 1);

        }

        /// <summary>
        /// Handle alterning speed
        /// </summary>
        protected void HandleFrenzy()
        {
            frenzyTimer += Time.deltaTime;
            if (frenzyTimer > IntervalFrenzySeconds)
            {
                //Alternates speed
                if (isFrenzy)
                {
                    nav.speed -= 10f;
                    isFrenzy = false;
                    anim.speed = 2f;
                }
                else
                {
                    nav.speed += 10f;
                    isFrenzy = true;
                    anim.speed = 3f;
                }
                frenzyTimer = 0;
            }
        }


        protected virtual void Update()
        {

            HandleFrenzy();

            if (!enemyHealth.IsDead() && playerHealth.currentHealth > 0)
            {
                //Enemy and player alive
                float distanceFromPlayer = Vector3.Distance(transform.position, player.position);
                if (distanceFromPlayer > firstApproachDistance)
                {
                    //Far distance approach
                    if (!approachStages[0])
                    {
                        //Initializing approach values, and setting state
                        currentDistanceApproach = UnityEngine.Random.Range(-firstApproachDistance, 
                            firstApproachDistance);
                        approachStages[0] = true;
                        approachStages[1] = false;
                    }
                    FlankApproach(currentDistanceApproach);
                }
                else if (distanceFromPlayer > secondApproachDistance)
                {
                    //Medium distance approach
                    if (!approachStages[1])
                    {
                        //Initializing approach values, and setting state
                        currentDistanceApproach = UnityEngine.Random.Range(-secondApproachDistance,
                            secondApproachDistance);
                        approachStages[1] = true;
                        approachStages[0] = false;
                    }
                    FlankApproach(currentDistanceApproach);
                }
                else
                {
                    //Short distance approach - attack!
                    approachStages[0] = false;
                    approachStages[1] = false;
                    nav.SetDestination(player.position);
                }
            }
            else
            {
                //No movement - enemy or player dead
                enemyAudio2.Stop();
                if (!nav.isStopped)
                    nav.isStopped = true;
                anim.SetFloat("Speed", 0f);
            }

        }


        /// <summary>
        /// Flank the current player position
        /// </summary>
        /// <param name="approachVariance">Variance used in the approach</param>
        private void FlankApproach(float approachVariance)
        {
            //Setting approachPoint
            Vector3 approachPoint = player.position;
            approachPoint.x += approachVariance;
            approachPoint.y = 0f;

            //Verifying the navMesh hit
            UnityEngine.AI.NavMeshHit nearHitOnNavMesh;
            if (UnityEngine.AI.NavMesh.SamplePosition(approachPoint, out nearHitOnNavMesh,
                5f, (int)Mathf.Pow(2, LevelManager.Manager.NavMeshIndex)))
                approachPoint = nearHitOnNavMesh.position;

            //Approaching
            nav.SetDestination(approachPoint);
        }
    }
}


