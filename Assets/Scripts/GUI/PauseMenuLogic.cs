using System.Collections;
using System.Collections.Generic;
using FPSDemo.Scripts.Player;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FPSDemo.Scripts.UI
{
    public class PauseMenuLogic : MonoBehaviour
    {

        public static bool isPaused;

        [SerializeField]
        GameObject pauseMenu;

        [Tooltip("The mainPlayer character controller")]
        [SerializeField]
        MyFirstPersonController controller;

	void Start()
        {
            pauseMenu.SetActive(false);
            isPaused = false;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (isPaused)
                {
                    Resume();
                }
                else
                {
                    Pause();
                }
            }
        }

        public void Resume()
        {
            pauseMenu.SetActive(false);
            Time.timeScale = 1f;
            Cursor.visible = isPaused = false;
            controller.enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
        }

        public void Pause()
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
            Cursor.visible = isPaused = true;
            controller.enabled = false;
            Cursor.lockState = CursorLockMode.Confined;
        }

        public void Exit()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(0, LoadSceneMode.Single);
        }
    }
}


