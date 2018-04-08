using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FPSDemo.Scripts.Animations
{
    public class LimanaAnimatorCallback : MonoBehaviour
    {
        [SerializeField]
        private AudioClip footstepSound;
        [SerializeField]
        private AudioClip backFootstepSound;
        [SerializeField]
        private ParticleSystem dustEffect;
        [SerializeField]
        private Transform limanaZampa;
        AudioSource enemyAudio1;
        AudioSource enemyAudio2;

        private void Start()
        {
            enemyAudio1 = this.GetComponents<AudioSource>()[0];
            enemyAudio2 = this.GetComponents<AudioSource>()[1];
        }

        public void PlayFootStep()
        {
            enemyAudio2.PlayOneShot(footstepSound);
            ParticleSystem dustParticle = GameObject.Instantiate(dustEffect, limanaZampa.position, limanaZampa.rotation);
            dustParticle.Play();
            Destroy(dustParticle.gameObject, 2f);
        }

        public void PlayBackFootSteps()
        {
            enemyAudio1.PlayOneShot(backFootstepSound);
            Invoke("PlayBackFootStep", 0.25f);
        }

        public void PlayBackFootStep()
        {
            enemyAudio2.PlayOneShot(backFootstepSound);
        }
    }
}


