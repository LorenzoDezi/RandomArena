using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FPSDemo.Scripts.Weapons;

namespace FPSDemo.Scripts.Animations
{

    public class ReloadState : StateMachineBehaviour
    {

        public float ReloadTime;
        protected bool hasReloaded;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            hasReloaded = false;
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            if (hasReloaded) return;
            if (animatorStateInfo.normalizedTime >= ReloadTime)
            {
                animator.GetComponent<Weapon>().Reload();
                animator.SetBool("Reload", false);
                hasReloaded = true;
            }

        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            hasReloaded = false;
        }
    }
}

