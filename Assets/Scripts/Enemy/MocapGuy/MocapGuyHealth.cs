using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPSDemo.Scripts.Enemy.MocapGuy
{
    public class MocapGuyHealth : EnemyHealth
    {
        public override void TakeDamage(int amount, Vector3 hitPoint, bool isShocked = false)
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
    }
}


