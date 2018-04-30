using FPSDemo.Scripts.Enemy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FPSDemo.Scripts.Weapons
{
    public class Frizzy : Weapon
    {

        bool isReady = false;

        [Header("Weapon Shoot Values")]
        [SerializeField]
        float radius = 1f;

        public void Ready()
        {
            isReady = true;
            if (currentBullets > 0)
            {
                muzzleFlash.Play();
            }
        }

        public void NotReady()
        {
            isReady = false;
            if (muzzleFlash.isPlaying)
            {
                muzzleFlash.Stop();
                audio.Stop();
            }
        }

        void LightiningFire(Vector3 startPosition)
        {
            foreach (RaycastHit hit in Physics.RaycastAll(startPosition, shootPoint.transform.forward, range))
            {
                if (hit.transform.tag.Contains("Enemy"))
                {
                    hit.transform.gameObject.GetComponent<EnemyHealth>().TakeDamage(damage, hit.point, true);
                    hit.transform.gameObject.GetComponent<EnemyHealth>().ReceiveShock();

                }
                else if (hit.transform.tag.Contains("Environment"))
                {
                    //TODO: Vedere bene cosa spawnare quando il tuono impatta sui colliders
                    //GameObject shotParticleSpawn = GameObject.Instantiate(shotParticle, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
                    //Destroy(shotParticleSpawn, 2f);
                }
            }
        }

        public override void Fire()
        {
            if (currentBullets <= 0 || isSwitching || !isReady)
            {

                if (muzzleFlash.isPlaying)
                {
                    muzzleFlash.Stop();
                }
                return;
            }
            for (float i = 0; i < radius; i += 0.1f)
            {
                LightiningFire(shootPoint.position + new Vector3(i, 0, 0));
                LightiningFire(shootPoint.position + new Vector3(-i, 0, 0));
                LightiningFire(shootPoint.position + new Vector3(0, i, 0));
                LightiningFire(shootPoint.position + new Vector3(0, -i, 0));
            }
            if (!audio.isPlaying)
                audio.Play();
            currentBullets--;
            base.Fire();
        }

        public override void RefillAmmo()
        {
            //This time refilled ammo are exactly a mag
            int refilledBulletsLeft = bulletsLeft + bulletsPerMag;
            if(refilledBulletsLeft >= maxBullets)
            {
                this.bulletsLeft = maxBullets;
            } else
            {
                this.bulletsLeft = refilledBulletsLeft;
            }
            this.UpdateUI();
        }

        protected override void FixedUpdate()
        {
            shotInput = Input.GetButton("Fire");
            anim.SetBool("Fire", shotInput);
            if (currentBullets <= 0)
            {
                if (muzzleFlash.isPlaying)
                {
                    muzzleFlash.Stop();
                    audio.Stop();
                }
            }

            if (shotInput)
            {
                if (currentBullets > 0 && !isRunning && !isReloading)
                {
                    if (!isFuckingAround)
                        Fire();
                }
                else if (bulletsLeft > 0 && !isRunning)
                {
                    DoReload();
                }

            }

            if (isFuckingAround)
            {
                RaycastHit hit;
                if (Physics.SphereCast(shootPoint.position, 5f, shootPoint.transform.forward, out hit, range))
                {
                    isFuckingAroundAtSomeone = hit.transform.tag == "Enemy";
                }
                else
                {
                    isFuckingAroundAtSomeone = false;
                }
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                if (currentBullets < bulletsPerMag && bulletsLeft > 0)
                {
                    DoReload();
                }
            }

            if (Input.GetKeyDown(KeyCode.F) && !anim.GetBool("Fuck"))
            {
                anim.SetBool("Fuck", true);
                isFuckingAroundAtSomeone = true;
            }
        }


    }
}



