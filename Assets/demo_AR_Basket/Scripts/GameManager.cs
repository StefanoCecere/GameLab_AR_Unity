using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace demo.basket
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] GameObject GameplayUI;
        [SerializeField] GameObject GameOverPanel;
        [SerializeField] GameObject startButton;
        [SerializeField] GameObject ScanLabelUI;

        [SerializeField] float totalTime;

        PlaneObjectSpawner basketSpawner;
        ObjectThrower objectThrower;
        ScoreManager scoreManager;
        Timer timer;

        bool isScanComplete;

        TextMeshProUGUI scoreUI;
        TextMeshProUGUI bestScoreUI;
        TextMeshProUGUI accuracyUI;

        public enum GameState
        {
            Scanning,
            Gameplay,
            GameOver
        }
        public GameState currentGameState { get; private set; }

        // Start is called before the first frame update
        void Awake()
        {
            timer = FindObjectOfType<Timer>();
            timer.totalTime = totalTime;
            scoreManager = FindObjectOfType<ScoreManager>();
            objectThrower = FindObjectOfType<ObjectThrower>();

            timer.TimerFinishedEvent += GameSessionEnd;

            basketSpawner = FindObjectOfType<PlaneObjectSpawner>();

            scoreUI = GameOverPanel.transform.Find("Score").GetComponent<TextMeshProUGUI>();
            bestScoreUI = GameOverPanel.transform.Find("BestScore").GetComponent<TextMeshProUGUI>();
            accuracyUI = GameOverPanel.transform.Find("Accuracy").GetComponent<TextMeshProUGUI>();

            objectThrower.enabled = false;
            startButton.SetActive(false);
            currentGameState = GameState.Scanning;
        }

        private void Update()
        {
            if (basketSpawner.isEnvironmentReady && !isScanComplete) {
                ScanLabelUI.SetActive(false);
                startButton.SetActive(true);
            }
        }

        public void StartSession()
        {
            currentGameState = GameState.Gameplay;
            objectThrower.enabled = true;
            startButton.gameObject.SetActive(false);
            basketSpawner.enableSpawning = true;
            isScanComplete = true;
        }

        void GameSessionEnd()
        {
            StartCoroutine(EndSessionWithDelay(2));
        }

        IEnumerator EndSessionWithDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            currentGameState = GameState.GameOver;

            GameplayUI.SetActive(false);
            GameOverPanel.SetActive(true);
            objectThrower.enabled = false;

            scoreManager.SetBestScore();
            float accuracy = scoreManager.CalculateAccuracy();

            scoreUI.SetText(scoreManager.score.ToString());
            bestScoreUI.SetText(scoreManager.bestScore.ToString());
            string accuracyStr = accuracy.ToString("N1") + "%";
            accuracyUI.SetText(accuracyStr.Replace(",", "."));
        }

        public void RestartSession()
        {
            currentGameState = GameState.Gameplay;

            GameplayUI.SetActive(true);
            GameOverPanel.SetActive(false);
            objectThrower.enabled = true;
            objectThrower.ResetThrower();
            timer.ResetTimer();
            scoreManager.ResetScore();
        }

    }
}
