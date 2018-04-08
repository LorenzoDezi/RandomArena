using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPSDemo.Scripts.Enemy.MocapGuy
{
    public class MocapGuyAttack : EnemyAttack
    {
        [Header("Components Needed")]
        [SerializeField]
        protected GameObject spherePrefab;
        [SerializeField]
        protected Transform mocapGuyRightHand;
        [SerializeField]
        private MocapGuyHealth mocapGuyHealth;

        [Header("Values")]
        [SerializeField]
        private float sphereAcceleration = 10f;
        
        private GameObject currentSpheretta;

        /// <summary>
        /// Spawn sphere, as a children of MocapGuy right hand
        /// </summary>
        public void PrepareSphere()
        {
            currentSpheretta = GameObject.Instantiate(spherePrefab, mocapGuyRightHand.transform, false);
            currentSpheretta.GetComponent<SphereBehaviour>().SetMocapGuy(mocapGuyHealth);
            currentSpheretta.transform.localPosition = new Vector3(0f, 0f, 0f);
            currentSpheretta.transform.forward = transform.forward;
        }

        /// <summary>
        /// Launch a sphere for the player
        /// </summary>
        public void LaunchSphere()
        {
            if (currentSpheretta != null)
            {
                currentSpheretta.GetComponent<SphereBehaviour>().StartFlying(sphereAcceleration, player);
            }
        }

    }
}


