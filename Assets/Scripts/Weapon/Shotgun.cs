using FPSDemo.Scripts.Enemy;
using FPSDemo.Scripts.Enemy.MocapGuy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FPSDemo.Scripts.Weapons
{
    public class Shotgun : Weapon
    {
        [Header("Sounds/Effects")]
        [SerializeField]
        private AudioClip pumpUpSound;

        [Header("Weapon Shoot Values")]
        [SerializeField]
        float shotgunSpreadVariance = 1f;

        public void DoPumpUp()
        {
            audio.PlayOneShot(pumpUpSound);
        }

        protected override void FixedUpdate()
        {
            shotInput = Input.GetButtonDown("Fire");
            base.FixedUpdate();

        }

        void BulletFire(Vector3 startPosition)
        {
            RaycastHit hit;
            if (Physics.Raycast(startPosition, shootPoint.transform.forward, out hit, range))
            {
                if (hit.transform.tag.Contains("Enemy") || hit.transform.tag.Contains("Dead"))
                {
                    hit.transform.gameObject.GetComponent<EnemyHealth>().TakeDamage(ReturnDamageFromRange(hit.distance, range), hit.point);
                }
                else if (hit.transform.tag.Contains("Spheretta"))
                {
                    hit.transform.gameObject.GetComponent<SphereBehaviour>().Destroy();
                }
                else if (hit.transform.tag.Contains("Environment"))
                {
                    //We make bullet hole and particle
                   
                    GameObject shotParticleSpawn = GameObject.Instantiate(shotParticle, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
                    GameObject bulletHoleSpawn = GameObject.Instantiate(bulletHole, hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal));
                    Destroy(shotParticleSpawn, 2f);
                    Destroy(bulletHoleSpawn, 2f);
                }
            }
        }

        public override void Fire()
        {
            if (isShooting || currentBullets <= 0 || isSwitching) return;

            Vector3 rayStartPosition = shootPoint.position;
            for (int i = 0; i < 30f; i++)
            {
                if (i == 0)
                {
                    BulletFire(shootPoint.position);
                }
                else
                {
                    rayStartPosition += shootPoint.up * UnityEngine.Random.Range(-shotgunSpreadVariance, shotgunSpreadVariance);
                    rayStartPosition += shootPoint.right * UnityEngine.Random.Range(-shotgunSpreadVariance, shotgunSpreadVariance);
                    BulletFire(rayStartPosition);
                }

            }


            anim.CrossFadeInFixedTime("Fire", 0.05f);
            muzzleFlash.Play();
            PlayShootSound();
            currentBullets--;
            base.Fire();
        }

        public int ReturnDamageFromRange(float shotDistance, float maxRange)
        {
            int damage = Convert.ToInt32(100f - shotDistance * (100f / maxRange));
            return damage;
        }
    }
}



