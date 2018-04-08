using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

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
        public float scoreMultiplier = 0.5f;

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

        [Header("Parameters")]
        [SerializeField]
        Text scoreText;
        [SerializeField]
        static Text scoreMsgText;
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

            enemyKilled = 0;
        }


        void Update()
        {
            scoreText.text = score + "";
            if (scoreSlider.value >= 0)
            {
                scoreSlider.value -= Time.deltaTime * scoreSlider.maxValue / secondsToFullyDecreaseSlider * scoreMultiplier;
            }
            if (playerWasDamaged)
            {
                scoreSlider.value = 0;
                playerWasDamaged = false;
            }
            scoreMultiplier = 0.5f + scoreSlider.value / scoreSlider.maxValue;

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

        public void ScoreIncrease(int score)
        {
            ScoreManager.manager.score += score;
            int messagesIndex = Convert.ToInt32(scoreSlider.value / scoreSlider.maxValue * 3f);
            scoreMsgText.text = scoreMessages[messagesIndex, UnityEngine.Random.Range(0, 4)];
            if (scoreSlider.value <= scoreSlider.maxValue - score)
                scoreSlider.value += score;
            anim.SetTrigger("ScoreIncrease");
        }
    }
}


