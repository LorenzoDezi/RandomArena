using FPSDemo.Scripts.Enemy;
using FPSDemo.Scripts.Enemy.MocapGuy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FPSDemo.Scripts.Weapons
{
    public class Gun : Weapon
    {
        protected override void FixedUpdate()
        {
            shotInput = Input.GetButtonDown("Fire");
            base.FixedUpdate();
        }

        public override void Fire()
        {
            if (isShooting || currentBullets <= 0 || isSwitching)
                return;

            RaycastHit hit;
            if (Physics.Raycast(shootPoint.position, shootPoint.transform.forward, out hit, range))
            {
                if (hit.transform.tag.Contains("Enemy"))
                {
                    hit.transform.gameObject.GetComponent<EnemyHealth>().TakeDamage(damage, hit.point);
                }
                else if (hit.transform.tag.Contains("Spheretta"))
                {
                    hit.transform.gameObject.GetComponent<SphereBehaviour>().Destroy();
                }
                else
                {
                    GameObject shotParticleSpawn = GameObject.Instantiate(shotParticle, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
                    GameObject bulletHoleSpawn = GameObject.Instantiate(bulletHole, hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal));
                    Destroy(shotParticleSpawn, 2f);
                    Destroy(bulletHoleSpawn, 2f);
                }

            }

            anim.CrossFadeInFixedTime("Fire", 0.05f);
            muzzleFlash.Play();
            PlayShootSound();
            currentBullets--;
            base.Fire();
        }

    }
}



