using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FPSDemo.Scripts.Weapons;
using FPSDemo.Scripts.PickUps;

namespace FPSDemo.Scripts.Player
{
    public class PickUpHandler : MonoBehaviour
    {
        MyFirstPersonController controller;
        HealthController healthController;

        //Weapons
        [Header("Weapons")]
        [SerializeField]
        Gun gun;
        [SerializeField]
        Rifle rifle;
        [SerializeField]
        Shotgun shotgun;
        [SerializeField]
        Frizzy frizzy;
        [Header("Sound")]
        [SerializeField]
        private AudioClip pickedUpAmmo;
        [SerializeField]
        private AudioClip pickedUpHealth;

        AudioSource audioSource;
        

        void Start()
        {
            controller = this.GetComponent<MyFirstPersonController>();
            healthController = this.GetComponent<HealthController>();
            gun = this.GetComponentInChildren<Gun>();
            rifle = this.GetComponentInChildren<Rifle>();
            shotgun = this.GetComponentInChildren<Shotgun>();
            frizzy = this.GetComponentInChildren<Frizzy>();
            audioSource = this.GetComponents<AudioSource>()[1];
        }

        private void PickUpPicked(GameObject pickUpPicked, bool isHealth = false)
        {
            if (!isHealth)
                audioSource.PlayOneShot(pickedUpAmmo);
            else
                audioSource.PlayOneShot(pickedUpHealth);
            pickUpPicked.GetComponent<PickUp>().RaisePickUpEvent();
            Destroy(pickUpPicked);
        }

        private void OnTriggerEnter(Collider collider)
        {
            GameObject pickUpHit = collider.gameObject;
            if (pickUpHit.CompareTag("HealthPickUp"))
            {
                healthController.RechargeHealth(Mathf.FloorToInt(healthController.startingHealth / 2f));
                PickUpPicked(pickUpHit, true);
            }
            else if (pickUpHit.CompareTag("AmmoGunPickUp"))
            {
                gun.RefillAmmo();
                PickUpPicked(pickUpHit);
            } else if (pickUpHit.CompareTag("AmmoShotgunPickUp"))
            {
                shotgun.RefillAmmo();
                PickUpPicked(pickUpHit);

            } else if (pickUpHit.CompareTag("AmmoRiflePickUp"))
            {
                rifle.RefillAmmo();
                PickUpPicked(pickUpHit);

            } else if (pickUpHit.CompareTag("AmmoFrizzyPickUp"))
            {
                frizzy.RefillAmmo();
                PickUpPicked(pickUpHit);
            }
        }
    }
}


