using FPSDemo.Scripts.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FPSDemo.Scripts.Enemy.Zombie
{
    public class ZombieHealth : EnemyHealth
    {
        [Header("Sound/Particle Effects")]
        [SerializeField]
        private AudioClip[] deathClips;

        public override void TakeDamage(int amount, Vector3 hitPoint, Boolean isShocked = false)
        {
            if (isDead)
                return;
            CurrentHealth -= amount;

            if (!isShocked)
            {
                ParticleSystem bloodSplat = GameObject.Instantiate(
                    this.bloodSplat, hitPoint, 
                    Quaternion.FromToRotation(hitPoint.normalized, 
                    Vector3.right));
                bloodSplat.Play();
                Destroy(bloodSplat.gameObject, 2f);
            }

            if (CurrentHealth <= 0)
            {
                Death();
            }
        }
    }

}

