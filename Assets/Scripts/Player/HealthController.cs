using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using FPSDemo.Scripts.Manager;

namespace FPSDemo.Scripts.Player
{
    /// <summary>
    /// This script contains logic for player health.
    /// </summary>
    public class HealthController : MonoBehaviour
    {
        public int startingHealth = 100;
        public int currentHealth;

        private float timeStartedLerping;
        private float lerpingPercentage;
        public float timeTakenDuringLerp = 1f;
        private AudioSource audioSource;
        Vector3 displacementPosition;
        [SerializeField]
        Vector3 cameraIfDeadPosition;
        [SerializeField]
        GameObject toDestroyIfDie;
        [SerializeField]
        private Image damageImage;
        [SerializeField]
        Slider healthSlider;
        private bool damaged;
        [SerializeField]
        public AudioClip[] deathClip;
        [SerializeField]
        public AudioClip[] damageClip;
        public float flashSpeed = 5f;
        public Color flashColour = new Color(1f, 0f, 0f, 0.1f);

        [HideInInspector]
        public bool isBeingCharged;
        bool isDead;

        void Awake()
        {
            currentHealth = startingHealth;
            audioSource = this.GetComponents<AudioSource>()[1];

        }

        private void Start()
        {
            healthSlider.maxValue = startingHealth;
            healthSlider.value = startingHealth;
        }


        void Update()
        {
            if (damaged && !isDead)
            {
                damageImage.color = flashColour;
                audioSource.PlayOneShot(damageClip[Random.Range(0, damageClip.Length)]);
            }
            else if (!isDead)
            {
                damageImage.color = Color.Lerp(damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
            }
            damaged = false;
        }

        public void TakeDamage(int amount)
        {
            currentHealth -= amount;
            healthSlider.value = currentHealth;
            damaged = true;
            ScoreManager.manager.playerWasDamaged = true;
            if (currentHealth <= 0 && !isDead)
            {
                Death();
            }
        }

        private Vector3 GraduallyLerpToPosition(Vector3 startPosition, 
            Vector3 newPosition, 
            float speed)
        {
            float timeSinceStarted = Time.time - timeStartedLerping;
            float percentageComplete = (timeSinceStarted / timeTakenDuringLerp) * speed;
            return Vector3.Lerp(startPosition, newPosition,
               percentageComplete);
        }

        public void TakeCharge(int damage)
        {
            timeStartedLerping = Time.time;
            isBeingCharged = true;
            displacementPosition = transform.position + new Vector3(0, 5, 0);
            TakeDamage(damage);
        }

        public void RechargeHealth(int amount)
        {
            if (currentHealth <= startingHealth)
                currentHealth += Mathf.Clamp(amount,0,startingHealth);
            healthSlider.value = currentHealth;
        }

        private void FixedUpdate()
        {
            if (isBeingCharged)
            {
                this.GetComponentInChildren<CameraShaker>().shouldShake = true;
                transform.position = GraduallyLerpToPosition(transform.position, displacementPosition, 3f);
                if (transform.position == displacementPosition)
                {
                    isBeingCharged = false;
                }
            }

            if (isDead)
            {
                //Input shutdown
                this.GetComponent<MyFirstPersonController>().enabled =  
                    this.GetComponent<CharacterController>().enabled = 
                    false;

                //Camera Lerp
                Camera camera = this.GetComponentInChildren<Camera>();
                if (camera.transform.localPosition != cameraIfDeadPosition)
                    camera.transform.localPosition = GraduallyLerpToPosition(
                        camera.transform.localPosition, cameraIfDeadPosition, 1f);


            }
        }

        void Death()
        {
            timeStartedLerping = Time.time;
            damageImage.color = flashColour;
            isDead = true;
            Destroy(toDestroyIfDie);
            audioSource.PlayOneShot(deathClip[Random.Range(0, deathClip.Length)]);
        }

    }
}


