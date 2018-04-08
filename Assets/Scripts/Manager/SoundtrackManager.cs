using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPSDemo.Scripts.Manager
{
    public class SoundtrackManager : MonoBehaviour
    {
        public static SoundtrackManager Manager;

        [Header("Game tracks and clips")]
        [SerializeField]
        AudioClip[] ambientTracks;
        [SerializeField]
        AudioClip[] changeLevelSound;

        AudioSource ambientAudioSource;



        //Current track index
        int index = 0;

        void Start()
        {
            if(SoundtrackManager.Manager != null)
            {
                Destroy(SoundtrackManager.Manager.gameObject);
            }
            SoundtrackManager.Manager = this;
            ambientAudioSource = this.GetComponent<AudioSource>();
            if(ambientTracks != null && ambientTracks.Length > 0)
            {
                ambientAudioSource.clip = ambientTracks[index];
                ambientAudioSource.Play();
            }
            LevelManager.Manager.ChangeLevelEv += ChangeLevelSound;
        }

        /// <summary>
        /// The audio effect starting at a level change
        /// </summary>
        public void ChangeLevelSound()
        {
            AudioClip clip =
                changeLevelSound[Random.Range(0, changeLevelSound.Length)];
            ambientAudioSource.PlayOneShot(clip);
        }

        /// <summary>
        /// Change ambient musics. It happens at a change of stage.
        /// </summary>
        public void ChangeAmbientMusic()
        {
            index++;
            if(index >= ambientTracks.Length)
            {
                index = 0;
            }
            ambientAudioSource.clip = ambientTracks[index];
            ambientAudioSource.Play();
        }
    }
}


