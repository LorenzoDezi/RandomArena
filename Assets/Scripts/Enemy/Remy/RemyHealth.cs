using FPSDemo.Scripts.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPSDemo.Scripts.Enemy.Remy
{
    public class RemyHealth : EnemyHealth
    {
        [Header("Sound/Particle Effects")]
        [SerializeField]
        private AudioClip[] deathClips;

        public override void TakeDamage(int amount, Vector3 hitPoint, Boolean isShocked)
        {
            if (isDead)
                return;

            CurrentHealth -= amount;

            if (!enemyAudioEffects.isPlaying)
                enemyAudioEffects.PlayOneShot(damageClip, 0.5f);
            if (!isShocked)
            {
                ParticleSystem bloodSplat = GameObject.Instantiate(this.bloodSplat, hitPoint, Quaternion.FromToRotation(hitPoint.normalized, Vector3.right));
                bloodSplat.Play();
                Destroy(bloodSplat.gameObject, 2f);
            }

            if (CurrentHealth <= 0)
            {
                Death();
            }
        }

        protected override void Death()
        {
            isDead = true;
            anim.SetTrigger("Dead");
            enemyAudioEffects.PlayOneShot(deathClips[UnityEngine.Random.Range(0, deathClips.Length - 1)]);
            ScoreManager.manager.ScoreIncrease(Convert.ToInt32(ScoreValue * ScoreManager.manager.scoreMultiplier));
            ScoreManager.manager.enemyKilled++;
        }
    }
}


