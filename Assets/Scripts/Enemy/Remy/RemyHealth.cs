﻿using FPSDemo.Scripts.Manager;
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

        public static int SpawnIndex = 2;

        public override void TakeDamage(int amount, Vector3 hitPoint, Boolean isShocked)
        {
            if (isDead)
                return;

            CurrentHealth -= amount;

            if (!enemyAudioEffects.isPlaying)
                enemyAudioEffects.PlayOneShot(damageClip, 0.5f);
            if (!isShocked)
            {
                ParticleSystem bloodSplat = GameObject.Instantiate(this.bloodSplat, hitPoint, 
                    Quaternion.FromToRotation(hitPoint.normalized, Vector3.right), this.transform);
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
            base.Death();
            enemyAudioEffects.PlayOneShot(deathClips[UnityEngine.Random.Range(0, deathClips.Length - 1)]);
        }
    }
}


