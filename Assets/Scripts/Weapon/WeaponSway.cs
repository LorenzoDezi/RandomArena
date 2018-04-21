using FPSDemo.Scripts.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.CrossPlatformInput;

namespace FPSDemo.Scripts.Weapons
{

    public class WeaponSway : MonoBehaviour
    {
        [Header("Sway Parameters")]
        public float amount;
        public float maxAmountX;
        public float maxAmountY;
        public float smoothAmount;
        public float timeTakenDuringLerp = 1f;


        //Various positions
        private Vector3 initialPosition;
        private float timeStartedLerping;

        [Header("Cameras")]
        [SerializeField]
        private Camera camera1ToBlur;
        [SerializeField]
        private Camera camera2ToBlur;
        [Header("Player")]
        [SerializeField]
        CharacterController player;

        void Start()
        {
            initialPosition = transform.localPosition;
            player = this.GetComponentInParent<CharacterController>();
        }

        void FixedUpdate()
        {
                float movementX = Input.GetAxis("Mouse X") * amount;
                float movementY = Input.GetAxis("Mouse Y") * amount;
                movementX = Mathf.Clamp(movementX, -maxAmountX, maxAmountX);
                movementY = Mathf.Clamp(movementY, -maxAmountY, maxAmountY);

                Vector3 newPosition = new Vector3(movementX, movementY, 0);
                transform.localPosition = Vector3.Lerp(transform.localPosition, newPosition + initialPosition,
                    Time.deltaTime * smoothAmount);
        }

    }
}



