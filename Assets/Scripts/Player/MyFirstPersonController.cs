using System;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;
using Random = UnityEngine.Random;
using FPSDemo.Scripts.Weapons;

namespace FPSDemo.Scripts.Player
{
    /// <summary>
    /// This script controls the player. The code is taken directly from FirstPersonController from
    /// UnityStandardAssets.Characters.FirstPerson, with some modifies.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(AudioSource))]
    public class MyFirstPersonController : MonoBehaviour
    {
        [Header("First Person Logic")]
        [SerializeField] private bool m_IsWalking;
        [SerializeField] private float m_WalkSpeed;
        [SerializeField] private float m_RunSpeed;
        [SerializeField] [Range(0f, 1f)] private float m_RunstepLenghten;
        [SerializeField] private float m_JumpSpeed;
        [SerializeField] private float m_StickToGroundForce;
        [SerializeField] private float m_GravityMultiplier;
        [SerializeField] private MouseLook m_MouseLook;
        [SerializeField] private bool m_UseFovKick;
        [SerializeField] private FOVKick m_FovKick = new FOVKick();
        [SerializeField] private bool m_UseHeadBob;
        [SerializeField] private CurveControlledBob m_HeadBob = new CurveControlledBob();
        [SerializeField] private LerpControlledBob m_JumpBob = new LerpControlledBob();
        [SerializeField] private float m_StepInterval;
        [SerializeField] private float m_dodgeIncreaseSpeed;

        [Header("Sounds/Effects")]
        [SerializeField] private AudioClip[] m_FootstepSounds;     // an array of footstep sounds that will be randomly selected from.
        [SerializeField] private AudioClip m_JumpSound;           // the sound played when character leaves the ground.
        [SerializeField] private AudioClip m_LandSound;         // the sound played when character touches back on ground.
        [SerializeField] private AudioClip m_DodgeSound;        // the sound played when character dodges

        [Header("Dodge Logic")]
        [SerializeField] public int dodgeMax;
        [SerializeField] public int dodgeLeft;
        //Time taken to recover from a dodge
        [SerializeField] private float timeToRecoverDodge;
        //Dodge private fields
        private float RecoverDodgeTimer;
        private bool m_Dodge;
        private bool m_Dodging;


        [Header("Weapons")]
        [SerializeField] private Weapon weapon;
        [SerializeField] private RawImage[] dodgeIcons;
        

        private Camera m_Camera;
        private bool m_Jump;
        private float m_YRotation;
        private Vector2 m_Input;
        private Vector3 m_MoveDir = Vector3.zero;
        private CharacterController m_CharacterController;
        private CollisionFlags m_CollisionFlags;
        private bool m_PreviouslyGrounded;
        private Vector3 m_OriginalCameraPosition;
        private float m_StepCycle;
        private float m_NextStep;
        private bool m_Jumping;
        private AudioSource m_AudioSource;
        public new bool enabled;

        // Use this for initialization
        private void Start()
        {
            m_CharacterController = GetComponent<CharacterController>();
            m_Camera = Camera.main;
            m_OriginalCameraPosition = m_Camera.transform.localPosition;
            m_FovKick.Setup(m_Camera);
            m_HeadBob.Setup(m_Camera, m_StepInterval);
            m_StepCycle = 0f;
            m_NextStep = m_StepCycle / 2f;
            m_Jumping = false;
            m_AudioSource = GetComponent<AudioSource>();
            m_MouseLook.Init(transform, m_Camera.transform);

            //Dodge Logic
            dodgeLeft = dodgeMax;
            RecoverDodgeTimer = 0f;
            //We listen for an event of weaponSwitching
            this.GetComponent<WeaponManager>().SwitchEvent += WeaponSwitchedHandler;
            enabled = true;
        }

        /// <summary>
        /// Listen for a switch weapon event
        /// </summary>
        public void WeaponSwitchedHandler(object sender, WeaponEventArgs args)
        {
            weapon = args.Weapon;
        }

        /// <summary>
        /// Recover a dodge
        /// </summary>
        public void IncreaseDodgeLeft()
        {
            dodgeLeft++;
            this.UpdateUI(false);
        }

