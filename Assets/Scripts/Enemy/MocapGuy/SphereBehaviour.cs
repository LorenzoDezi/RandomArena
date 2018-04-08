using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FPSDemo.Scripts.Player;

namespace FPSDemo.Scripts.Enemy.MocapGuy
{
    public class SphereBehaviour : MonoBehaviour
    {

        public bool onFly = false;

        //Once reached, the sphere get destroyed.
        [SerializeField]
        float timeToLive = 5f;


        MocapGuyHealth mocapGuyWhoCreatedMe;
        Animator animator;
        bool destroyed = false;

        private void Awake()
        {
            this.animator = this.GetComponent<Animator>();
        }

        /// <summary>
        /// Set the MocapGuy who creates the sphere. To be called at instantiation.
        /// </summary>
        /// <param name="mocapGuyHealth"></param>
        public void SetMocapGuy(MocapGuyHealth mocapGuyHealth)
        {
            this.mocapGuyWhoCreatedMe = mocapGuyHealth;
        }

        /// <summary>
        /// The sphere start flying towards its objective
        /// </summary>
        /// <param name="sphereAcceleration"></param>
        /// <param name="player">Objective of the sphere</param>
        public void StartFlying(float sphereAcceleration, GameObject player)
        {
            onFly = true;
            this.GetComponent<Rigidbody>().isKinematic = false;
            //The sphere lose its parent, its coordinate now refers to the world frame
            transform.parent = null;
            transform.LookAt(player.transform.position);
            this.GetComponent<Rigidbody>().velocity = transform.forward * sphereAcceleration;
        }

        void OnTriggerEnter(Collider other)
        {
            //If the player destroys the spehre before the hit
            //it doesn't make damage
            if (this.destroyed)
                return;

            if (other.transform.CompareTag("Player"))
            {
                //Player hit
                other.transform.GetComponent<HealthController>().TakeDamage(20);
            }

            if (other.transform.CompareTag("Enemy") && !onFly)
            {
                //Enemies receive hit too
                other.transform.GetComponent<EnemyHealth>().TakeDamage(20, other.ClosestPoint(this.transform.position), false);
            }

            //TODO: Inserire effetto carino, su tutti i destroy
            if (!other.transform.CompareTag("MocapEnemy") && onFly)
                GameObject.Destroy(this.gameObject);
        }

        /// <summary>
        /// Destroy the sphere, on a player attack or on
        /// time to live exausted
        /// </summary>
        public void Destroy()
        {
            this.destroyed = true;
            animator.SetBool("Destroyed", true);
            GameObject.Destroy(this.gameObject, 1f);
        }

        private void Update()
        {
            timeToLive -= Time.deltaTime;
            if (timeToLive <= 0 || 
                (mocapGuyWhoCreatedMe != null 
                && mocapGuyWhoCreatedMe.IsDead()))
            {
                this.Destroy();
            }
        }
    }
}
