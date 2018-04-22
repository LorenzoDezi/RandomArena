using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FPSDemo.Scripts.Weapons;

namespace FPSDemo.Scripts.Animations
{
    class ShotgunReloadState : ReloadState
    {
        [SerializeField] private AudioClip BulletReloadSound;

        public override void OnStateUpdate(UnityEngine.Animator animator, UnityEngine.AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            if (hasReloaded) return;
            if (animatorStateInfo.normalizedTime >= ReloadTime)
            {
                Weapon currentWeapon = animator.GetComponent<Weapon>();
                int bulletsToReload = currentWeapon.bulletsPerMag - currentWeapon.currentBullets;
                hasReloaded = true;
                currentWeapon.StartCoroutine(waitForReload(animator, bulletsToReload));
                currentWeapon.Reload();
            }
        }

        /// <summary>
        /// This function waits for the reload of the shotgun.
        /// </summary>
        /// <param name="bulletToReload">The number of bullets to reload, or the number of seconds to wait
        /// for the animation to finish</param>
        /// <returns></returns>
        private IEnumerator waitForReload(UnityEngine.Animator anim, int bulletToReload)
        {
            for(int i=0; i <= bulletToReload; i++)
            {
                if (i == bulletToReload)
                {
                    anim.SetBool("Reload", false);
                    yield return null;
                }
                else
                {
                    anim.GetComponent<AudioSource>().PlayOneShot(BulletReloadSound);
                    yield return new WaitForSeconds(0.2f);
                }
                
            }
        }
    }
}
