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
        AudioClip pickedUpAmmo;

        AudioSource audioSource;
        

        void Start()
        {
            controller = this.GetComponent<MyFirstPersonController>();
            gun = this.GetComponentInChildren<Gun>();
            rifle = this.GetComponentInChildren<Rifle>();
            shotgun = this.GetComponentInChildren<Shotgun>();
            frizzy = this.GetComponentInChildren<Frizzy>();
            audioSource = this.GetComponents<AudioSource>()[1];
        }

        private void AmmoPicked(GameObject ammoPicked, bool isFrizzy = false)
        {
            audioSource.PlayOneShot(pickedUpAmmo);
            if(!isFrizzy)
                ammoPicked.GetComponent<PickUp>().RaisePickUpEvent();
            Destroy(ammoPicked);
        }

        private void OnTriggerEnter(Collider collider)
        {
            GameObject pickUpHit = collider.gameObject;
            if (pickUpHit.CompareTag("SprintPickUp"))
            {
                if (controller.dodgeLeft < controller.dodgeMax)
                {
                    controller.IncreaseDodgeLeft();
                    GameObject.Destroy(pickUpHit);
                }


            } else if (pickUpHit.CompareTag("AmmoGunPickUp"))
            {
                gun.RefillAmmo();
                AmmoPicked(pickUpHit);


            } else if (pickUpHit.CompareTag("AmmoShotgunPickUp"))
            {
                shotgun.RefillAmmo();
                AmmoPicked(pickUpHit);

            } else if (pickUpHit.CompareTag("AmmoRiflePickUp"))
            {
                rifle.RefillAmmo();
                AmmoPicked(pickUpHit);

            } else if (pickUpHit.CompareTag("AmmoFrizzyPickUp"))
            {
                frizzy.RefillAmmo();
                AmmoPicked(pickUpHit, true);
            }

        }
    }
}


