using UnityEngine;
using System.Collections;
using System;

namespace FPSDemo.Scripts.Enemy.Zombie
{
    public class ZombieMovement : EnemyMovement
    {

        protected override void Start()
        {
            anim.SetFloat("Speed", 1);
        }

        protected override void Update()
        {
            
            this.HandleFrenzy();
            if (!enemyHealth.IsDead() && playerHealth.currentHealth > 0)
            {
                //Simple and stupid attack for the zombie
                nav.SetDestination(playerHealth.transform.position);
            }
            
            else
            {
                if (!nav.isStopped)
                    nav.isStopped = true;
                anim.SetFloat("Speed", 0);
            }

        }
    }
}


