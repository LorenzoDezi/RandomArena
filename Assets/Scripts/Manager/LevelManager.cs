using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FPSDemo.Scripts.Enemy;
using System.Linq;
using System;
using UnityEngine.AI;

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
        public GameObject[] enemyPrefabs;
        [SerializeField]
        public int[] maxEnemiesPerPrefab;
        [SerializeField]
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

            //Setting enemies spawn parameters
            if(enemyPrefabs.Length != maxEnemiesPerPrefab.Length)
            {
                Debug.LogError("enemyPrefabs length must be the same of maxEnemiesPerPrefab!");
            }

            currentEnemiesPerPrefab = new int[enemyPrefabs.Length];
            for(int i=0; i < enemyPrefabs.Length; i++) {
                currentEnemiesPerPrefab[i] = 0;
            }
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
            //Hardcoding door animations
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

        /// <summary>
        /// Get currently active teleports
        /// </summary>
        public EnemyTeleport[] GetActiveTeleports()
        {
            return teleports.Where(teleport => teleport.StageIndex == this.CurrentStage).ToArray();
        }
    }
}

