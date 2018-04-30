using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using FPSDemo.Scripts.Enemy;

namespace FPSDemo.Scripts.Manager
{
    public class ScoreManager : MonoBehaviour
    {
        //Applying singleton pattern
        public static ScoreManager manager;

        //Game score properties
        //Current score
        public int score = 0;
        //Current number of enemies killed
        public int enemyKilled;
        //Current score multiplier
        [SerializeField]
        [Tooltip("Starting multiplier value")]
        private float startingMultiplier = 0.5f;
        private float scoreMultiplier;

        [SerializeField]
        int scoreToReachSecondStage = 500;

        [SerializeField]
        int scoreToReachThirdStage = 1500;


        public bool playerWasDamaged;
        bool hasChangedLevel = false;

        //Score messages, appearing on the UI when setting a score
        public static string[,] scoreMessages = new string[4, 5]
        {
            { "Serious?", "n0oB!1", "Need a 'Pezzo'?", "Woo Men", "Still Gay" },
            { "Wasted", "Rekt", "In the 'Zona'", "Almost a man", "Fucked in the ass" },
            { "Dicked Down", "Fuck, Boy!", "'Squalo' mode", "Big Donga", "Suck my Dick"},
            { "QuickFucked360", "420BlazeIt", "'Non se sa'", "Ass Breaker", "Suck Your Dick"}
        };

        public static string[] multiplierMessages =
        {
            "Go!", "Ass-Whooped", "SuckMyDickInCamelCase", "U Can't See me!"
        };

        [Header("Parameters")]
        [SerializeField]
        Text scoreText;
        [SerializeField]
        static Text scoreMsgText;
        [SerializeField]
        static Text multiplierMsgText;
        [SerializeField]
        static Slider scoreSlider;
        [SerializeField]
        float secondsToFullyDecreaseSlider;
        static Animator anim;
        Text text;


        void Awake()
        {
            //Singleton pattern
            if(ScoreManager.manager == null)
            {
                ScoreManager.manager = this;
            } else
            {
                Destroy(ScoreManager.manager);
                ScoreManager.manager = this;
            }

            //Retrieving components
            anim = GetComponent<Animator>();
            scoreMsgText = GameObject.Find("ScoreText").GetComponent<Text>();
            scoreSlider = GameObject.Find("ScoreSlider").GetComponent<Slider>();
            multiplierMsgText = GameObject.Find("MultiplierText").GetComponent<Text>();
            scoreMultiplier = startingMultiplier;
            enemyKilled = 0;
        }


        void Update()
        {
            scoreText.text = score + "";
            if (scoreSlider.value >= 0)
            {
                scoreSlider.value -= Time.deltaTime * scoreSlider.maxValue / secondsToFullyDecreaseSlider;
            }
            if (playerWasDamaged)
            {
                scoreSlider.value = 0;
                playerWasDamaged = false;
                scoreMultiplier = startingMultiplier;
            }

            //Stage change handling
            //Try something better than this shitty code
            if(score > scoreToReachSecondStage 
                && score <= scoreToReachThirdStage && !hasChangedLevel)
            {
                LevelManager.Manager.ChangeLevel();
                hasChangedLevel = true;
            } else if (score > scoreToReachThirdStage && hasChangedLevel)
            {
                LevelManager.Manager.ChangeLevel();
                hasChangedLevel = false;
            }
        }

        internal void IncreaseMultiplier()
        {
            scoreMultiplier += startingMultiplier;
            UpdateUIMultiplier();
        }

        public void ScoreIncrease(object source, EnemyDeadEventArgs args)
        {
            ScoreManager.manager.enemyKilled++;
            scoreMultiplier += scoreSlider.value / scoreSlider.maxValue;
            ScoreManager.manager.score += Convert.ToInt32(args.ScoreValue * scoreMultiplier);
            UpdateUI(args.ScoreValue);
        }

        public void ScoreIncrease(int scoreToIncrease)
        {
            ScoreManager.manager.score += Convert.ToInt32(scoreToIncrease * scoreMultiplier);
            UpdateUI(scoreToIncrease);
        }

        private void UpdateUI(int score)
        {
            int messagesIndex = Convert.ToInt32(scoreSlider.value / scoreSlider.maxValue * 3f);
            scoreMsgText.text = scoreMessages[messagesIndex, UnityEngine.Random.Range(0, 5)];
            if (scoreSlider.value <= scoreSlider.maxValue - score)
                scoreSlider.value += score;
            anim.SetTrigger("ScoreIncrease");
        }

        private void UpdateUIMultiplier()
        {
            int messagesIndex = Convert.ToInt32(UnityEngine.Random.Range(0, multiplierMessages.Length));
            multiplierMsgText.text = multiplierMessages[messagesIndex] + "x" + scoreMultiplier.ToString("n1");
            anim.SetTrigger("MultiplierIncrease");
        }

        
    }
}


