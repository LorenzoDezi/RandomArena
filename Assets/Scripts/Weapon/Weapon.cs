using FPSDemo.Scripts.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FPSDemo.Scripts.Weapons
{

    public abstract class Weapon : MonoBehaviour
    {
        //Bullets and mags logic
        [Header("Bullets and Mags logic")]
        public int bulletsPerMag = 30;
        public int bulletsLeft = 200;
        public int maxBullets = 200;
        public int currentBullets;

        //Prefabs
        [Header("Required Prefabs")]
        [SerializeField]
        protected ParticleSystem muzzleFlash;
        [SerializeField]
        protected GameObject bulletHole;
        [SerializeField]
        protected GameObject shotParticle;

        //Weapon values
        [Header("Weapon Shoot Values")]
        [SerializeField]
        protected float range = 100f;
        [SerializeField]
        protected Transform shootPoint;
        [SerializeField]
        protected int damage;

        [Header("UI")]
        [SerializeField]
        protected Text AmmoText;

        

        //Audio
        protected new AudioSource audio;
        [Header("Sounds/Effects")]
        [SerializeField]
        protected AudioClip shootSound;
        [SerializeField]
        protected AudioClip emptyMagSound;

        //Animator
        protected Animator anim;
        
        //Boolean status variables
        [HideInInspector]
        public bool isSwitching;
        protected bool isShooting;
        protected bool isReloading;
        protected bool isRunning;
        protected bool shotInput;
        protected bool isFuckingAround;
        protected bool isFuckingAroundAtSomeone;


        protected virtual void Start()
        {
            currentBullets = bulletsPerMag;
            anim = this.GetComponent<Animator>();
            audio = this.GetComponent<AudioSource>();
            shotInput = isShooting = isReloading
                = isFuckingAround = isSwitching = isFuckingAroundAtSomeone = false;
            this.UpdateUI();
        }

        private void OnEnable()
        {
            isSwitching = false;
            this.UpdateUI();
        }


        protected void UpdateUI()
        {
            this.AmmoText.text = currentBullets + "/" + bulletsLeft;
        }

        public void StopFucking()
        {
            anim.SetBool("Fuck", false);
            if (isFuckingAroundAtSomeone)
            {
                ScoreManager.manager.ScoreIncrease(Convert.ToInt32(20 * ScoreManager.manager.scoreMultiplier));
                isFuckingAroundAtSomeone = false;
            }
        }



        public void StartRunning(bool isWalking)
        {
            isRunning = !isWalking;
            anim.SetBool("Run", isRunning);
        }


        protected virtual void FixedUpdate()
        {
            //Checking animator infos
            AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
            isReloading = info.IsName("Reload");
            isShooting = info.IsName("Fire");
            isFuckingAround = info.IsName("FuckingAround");

            //You want to shoot
            if (shotInput)
            {
                //If you can shoot
                if (currentBullets > 0 && !isRunning && !isReloading)
                {
                    if (!isFuckingAround)
                        Fire();
                }
                //else if your magazine is empty
                else if (bulletsLeft > 0 && !isRunning)
                {
                    if (!audio.isPlaying && !isReloading)
                        audio.PlayOneShot(emptyMagSound);
                    DoReload();
                }
                //else if all of your ammunitions are dead
                else if (bulletsLeft <= 0)
                {
                    if (!audio.isPlaying)
                        audio.PlayOneShot(emptyMagSound);
                }

            }

            //You're fucking at someone
            if (isFuckingAround)
            {
                RaycastHit hit;
                //You're insulting someone
                if (Physics.Raycast(shootPoint.position, shootPoint.transform.forward, out hit, range))
                {
                    if (hit.transform.tag.Contains("Enemy"))
                    {
                        isFuckingAroundAtSomeone = false;
                    }
                }
                else
                {
                    isFuckingAroundAtSomeone = false;
                }
            }

            //You're reloading
            if (Input.GetKeyDown(KeyCode.R))
            {
                if (currentBullets < bulletsPerMag && bulletsLeft > 0)
                {
                    DoReload();
                }
            }

            //You want to fuck at someone
            if (Input.GetKeyDown(KeyCode.F) && !anim.GetBool("Fuck"))
            {
                anim.SetBool("Fuck", true);
                isFuckingAroundAtSomeone = true;
            }

        }

        /// <summary>
        /// Fire is implemented by concrete classes.
        /// </summary>
        public virtual void Fire()
        {
            //The rest of the method is implemented by concrete classes
            UpdateUI();
        }


        protected void PlayShootSound()
        {
            audio.PlayOneShot(shootSound);
        }

        public void Reload()
        {

            if (bulletsLeft <= 0)
            {
                return;
            }

            int bulletsToLoad = bulletsPerMag - currentBullets;
            int bulletsToDeduct = (bulletsLeft >= bulletsToLoad) ? bulletsToLoad : bulletsLeft;

            bulletsLeft -= bulletsToDeduct;
            currentBullets += bulletsToDeduct;

            this.UpdateUI();
        }

        protected void DoReload()
        {
            if (isReloading) return;

            anim.SetBool("Reload", true);
        }

        /// <summary>
        /// Refill ammo is called when the player hits an
        /// ammo pickup
        /// </summary>
        public virtual void RefillAmmo()
        {
            int bulletsToRefill = this.maxBullets / 4;
            int refilledBulletsLeft = bulletsLeft + bulletsToRefill;

            //At max the weapon can contain maxBullets ammo
            if(refilledBulletsLeft >= maxBullets)
            {
                this.bulletsLeft = maxBullets;

            } else
            {
                this.bulletsLeft = refilledBulletsLeft;
            }
            this.UpdateUI();
        }
    }
}




