using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPSDemo.Scripts.Player
{
    public class CameraShaker : MonoBehaviour
    {

        [SerializeField]
        private float power = 0.7f;
        [SerializeField]
        private float duration = 1.0f;
        private Transform cameraTransform;
        [SerializeField]
        private float slowDownAmount = 1.0f;
        public bool shouldShake = false;

        Vector3 startPosition;
        float initialDuration;

        // Use this for initialization
        void Start()
        {
            cameraTransform = GetComponent<Camera>().transform;
            startPosition = cameraTransform.localPosition;
            initialDuration = duration;
        }

        // Update is called once per frame
        void Update()
        {
            if (shouldShake)
            {
                if (duration > 0)
                {
                    cameraTransform.localPosition = startPosition + Random.insideUnitSphere * power;
                    duration -= Time.deltaTime * slowDownAmount;
                }
                else
                {
                    shouldShake = false;
                    duration = initialDuration;
                    cameraTransform.localPosition = startPosition;
                }
            }
        }
    }
}


