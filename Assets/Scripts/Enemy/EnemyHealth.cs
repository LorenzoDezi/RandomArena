using FPSDemo.Scripts.Manager;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace FPSDemo.Scripts.Enemy
{
    /// <summary>
    /// This script handle enemy health, death and damage effects
    /// </summary>
    public abstract class EnemyHealth : MonoBehaviour
    {
        [Header("Game Values")]
        public int StartingHealth = 100;
        public int CurrentHealth;
        public int ScoreValue = 10;
        [Header("Sound/Particle Effects")]
        [SerializeField]
        protected AudioClip deathClip;
        [SerializeField]
        protected AudioClip damageClip;
        [SerializeField]
        protected ParticleSystem shockEffect;
        [SerializeField]
        protected ParticleSystem bloodSplat;
        [Header("Animator")]
        [SerializeField]
        protected Animator anim;
        [Header("Spawn index")]
        public int spawnIndex;

        
        protected AudioSource enemyAudioEffects;
        protected new Collider collider;
        protected bool isDead = false;
        protected bool isSinking;

        public delegate void EnemyDeadEventHandler(object source, EnemyDeadEventArgs args);
        public event EnemyDeadEventHandler EnemyDeadEvent;

        protected virtual void Awake()
        {
            enemyAudioEffects = GetComponents<AudioSource>()[0];
            collider = GetComponents<Collider>()[0];
            CurrentHealth = StartingHealth;
        }

        private void Start()
        {
            LevelManager.Manager.ChangeLevelEv += AutoDeath;
            EnemyDeadEvent += PickUpManager.manager.OnEnemyDeath;
            EnemyDeadEvent += ScoreManager.manager.ScoreIncrease;
        }

        /// <summary>
        /// Take damage from an hit.
        /// </summary>
        /// <param name="amount">The amount of damage</param>
        /// <param name="hitPoint">The position in which it was hit</param>
        /// <param name="isShocked">If the attack is from the zapgun, the effect will be different</param>
        public abstract void TakeDamage(int amount, Vector3 hitPoint, Boolean isShocked = false);

        /// <summary>
        /// Play shock particle effect
        /// </summary>
        public virtual void ReceiveShock()
        {
            if (!shockEffect.isPlaying && !isDead)
                shockEffect.Play();
        }

        /// <summary>
        /// Death of an enemy - score increase
        /// </summary>
        protected virtual void Death()
        {
            isDead = true;
            anim.SetTrigger("Dead");
            this.LeavePhysicWorld();
            EnemyDeadEvent.Invoke(this, new EnemyDeadEventArgs(this.ScoreValue, transform.position));
            LevelManager.Manager.UpdateNumberOfEnemiesPerPrefab(spawnIndex);

        }

        

        /// <summary>
        /// AutoDeath - it happens at a level change
        /// </summary>
        protected void AutoDeath()
        {
            if(!this.IsDead())
            {
                this.CurrentHealth = 0;
                isDead = true;
                anim.SetTrigger("Dead");
                this.LeavePhysicWorld();
                LevelManager.Manager.UpdateNumberOfEnemiesPerPrefab(spawnIndex);
            }

        }

        /// <summary>
        /// Remove the "corpse" of the enemy from the physic world
        /// </summary>
        public void LeavePhysicWorld()
        {
            Destroy(collider, 2f);
            GetComponent<Rigidbody>().isKinematic = true;
            Destroy(this.gameObject, 2f);
            this.tag = "Dead";
        }


        /// <summary>
        /// Verifying the death of the enemy
        /// </summary>
        /// <returns></returns>
        public bool IsDead()
        {
            return isDead;
        }
    }

    public class EnemyDeadEventArgs
    {
        private int scoreValue;
        public int ScoreValue
        {
            get { return scoreValue; }
            set { this.scoreValue = value; }
        }

        private Vector3 deathPosition;
        public Vector3 DeathPosition
        {
            get { return deathPosition; }
            set { this.deathPosition = value; }
        }

        public EnemyDeadEventArgs(int scoreValue, Vector3 deathPosition)
        {
            this.scoreValue = scoreValue;
            this.deathPosition = deathPosition;
        }
    }
}


