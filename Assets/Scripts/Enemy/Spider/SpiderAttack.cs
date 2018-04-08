using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FPSDemo.Scripts.Enemy
{
    public class SpiderAttack : EnemyAttack
    {
        [Header("Sound/Particle Effects")]
        [SerializeField]
        protected AudioClip attackClip;

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == player)
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
                    enemyAudioEffects.PlayOneShot(attackClip, 0.5f);
                anim.SetTrigger("Attack");
                playerInRange = false;
            }
            else if (enemyHealth.CurrentHealth > 0)
            {
                if (!enemyAudioLoop.isPlaying)
                    enemyAudioLoop.Play();
            }
        }
    }
}


