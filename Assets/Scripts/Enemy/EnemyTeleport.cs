using FPSDemo.Scripts.Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPSDemo.Scripts.Enemy
{
    public class EnemyTeleport : MonoBehaviour
    {
        

        //Time handling
        [SerializeField]
        public float TimeBetweenSpawns;
        float timer = 0f;

        [HideInInspector]
        private bool isActive = false;

        //The index of the belonging level state
        public int StageIndex;

        private void Awake()
        {
            //The stage index is needed by the level manager to understand which
            //teleport to activate
            if(this.CompareTag("TeleportStage1"))
            {
                StageIndex = 1;
            } else if (this.CompareTag("TeleportStage2"))
            {
                StageIndex = 2;
            } else if (this.CompareTag("TeleportStage3"))
            {
                StageIndex = 3;
            }
        }

        void Update()
        {
            if(isActive)
            {
                timer += Time.deltaTime;
                if(timer >= TimeBetweenSpawns)
                {
                    //Spawn enemy
                    int i = Random.Range(0, LevelManager.Manager.enemyPrefabs.Length);
                    if(LevelManager.Manager.currentEnemiesPerPrefab[i] < LevelManager.Manager.maxEnemiesPerPrefab[i])
                    {
                        GameObject enemy = GameObject.Instantiate(
                            LevelManager.Manager.enemyPrefabs[i],
                            position: this.transform.position, rotation: Quaternion.identity);
                        enemy.GetComponent<EnemyHealth>().spawnIndex = i;
                        timer = 0f;
                        LevelManager.Manager.currentEnemiesPerPrefab[i]++;
                    }
                    
                }
            } 
        }

        /// <summary>
        /// Activate enemy spawn
        /// </summary>
        public void Activate()
        {
            isActive = true;
        }

        /// <summary>
        /// Deactivate enemy spawn
        /// </summary>
        public void Deactivate()
        {
            isActive = false;
        }

       
    }
}


