using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FPSDemo.Scripts.Enemy.Limana
{
    public class LimanaHealth : EnemyHealth
    {
        //This is to fix the shittiness of our resources
        private AudioSource enemyAudioEffectWithout3dBlend;

        protected override void Awake()
        {
            enemyAudioEffectWithout3dBlend = this.GetComponents<AudioSource>()[1];
            base.Awake();
        }


        public override void TakeDamage(int amount, Vector3 hitPoint, Boolean isShocked)
        {
            if (isDead || isShocked)
                return;

            enemyAudioEffectWithout3dBlend.PlayOneShot(damageClip, 1f);

            CurrentHealth -= amount;
            ParticleSystem bloodSplatInstance = GameObject.Instantiate(this.bloodSplat, hitPoint, Quaternion.FromToRotation(hitPoint.normalized, Vector3.right));
            bloodSplatInstance.Play();
            Destroy(bloodSplatInstance.gameObject, 2f);

            if (CurrentHealth <= 0)
            {
                Death();
            }
        }
    }
}


