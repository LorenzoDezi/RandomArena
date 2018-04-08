using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FPSDemo.Scripts.Enemy.Zombie
{
    public class ZombieAttack : EnemyAttack
    {
        [Header("Sound/Particle Effects")]
        [SerializeField]
        private AudioClip[] attackClips;

        void OnCollisionEnter(Collision other)
        {
            if (other.collider.gameObject == player)
            {
                playerInRange = true;
            }
        }

        private void OnCollisionStay(Collision collision)
        {
            if (collision.collider.gameObject == player)
            {
                playerInRange = true;
            }
        }

        void FixedUpdate()
        {
            if (playerInRange && enemyHealth.CurrentHealth > 0 && playerHealth.currentHealth > 0)
            {
                enemyAudioLoop.Stop();
                if (!enemyAudioEffects.isPlaying)
                    enemyAudioEffects.PlayOneShot(attackClips[UnityEngine.Random.Range(0, attackClips.Length - 1)]);
                anim.SetTrigger("Attack");
                playerInRange = false;
            }

        }
    }
}