        /// <summary>
        /// Using a dodge
        /// </summary>
        public void DecreaseDodgeLeft()
        {
            dodgeLeft--;
            this.UpdateUI(true);
        }


        // Update is called once per frame
        private void Update()
        {
            if (enabled)
            {
                RotateView();
                // the jump state needs to read here to make sure it is not missed
                if (!m_Jump)
                {
                    m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
                }

                if (!m_Dodge)
                {
                    m_Dodge = CrossPlatformInputManager.GetButtonDown("Dodge");
                }

                if (dodgeLeft < dodgeMax)
                {
                    RecoverDodgeTimer += Time.deltaTime;
                    if (RecoverDodgeTimer >= timeToRecoverDodge)
                    {
                        IncreaseDodgeLeft();
                        //After the first dodge recharge, the others load faster;
                        RecoverDodgeTimer = timeToRecoverDodge - 0.5f;
                    }
                }

                if (!m_PreviouslyGrounded && m_CharacterController.isGrounded)
                {
                    StartCoroutine(m_JumpBob.DoBobCycle());
                    PlayLandingSound();
                    m_MoveDir.y = 0f;
                    m_Jumping = false;
                }
                if (!m_CharacterController.isGrounded && !m_Jumping && m_PreviouslyGrounded)
                {
                    m_MoveDir.y = 0f;
                }
                m_PreviouslyGrounded = m_CharacterController.isGrounded;

            }


        }


        private void PlayLandingSound()
        {
            m_AudioSource.clip = m_LandSound;
            m_AudioSource.Play();
            m_NextStep = m_StepCycle + .5f;
        }

        /// <summary>
        /// Update user interface with the current number of dodge
        /// </summary>
        /// <param name="dodged"> true if you have just dodged </param>
        private void UpdateUI(bool dodged)
        {
            if (dodged)
            {
                dodgeIcons[dodgeLeft].canvasRenderer.SetAlpha(1f);
                dodgeIcons[dodgeLeft].CrossFadeAlpha(0f, 0.5f, true);
            }

            else
            {
                dodgeIcons[dodgeLeft - 1].canvasRenderer.SetAlpha(0f);
                dodgeIcons[dodgeLeft - 1].CrossFadeAlpha(100f, 0.5f, true);

            }

        }


        private void FixedUpdate()
        {
            float speed;
            GetInput(out speed);
            if (enabled)
            {
                // always move along the camera forward as it is the direction that it being aimed at
                Vector3 desiredMove = transform.forward * m_Input.y + transform.right * m_Input.x;

                // get a normal for the surface that is being touched to move along it
                RaycastHit hitInfo;
                Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo,
                                   m_CharacterController.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);

                desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;
                //Simple increase of movement speed if dodge
                if (m_Dodge && (m_Input != Vector2.zero) && dodgeLeft > 0)
                {
                    desiredMove = desiredMove * m_dodgeIncreaseSpeed;
                    GetComponents<AudioSource>()[1].PlayOneShot(m_DodgeSound);
                    DecreaseDodgeLeft();
                    //When you use a dodge, the recharge restarts slow
                    RecoverDodgeTimer = 0f;
                }
                m_MoveDir.x = desiredMove.x * speed;
                m_MoveDir.z = desiredMove.z * speed;

                if (m_CharacterController.isGrounded)
                {
                    m_MoveDir.y = -m_StickToGroundForce;

                    if (m_Jump)
                    {
                        m_MoveDir.y = m_JumpSpeed;
                        PlayJumpSound();
                        m_Jump = false;
                        m_Jumping = true;
                    }
                }
                else
                {
                    if (m_Dodge)
                    {
                        m_MoveDir += Physics.gravity * m_GravityMultiplier * Time.fixedDeltaTime;

                    }
                    else
                    {
                        m_MoveDir += Physics.gravity * m_GravityMultiplier * Time.fixedDeltaTime;
                    }
                }
                m_CollisionFlags = m_CharacterController.Move(m_MoveDir * Time.fixedDeltaTime);

                ProgressStepCycle(speed);
                UpdateCameraPosition(speed);

                m_MouseLook.UpdateCursorLock();
                m_Dodge = false;
            }
        }


