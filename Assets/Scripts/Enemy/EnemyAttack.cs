using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using FPSDemo.Scripts.Player;

namespace FPSDemo.Scripts.Enemy
{
    /// <summary>
    /// This script handles enemy attack behaviours, and damage values
    /// </summary>
    public abstract class EnemyAttack : MonoBehaviour
    {
        [Header("Values")]
        [SerializeField]
        protected int attackDamage = 10;

        [Header("Animator")]
        [SerializeField]
        protected Animator anim;

        protected GameObject player;
        protected HealthController playerHealth;
        protected AudioSource enemyAudioLoop;
        protected AudioSource enemyAudioEffects;
        protected EnemyHealth enemyHealth;
        protected bool playerInRange;
        protected NavMeshAgent nav;


        void Awake()
        {
            player = GameObject.Find("MainPlayer");
            playerHealth = player.GetComponent<HealthController>();
            enemyHealth = GetComponent<EnemyHealth>();
            anim = GetComponent<Animator>();
            enemyAudioEffects = GetComponents<AudioSource>()[0];
            enemyAudioLoop = GetComponents<AudioSource>()[1];
            nav = GetComponent<NavMeshAgent>();
        }

        /// <summary>
        /// This script handles the attack damage. It is called
        /// at a certain point of the attack animation
        /// </summary>
        protected virtual void Attack()
        {
            //if the player is inside the collider, than it will be affected by the attack
            if (playerInRange)
            {
                playerHealth.TakeDamage(attackDamage);
            }

        }
    }
}


