using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FPSDemo.Scripts.UI
{
    public class MenuSceneInteractionFunctions : MonoBehaviour
    {
        [Header("Main panels")]
        [SerializeField]
        [Tooltip("The main panel")]
        private GameObject mainPanel;

        [SerializeField]
        [Tooltip("The help panel")]
        private GameObject helpPanel;

        [SerializeField]
        [Tooltip("The option panel")]
        private GameObject optionPanel;

        [SerializeField]
        [Tooltip("The info panel")]
        private GameObject infoPanel;

        [SerializeField]
        [Tooltip("The credits page panel")]
        private GameObject creditsPanel;

        private GameObject currentSelectedPanel;

        private void Start()
        {
            LoadMainPanel();
        }

        /// <summary>
        /// Loads the scene specified by the index parameter
        /// </summary>
        /// <param name="index">as the scene index</param>
        public void LoadSceneByIndex(int index)
        {
            SceneManager.LoadScene(index, LoadSceneMode.Single);
        }

        /// <summary>
        /// Loads the main panel
        /// </summary>
        public void LoadMainPanel()
        {
            helpPanel.SetActive(false);
            optionPanel.SetActive(false);
            infoPanel.SetActive(false);
            creditsPanel.SetActive(false);
            mainPanel.SetActive(true);
            currentSelectedPanel = mainPanel;
        }

        /// <summary>
        /// Loads the option panel
        /// </summary>
        public void LoadOptionPanel()
        {
            currentSelectedPanel.SetActive(false);
            optionPanel.SetActive(true);
            currentSelectedPanel = optionPanel;
        }

        /// <summary>
        /// Close the application
        /// </summary>
        public void CloseApplication()
        {
            Application.Quit();
        }

        /// <summary>
        /// Loads the help panel
        /// </summary>
        public void LoadHelpPanel()
        {
            currentSelectedPanel.SetActive(false);
            helpPanel.SetActive(true);
            currentSelectedPanel = helpPanel;
        }

        /// <summary>
        /// Loads the credits panel
        /// </summary>
        public void LoadCreditsPanel()
        {
            currentSelectedPanel.SetActive(false);
            creditsPanel.SetActive(true);
            currentSelectedPanel = creditsPanel;
        }

        /// <summary>
        /// Loads the info panel
        /// </summary>
        public void LoadInfoPanel()
        {
            currentSelectedPanel.SetActive(false);
            infoPanel.SetActive(true);
            currentSelectedPanel = infoPanel;
        }
        
    }

}