        private void PlayJumpSound()
        {
            m_AudioSource.clip = m_JumpSound;
            m_AudioSource.Play();
        }


        private void ProgressStepCycle(float speed)
        {
            if (m_CharacterController.velocity.sqrMagnitude > 0 && (m_Input.x != 0 || m_Input.y != 0))
            {
                m_StepCycle += (m_CharacterController.velocity.magnitude + (speed * (m_IsWalking ? 1f : m_RunstepLenghten))) *
                             Time.fixedDeltaTime;
            }

            if (!(m_StepCycle > m_NextStep))
            {
                return;
            }

            m_NextStep = m_StepCycle + m_StepInterval;

            PlayFootStepAudio();
        }


        private void PlayFootStepAudio()
        {
            if (!m_CharacterController.isGrounded || m_Dodge)
            {
                return;
            }
            // pick & play a random footstep sound from the array,
            // excluding sound at index 0
            int n = Random.Range(1, m_FootstepSounds.Length);
            m_AudioSource.clip = m_FootstepSounds[n];
            m_AudioSource.PlayOneShot(m_AudioSource.clip);
            // move picked sound to index 0 so it's not picked next time
            m_FootstepSounds[n] = m_FootstepSounds[0];
            m_FootstepSounds[0] = m_AudioSource.clip;
        }


        private void UpdateCameraPosition(float speed)
        {
            Vector3 newCameraPosition;
            if (!m_UseHeadBob)
            {
                return;
            }
            if (m_CharacterController.velocity.magnitude > 0 && m_CharacterController.isGrounded)
            {
                m_Camera.transform.localPosition =
                    m_HeadBob.DoHeadBob(m_CharacterController.velocity.magnitude +
                                      (speed * (m_IsWalking ? 1f : m_RunstepLenghten)));
                newCameraPosition = m_Camera.transform.localPosition;
                newCameraPosition.y = m_Camera.transform.localPosition.y - m_JumpBob.Offset();
            }
            else
            {
                newCameraPosition = m_Camera.transform.localPosition;
                newCameraPosition.y = m_OriginalCameraPosition.y - m_JumpBob.Offset();
            }
            m_Camera.transform.localPosition = newCameraPosition;
        }


        private void GetInput(out float speed)
        {
            // Read input
            float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
            float vertical = CrossPlatformInputManager.GetAxis("Vertical");

            bool waswalking = m_IsWalking;

#if !MOBILE_INPUT
            // On standalone builds, walk/run speed is modified by a key press.
            // keep track of whether or not the character is walking or running
            //You can only run forward
            m_IsWalking = !Input.GetKey(KeyCode.LeftShift) || (vertical < 0 || horizontal != 0);
#endif
            // set the desired speed to be walking or running
            speed = m_IsWalking ? m_WalkSpeed : m_RunSpeed;
            m_Input = new Vector2(horizontal, vertical);

            // normalize input if it exceeds 1 in combined length:
            if (m_Input.sqrMagnitude > 1)
            {
                m_Input.Normalize();
            }

            // handle speed change to give an fov kick
            // only if the player is going to a run, is running and the fovkick is to be used
            if (m_IsWalking != waswalking && m_CharacterController.velocity.sqrMagnitude > 0)
            {
                if (m_UseFovKick)
                {
                    StopAllCoroutines();
                    StartCoroutine(!m_IsWalking ? m_FovKick.FOVKickUp() : m_FovKick.FOVKickDown());
                }
                weapon.StartRunning(m_IsWalking);

            }
        }


        private void RotateView()
        {
            m_MouseLook.LookRotation(transform, m_Camera.transform);
        }


        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            Rigidbody body = hit.collider.attachedRigidbody;
            //dont move the rigidbody if the character is on top of it
            if (m_CollisionFlags == CollisionFlags.Below)
            {
                return;
            }

            if (body == null || body.isKinematic)
            {
                return;
            }
            body.AddForceAtPosition(m_CharacterController.velocity * 0.1f, hit.point, ForceMode.Impulse);
        }
    }
}


