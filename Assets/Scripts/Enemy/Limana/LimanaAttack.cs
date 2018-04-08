using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FPSDemo.Scripts.Enemy.Limana
{
    public class LimanaAttack : EnemyAttack
    {
        [Header("Sound/Particle Effects")]
        [SerializeField]
        protected AudioClip attackClip;

        [Header("Values")]
        [SerializeField]
        protected float minAccelerationToAttack = 10f;

        void OnCollisionEnter(Collision other)
        {
            if (other.collider.gameObject == player)
            {
                playerInRange = true;
            }
        }


        void OnCollisionExit(Collision other)
        {
            if (other.collider.gameObject == player)
            {
                playerInRange = false;
            }
        }

        void FixedUpdate()
        {
            //Player is alive, Limana is alive, acceleration is > minimum
            if (playerInRange && enemyHealth.CurrentHealth > 0 
                && playerHealth.currentHealth > 0 && nav.acceleration > 
                minAccelerationToAttack)
            {
                enemyAudioLoop.Stop();
                if (!enemyAudioEffects.isPlaying)
                    enemyAudioEffects.PlayOneShot(attackClip);
                //Charge on the player!
                if (!playerHealth.isBeingCharged)
                    playerHealth.TakeCharge(attackDamage);

            }

        }
    }
}


