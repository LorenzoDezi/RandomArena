using FPSDemo.Scripts.Weapons;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPSDemo.Scripts.Weapons
{
    public class WeaponEventArgs : EventArgs
    {
        private Weapon weapon;

        public WeaponEventArgs(Weapon weapon)
        {
            this.weapon = weapon;
        }

        public Weapon Weapon
        {
            get
            {
                return weapon;
            }

            set
            {
                this.weapon = value;
            }
        }
    }

    public class WeaponManager : MonoBehaviour
    {

        public delegate void SwitchEventHandler(object sender, WeaponEventArgs args);

        public SwitchEventHandler SwitchEvent;

        [SerializeField] private GameObject[] Weapons;
        [SerializeField] private GameObject[] WeaponIcons;
        [SerializeField] private float SwitchDelay;
        private bool isSwitching;
        private int currentWeaponIndex;

        private void InitializeWeapons()
        {
            Weapons[0].SetActive(true);
            WeaponIcons[0].SetActive(true);
            for (int i = 1; i < Weapons.Length; i++)
            {
                Weapons[i].SetActive(false);
                WeaponIcons[i].SetActive(false);
            }
            currentWeaponIndex = 0;
        }

        private IEnumerator SwitchAfterDelay(int weaponIndex)
        {
            isSwitching = true;
            yield return new WaitForSeconds(SwitchDelay);
            SwitchWeapons(weaponIndex);
        }

        private void SwitchWeapons(int weaponIndex)
        {
            for (int i = 0; i < Weapons.Length; i++)
            {
                if (i == weaponIndex)
                {
                    Weapons[i].SetActive(true);
                    WeaponIcons[i].SetActive(true);
                    Weapon currentActiveWeapon = Weapons[i].GetComponentInChildren<Weapon>();
                    SwitchEvent.Invoke(this, new WeaponEventArgs(currentActiveWeapon));
                }
                else
                {
                    Weapons[i].SetActive(false);
                    WeaponIcons[i].SetActive(false);
                }
            }

            isSwitching = false;
        }

        void Start()
        {

            InitializeWeapons();
            isSwitching = false;
        }

        // Update is called once per frame
        void Update()
        {

            if (Input.GetAxis("Mouse ScrollWheel") > 0 && !isSwitching)
            {
                Weapons[currentWeaponIndex].GetComponentInChildren<Animator>().SetTrigger("Switch");
                Weapons[currentWeaponIndex].GetComponentInChildren<Weapon>().isSwitching = true;
                currentWeaponIndex++;
                if (currentWeaponIndex >= Weapons.Length)
                {
                    currentWeaponIndex = 0;
                }
                StartCoroutine(SwitchAfterDelay(currentWeaponIndex));
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0 && !isSwitching)
            {
                Weapons[currentWeaponIndex].GetComponentInChildren<Animator>().SetTrigger("Switch");
                Weapons[currentWeaponIndex].GetComponentInChildren<Weapon>().isSwitching = true;
                currentWeaponIndex--;
                if (currentWeaponIndex < 0)
                {
                    currentWeaponIndex = Weapons.Length - 1;
                }
                StartCoroutine(SwitchAfterDelay(currentWeaponIndex));
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1) && !isSwitching && currentWeaponIndex != 0)
            {
                BeginSwitch(0);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2) && !isSwitching && currentWeaponIndex != 1)
            {
                BeginSwitch(1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3) && !isSwitching && currentWeaponIndex != 2)
            {
                BeginSwitch(2);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4) && !isSwitching && currentWeaponIndex != 3)
            {
                BeginSwitch(3);
            }

        }

        void BeginSwitch(int weaponIndex)
        {
            Weapons[currentWeaponIndex].GetComponentInChildren<Animator>().SetTrigger("Switch");
            Weapons[currentWeaponIndex].GetComponentInChildren<Weapon>().isSwitching = true;
            currentWeaponIndex = weaponIndex;
            StartCoroutine(SwitchAfterDelay(currentWeaponIndex));
        }
    }
}




