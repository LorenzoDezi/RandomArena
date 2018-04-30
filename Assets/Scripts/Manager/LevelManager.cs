using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FPSDemo.Scripts.Enemy;
using System.Linq;
using System;
using UnityEngine.AI;
using FPSDemo.Scripts.Enemy.MocapGuy;
using FPSDemo.Scripts.Enemy.Limana;
using FPSDemo.Scripts.Enemy.Zombie;
using FPSDemo.Scripts.Enemy.Spider;
using FPSDemo.Scripts.Enemy.Remy;

namespace FPSDemo.Scripts.Manager
{
    public class LevelManager : MonoBehaviour
    {

        //Applying singleton pattern, to make sure there is only one LevelManager per level
        [HideInInspector]
        public static LevelManager Manager;

        //The current stage, it goes from 1 to 3
        [HideInInspector]
        public int CurrentStage;
        //The current navmesh index
        [HideInInspector]
        public int NavMeshIndex;

        //The gameObject that contains all of the
        //teleports
        [SerializeField]
        GameObject teleportContainer;
        //Actual teleports
        EnemyTeleport[] teleports;

        //Door opening animation, starting at each level change
        [SerializeField]
        Animation[] doorAnimations;



        [Header("Enemies to spawn and parameters")]
        //Enemy prefabs to spawn
        [SerializeField]
        GameObject spiderPrefab;
        [SerializeField]
        GameObject mocapPrefab;
        [SerializeField]
        GameObject limanaPrefab;
        [SerializeField]
        GameObject remyPrefab;
        [SerializeField]
        GameObject zombiePrefab;

        [HideInInspector]
        public GameObject[] enemyPrefabs;
        [HideInInspector]
        public int[] maxEnemiesPerPrefab;
        [HideInInspector]
        public int[] currentEnemiesPerPrefab;

        //Change level event, every enemy is linked to this event.
        public delegate void ChangeLevelEvent();
        public event ChangeLevelEvent ChangeLevelEv;

        void Awake()
        {

            this.CurrentStage = 1;
            this.NavMeshIndex = NavMesh.GetAreaFromName("Phase1");
            if (Manager == null)
                LevelManager.Manager = this;
            else
            {
                Destroy(LevelManager.Manager);
                LevelManager.Manager = this;
            }

            teleports = teleportContainer.GetComponentsInChildren<EnemyTeleport>();
            this.InitializeEnemyPrefabs();
            ChangeMaxNumberOfEnemies(this.CurrentStage);
        }

        private void InitializeEnemyPrefabs()
        {
            this.enemyPrefabs = new GameObject[5];
            this.enemyPrefabs[MocapGuyHealth.SpawnIndex] = mocapPrefab;
            this.enemyPrefabs[LimanaHealth.SpawnIndex] = limanaPrefab;
            this.enemyPrefabs[RemyHealth.SpawnIndex] = remyPrefab;
            this.enemyPrefabs[ZombieHealth.SpawnIndex] = zombiePrefab;
            this.enemyPrefabs[SpiderHealth.SpawnIndex] = spiderPrefab;
            this.currentEnemiesPerPrefab = new int[5] { 0, 0, 0, 0, 0 };
        }

        private void Start()
        {
            //Activating teleports
            this.ActivateTeleportsWithStageIndex(this.CurrentStage);

        }



        public void ActivateTeleportsWithStageIndex(int index)
        {
            //Activate teleports at index
            EnemyTeleport[] currentStageTeleports = teleports.Where(t => t.StageIndex == index).ToArray();
            PickUpManager.manager.SetActiveTeleports(currentStageTeleports);
            foreach (EnemyTeleport teleport in currentStageTeleports)
            {
                teleport.Activate();
            }
        }

        /// <summary>
        /// When an enemy dies, the current number of enemies of its type 
        /// decrease
        /// </summary>
        /// <param name="index"></param>
        public void UpdateNumberOfEnemiesPerPrefab(int index)
        {
            this.currentEnemiesPerPrefab[index]--;
        }

        public void ChangeLevel()
        {
            //Deactivate all teleports
            ChangeLevelEv.Invoke();
            foreach (EnemyTeleport teleport in teleports)
            {
                teleport.Deactivate();
            }
            CurrentStage++;
            this.ChangeMaxNumberOfEnemies(this.CurrentStage);
            if (CurrentStage == 2)
            {
                Vector3 healthSpawnPosition = doorAnimations[0].transform.position;
                healthSpawnPosition.y = 0;
                PickUpManager.manager.SpawnHealthAtPosition(healthSpawnPosition);
                doorAnimations[0].Play();
                this.NavMeshIndex =  NavMesh.GetAreaFromName("Phase2");
            }
            else if (CurrentStage == 3)
            {
                Vector3 healthSpawnPosition = doorAnimations[1].transform.position;
                healthSpawnPosition.y = 0;
                PickUpManager.manager.SpawnHealthAtPosition(healthSpawnPosition);
                doorAnimations[1].Play();
                this.NavMeshIndex = NavMesh.GetAreaFromName("Phase3");
            }

        }

        private void ChangeMaxNumberOfEnemies(int currentStage)
        {
            switch(currentStage)
            {
                case 1:
                    this.maxEnemiesPerPrefab[SpiderHealth.SpawnIndex] = 25;
                    this.maxEnemiesPerPrefab[MocapGuyHealth.SpawnIndex] = 5;
                    this.maxEnemiesPerPrefab[RemyHealth.SpawnIndex] = 0;
                    this.maxEnemiesPerPrefab[LimanaHealth.SpawnIndex] = 0;
                    this.maxEnemiesPerPrefab[ZombieHealth.SpawnIndex] = 0;
                    break;
                case 2:
                    this.maxEnemiesPerPrefab[SpiderHealth.SpawnIndex] = 15;
                    this.maxEnemiesPerPrefab[MocapGuyHealth.SpawnIndex] = 5;
                    this.maxEnemiesPerPrefab[RemyHealth.SpawnIndex] = 3;
                    this.maxEnemiesPerPrefab[LimanaHealth.SpawnIndex] = 0;
                    this.maxEnemiesPerPrefab[ZombieHealth.SpawnIndex] = 15;
                    break;
                case 3:
                    this.maxEnemiesPerPrefab[SpiderHealth.SpawnIndex] = 15;
                    this.maxEnemiesPerPrefab[MocapGuyHealth.SpawnIndex] = 5;
                    this.maxEnemiesPerPrefab[RemyHealth.SpawnIndex] = 3;
                    this.maxEnemiesPerPrefab[LimanaHealth.SpawnIndex] = 2;
                    this.maxEnemiesPerPrefab[ZombieHealth.SpawnIndex] = 15;
                    break;
            }
        }

        /// <summary>
        /// Get currently active teleports
        /// </summary>
        public EnemyTeleport[] GetActiveTeleports()
        {
            return teleports.Where(teleport => teleport.StageIndex == this.CurrentStage).ToArray();
        }
    }
}

