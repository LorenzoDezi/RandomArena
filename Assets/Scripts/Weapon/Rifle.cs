using FPSDemo.Scripts.Enemy;
using FPSDemo.Scripts.Enemy.MocapGuy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FPSDemo.Scripts.Weapons
{
    public class Rifle : Weapon
    {
        [Header("Weapon Shoot Values")]
        [SerializeField]
        protected float fireRate;
        protected float fireTimer;


        protected override void Start()
        {
            base.Start();
        }

        public override void Fire()
        {
            if (fireTimer < fireRate || currentBullets <= 0 || isSwitching) return;
            RaycastHit hit;
            if (Physics.Raycast(shootPoint.position, 
                shootPoint.transform.forward, 
                out hit, range))
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
                    //We make bullet hole and particle
                    GameObject shotParticleSpawn = GameObject.Instantiate(shotParticle, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
                    GameObject bulletHoleSpawn = GameObject.Instantiate(bulletHole, hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal));
                    Destroy(shotParticleSpawn, 2f);
                    Destroy(bulletHoleSpawn, 2f);
                }
            }

            anim.CrossFadeInFixedTime("Fire", 0.3f);
            muzzleFlash.Play();
            PlayShootSound();
            currentBullets--;
            fireTimer = 0.0f;
            base.Fire();
        }

        protected override void Update()
        {
            shotInput = Input.GetButton("Fire");
            base.Update();
            if (fireTimer < fireRate)
            {
                fireTimer += Time.deltaTime;
            }
        }


    }
}



