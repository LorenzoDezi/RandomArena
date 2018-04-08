using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPSDemo.Scripts.Enemy.Remy
{
    public class RemyAttack : EnemyAttack
    {
        RemyMovement movement;

        //Material to handle invisible pattern
        [SerializeField]
        Material invisibleMaterial;
        //This list keeps track of the original Remy materials, in order to 
        //make him appear when he attacks
        List<Material> remyStandardMaterials;
        SkinnedMeshRenderer[] meshRenderers;

        void Start()
        {

            movement = GetComponent<RemyMovement>();
            meshRenderers = this.GetComponentsInChildren<SkinnedMeshRenderer>();
            remyStandardMaterials = new List<Material>();
            foreach (SkinnedMeshRenderer renderer in meshRenderers)
            {
                remyStandardMaterials.Add(renderer.material);
                //Remy is invisible when moving. 
                renderer.material = invisibleMaterial;
            }

        }

        [Header("Sound Effects/Particles")]
        [SerializeField]
        protected AudioClip attackClip;

        void OnCollisionEnter(Collision other)
        {
            if (other.collider.gameObject == player)
            {
                playerInRange = true;
            }
        }


        void OnCollisionExit(Collision other)
        {
            if (other.collider.gameObject == player)
            {
                playerInRange = false;
            }
        }


        void OnCollisionStay(Collision other)
        {
            if (other.collider.gameObject == player)
            {
                playerInRange = true;
            }
        }

        void FixedUpdate()
        {
            if (playerInRange && enemyHealth.CurrentHealth > 0 && playerHealth.currentHealth > 0 
                && movement.isAttacking)
            {
                //Attack the player
                TurnVisible();
                enemyAudioLoop.Stop();
                if (!enemyAudioEffects.isPlaying)
                {
                    base.Attack();
                    enemyAudioEffects.PlayOneShot(attackClip);
                }
                anim.SetTrigger("Attack");
                movement.isAttacking = false;
            }
            else if (playerHealth.currentHealth <= 0 || enemyHealth.CurrentHealth <= 0)
            {
                //Remy is dead or the player is dead, Remy turns visible
                TurnVisible();
            }
        }

        /// <summary>
        /// Makes Remy visible, using its standard materials saved at Start function.
        /// </summary>
        void TurnVisible()
        {
            for (int i = 0; i < meshRenderers.Length; i++)
            {
                meshRenderers[i].material = remyStandardMaterials[i];
            }
        }

        /// <summary>
        /// Makes Remy invisible, using the material set
        /// </summary>
        void TurnInvisible()
        {
            foreach (SkinnedMeshRenderer renderer in meshRenderers)
            {
                renderer.material = invisibleMaterial;
            }
        }

        protected override void Attack()
        {
            base.Attack();
            movement.hasAttacked = true;
            TurnInvisible();
        }
    }
}


