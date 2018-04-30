using FPSDemo.Scripts.Weapons;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FPSDemo.Scripts.Enemy;
using System;
using FPSDemo.Scripts.PickUps;
using UnityEngine.AI;
using FPSDemo.Scripts.Player;

namespace FPSDemo.Scripts.Manager
{
    public class PickUpManager : MonoBehaviour
    {
        //Singleton pattern
        public static PickUpManager manager;

        /// <summary>
        /// these weapons follow the standard spawn algorithm
        /// </summary>
        [SerializeField]
        Weapon[] normalSpawnWeapons;

        /// <summary>
        /// Ammo prefabs to spawn
        /// </summary>
        [SerializeField]
        GameObject[] normalAmmoPrefabs;

        /// <summary>
        /// Zap ammo prefab to spawn
        /// </summary>
        [SerializeField]
        GameObject zapAmmoPrefab;

        /// <summary>
        /// Health pickUp prefab to spawn
        /// </summary>
        [SerializeField]
        GameObject healthPickUpPrefab;
        bool isHealthSpawned = false;

        /// <summary>
        /// Player health controller, to detect current health
        /// </summary>
        [SerializeField]
        HealthController healthController;

        /// <summary>
        /// Current active enemy teleports
        /// </summary>
        EnemyTeleport[] currentTeleports;



        /// <summary>
        /// This weapon  is spawned near enemySpawn after a certain time,
        /// and only if it is not already spawned
        /// </summary>
        [SerializeField]
        Weapon zapGun;
        [SerializeField]
        float timeToSpawn = 20f;
        float Timer;
        bool isZapSpawned = false;

        /// <summary>
        /// The distance at which ammo pickups are spawn
        /// </summary>
        [SerializeField]
        float distanceFromPlayer;

        /// <summary>
        /// The ammo perc. treshold at which a new ammo pickUp will be spawned.
        /// </summary>
        [SerializeField]
        float ammoTreshold = 0.25f;

        /// <summary>
        /// are pickups spawned after the threshold?
        /// </summary>
        bool[] spawned;

        /// <summary>
        /// The player
        /// </summary>
        [SerializeField]
        GameObject Player;

        public void OnPickUp(object sender, PickUpEventArgs args)
        {
            int pickUpIndex = args.PickUpManagerIndexTracker;
            if(pickUpIndex != -1)
            {
                spawned[pickUpIndex] = false;
            } else
            {
                if (args.PickUpTag == "HealthPickUp")
                    isHealthSpawned = false;
                else if (args.PickUpTag == "AmmoFrizzyPickUp")
                    isZapSpawned = false;
            }
        }

        void Awake()
        {
            //Applying singleton pattern
            if(PickUpManager.manager == null)
            {
                PickUpManager.manager = this;
            } else
            {
                Destroy(PickUpManager.manager);
                PickUpManager.manager = this;
            }

            spawned = new bool[normalSpawnWeapons.Length];

            //Initializes boolean values
            for(int i = 0; i < normalSpawnWeapons.Length; i++)
            {
                spawned[i] = false;
            }
            //Initializes zap gun timer
            Timer = 0f;

        }

        private void Start()
        {
            //Get current active teleports
            currentTeleports = LevelManager.Manager.GetActiveTeleports();
        }

        void Update()
        {
            double ammoPercentage;
            for (int i=0; i < normalSpawnWeapons.Length; i++)
            {
                ammoPercentage = ((double) normalSpawnWeapons[i].bulletsLeft) 
                    /  normalSpawnWeapons[i].maxBullets;
                if (ammoPercentage <= ammoTreshold && !spawned[i])
                {
                    //Spawn the pickUp at distanceFromThePlayer from the player
                    this.RandomSpawnNormal();
                    spawned[i] = true;
                }
            }
            Timer += Time.deltaTime;
            if(Timer >= timeToSpawn)
            {
                this.RandomSpawnZap();
                Timer = 0f;
            }
            
        }

        private void RandomSpawnNormal()
        {
            //Spawn a random pickup between the available weapons
            int pickUpChosenIndex = UnityEngine.Random.Range(0, 
                normalAmmoPrefabs.Length);
            Vector3 spawnPosition = 
                Player.transform.position + new Vector3(UnityEngine.Random.Range(-1, 1),
                0f,
                UnityEngine.Random.Range(-1, 1)) * distanceFromPlayer;

            //Checking navMesh
            NavMeshHit nearHitOnNavMesh;
            if (NavMesh.SamplePosition(spawnPosition,
                out nearHitOnNavMesh, distanceFromPlayer, 
                (int) Mathf.Pow(2, LevelManager.Manager.NavMeshIndex)))
            {
                spawnPosition = nearHitOnNavMesh.position;
                spawnPosition.y = 1f;
                GameObject ammoSpawned = GameObject.Instantiate(
                    normalAmmoPrefabs[pickUpChosenIndex],
                    spawnPosition, Quaternion.identity
                );
                ammoSpawned.GetComponent<PickUp>().ManagerIndexTracker = pickUpChosenIndex;                
            }
        }

        private void RandomSpawnZap()
        {
            if(!isZapSpawned)
            {
                int randomTeleportChosenIndex = UnityEngine.Random.Range(0, currentTeleports.Length);
                GameObject ammoSpawned = GameObject.Instantiate(
                    zapAmmoPrefab,
                    currentTeleports[randomTeleportChosenIndex].transform.position
                    + new Vector3(0, 1.5f, 0),
                    Quaternion.identity);
                //One index more of the normalAmmoPrefabs length
                ammoSpawned.GetComponent<PickUp>().ManagerIndexTracker =-1;
                isZapSpawned = true;
            }
            //Spawn zap at a random position between enemy spawn points

        }

        /// <summary>
        /// Set currently active teleports. Called by the level manager
        /// at level change
        /// </summary>
        /// <param name="teleports">Currently active teleports</param>
        public void SetActiveTeleports(EnemyTeleport[] teleports)
        {
            this.currentTeleports = teleports;
        }

        /// <summary>
        /// Method which subscribes to each enemy death in the level
        /// </summary>
        /// <param name="source">The object source</param>
        /// <param name="args"></param>
        public void OnEnemyDeath(object source, EnemyDeadEventArgs args)
        {
            if(UnityEngine.Random.value <= 
                Math.Abs(healthController.startingHealth / healthController.currentHealth - 1.0f)
                && !isHealthSpawned)
            {
                this.SpawnHealthAtPosition(args.DeathPosition);
                isHealthSpawned = true;
            }
        }

        /// <summary>
        /// Spawn health at the parameter position.
        /// </summary>
        /// <param name="position">The position where the pickUp will be spawned</param>
        public void SpawnHealthAtPosition(Vector3 position)
        {
            GameObject.Instantiate(healthPickUpPrefab,
                    position + new Vector3(0, 1, 0),
                    Quaternion.identity);
            healthPickUpPrefab.GetComponent<PickUp>().ManagerIndexTracker = -1;
        }

        /// <summary>
        /// Reset all spawned flag for ammo. Used when level changes to avoid
        /// lost ammo in the previous stage
        /// </summary>
        public void ResetSpawnedPickUp()
        {
            for(int i=0; i < spawned.Length; i++)
            {
                spawned[i] = false;
            }
            isHealthSpawned = false;
            isZapSpawned = false;
        }
    }

    
}


