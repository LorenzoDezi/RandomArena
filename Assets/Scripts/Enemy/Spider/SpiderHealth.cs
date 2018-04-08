using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FPSDemo.Scripts.Enemy.Spider
{
    public class SpiderHealth : EnemyHealth
    {
        public override void TakeDamage(int amount, Vector3 hitPoint, Boolean isShocked)
        {
            if (isDead)
                return;
            if (!enemyAudioEffects.isPlaying)
                enemyAudioEffects.PlayOneShot(damageClip, 0.5f);

            CurrentHealth -= amount;
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
    }
}


