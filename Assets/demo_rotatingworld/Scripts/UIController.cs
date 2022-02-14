using SeaberyTest.GameManagement;
using SeaberyTest.World;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace SeaberyTest.UI
{
    public class UIController : MonoBehaviour
    {
        #region INSPECTOR VARIABLES

        [Header("Initial UI")]
        
        [SerializeField]
        [Tooltip("Parent game object that includes the Initial UI section")]
        private GameObject _initialUI;

        [Header("Gameplay UI")]

        [SerializeField]
        [Tooltip("Parent game object that includes the Gameplay UI section")]
        private GameObject _gameplayUI;

        [SerializeField]
        [Tooltip("Text where the time left is showed")]
        private Text _timeLeftText;

        [SerializeField]
        [Tooltip("Text where the number of coins is showed")]
        private Text _coinsText;

        [SerializeField]
        [Tooltip("Animator of the coins icon")]
        private Animator _coinsAnimator;

        [SerializeField]
        [Tooltip("Restart Game button")]
        private Button _restartButton;

        [Header("Game Over UI")]

        [SerializeField]
        [Tooltip("Parent game object that includes the Game Over UI section")]
        private GameObject _gameOverUI;

        [SerializeField]
        [Tooltip("Try Again button (Game Over section)")]
        private Button _gameOverTryAgainButton;
        
        [Header("Player Win UI")]

        [SerializeField]
        [Tooltip("Parent game object that includes the Player Win UI section")]
        private GameObject _playerWinUI;

        [SerializeField]
        [Tooltip("Try Again button (Player Win section)")]
        private Button _playerWinTryAgainButton;

        [Header("Restart Window UI")]

        [SerializeField]
        [Tooltip("Parent game object that includes the Restart Window UI section")]
        private GameObject _restartWindowUI;

        [SerializeField]
        [Tooltip("Confirm Restar Game button")]
        private Button _confirmButton;
        
        [SerializeField]
        [Tooltip("Cancel Restar Game button")]
        private Button _cancelButton;

        #endregion

        #region PRIVATE VARIABLES
        
        private TimeSpan _currentTime;

        #endregion

        #region UNITY EVENTS

        private void Awake()
        {
            // Events subscriptions
            WorldController.OnWorldCreated += OnWorldCreated;
            GameController.OnNewCoinTaked += OnNewCoinTaked;
            GameController.OnGameOver += OnGameOver;
            GameController.OnWin += OnWin;

            // Adds listeners to all the UI buttons
            _restartButton.onClick.AddListener(OnRestartGame);
            _gameOverTryAgainButton.onClick.AddListener(OnTryAgain);
            _playerWinTryAgainButton.onClick.AddListener(OnTryAgain);
            _confirmButton.onClick.AddListener(OnTryAgain);
            _cancelButton.onClick.AddListener(OnCancelRestartGame);

            ShowInitialUI();
        }

        private void Start()
        {
            UpdateCoins(0, GameController.Instance.TotalCoinsToWin);
        }

        private void Update()
        {
            UpdateTimeLeft();
        }

        private void OnDestroy()
        {
            // Events unsubscriptions
            WorldController.OnWorldCreated -= OnWorldCreated;
            GameController.OnNewCoinTaked -= OnNewCoinTaked;
            GameController.OnGameOver -= OnGameOver;
            GameController.OnWin -= OnWin;

            // Removes listeners of all the UI buttons
            _restartButton.onClick.RemoveListener(OnRestartGame);
            _gameOverTryAgainButton.onClick.RemoveListener(OnTryAgain);
            _playerWinTryAgainButton.onClick.RemoveListener(OnTryAgain);
            _confirmButton.onClick.RemoveListener(OnTryAgain);
            _cancelButton.onClick.RemoveListener(OnCancelRestartGame);
        }

        #endregion

        #region PRIVATE METHODS

        /// <summary>
		/// Once the world has been instantiated in the AR scene, it shows the Gameplay UI.
		/// </summary>
		/// <param name="worldReference">Reference to the instantiated world</param>
        private void OnWorldCreated(Transform worldReference)
        {
            ShowGameplayUI();
        }

        /// <summary>
		/// Each time the player pick up a coin, it updates the coins counter UI.
		/// </summary>
		/// <param name="currentCoins">Number of coins currently taken</param>
        private void OnNewCoinTaked(int currentCoins)
        {
            UpdateCoins(currentCoins, GameController.Instance.TotalCoinsToWin);
            _coinsAnimator.SetTrigger("CoinWon");
        }

        /// <summary>
        /// When the player loses, it shows the Game Over UI.
        /// </summary>
        private void OnGameOver()
        {
            GameController.Instance.GameIsPaused = true;
            ShowGameOverUI();
        }

        /// <summary>
        /// When the player wins, it shows the Player Win UI.
        /// </summary>
        private void OnWin()
        {
            GameController.Instance.GameIsPaused = true;
            ShowPlayerWinUI();
        }

        /// <summary>
        /// When the user clicks on the Restart Game button, it shows the Restart Game UI.
        /// </summary>
        private void OnRestartGame()
        {
            GameController.Instance.GameIsPaused = true;
            ShowRestartWindowUI();
        }

        /// <summary>
        /// When the user confirms about restart the game, it notifies to the GameController in order to restart the game logic.
        /// </summary>
        private void OnTryAgain()
        {
            GameController.Instance.InitializeGame();
            UpdateCoins(0, GameController.Instance.TotalCoinsToWin);
            ShowInitialUI();
        }

        /// <summary>
        /// If the user cancel the game restarting, it shows the Gameplay UI again.
        /// </summary>
        private void OnCancelRestartGame()
        {
            GameController.Instance.GameIsPaused = false;
            ShowGameplayUI();
        }

        /// <summary>
        /// Shows the Initial UI and deactivates the others.
        /// </summary>
        private void ShowInitialUI()
        {
            _initialUI.SetActive(true);
            _gameplayUI.SetActive(false);
            _gameOverUI.SetActive(false);
            _playerWinUI.SetActive(false);
            _restartWindowUI.SetActive(false);
        }

        /// <summary>
        /// Shows the Gameplay UI and deactivates the others.
        /// </summary>
        private void ShowGameplayUI()
        {
            _initialUI.SetActive(false);
            _gameplayUI.SetActive(true);
            _gameOverUI.SetActive(false);
            _playerWinUI.SetActive(false);
            _restartWindowUI.SetActive(false);
        }

        /// <summary>
        /// Shows the Game Over UI and deactivates the others.
        /// </summary>
        private void ShowGameOverUI()
        {
            _initialUI.SetActive(false);
            _gameplayUI.SetActive(false);
            _gameOverUI.SetActive(true);
            _playerWinUI.SetActive(false);
            _restartWindowUI.SetActive(false);
        }

        /// <summary>
        /// Shows the Player Win UI and deactivates the others.
        /// </summary>
        private void ShowPlayerWinUI()
        {
            _initialUI.SetActive(false);
            _gameplayUI.SetActive(false);
            _gameOverUI.SetActive(false);
            _playerWinUI.SetActive(true);
            _restartWindowUI.SetActive(false);
        }

        /// <summary>
        /// Shows the Restart Window UI and deactivates the others.
        /// </summary>
        private void ShowRestartWindowUI()
        {
            _initialUI.SetActive(false);
            _gameplayUI.SetActive(false);
            _gameOverUI.SetActive(false);
            _playerWinUI.SetActive(false);
            _restartWindowUI.SetActive(true);
        }

        /// <summary>
        /// Updates the time left for the game ending in the time left UI.
        /// </summary>
        private void UpdateTimeLeft()
        {
            _currentTime = TimeSpan.FromSeconds(GameController.Instance.TimeLeft);

            _timeLeftText.text = string.Format("{0:00}:{1:00}",
                _currentTime.Minutes,
                _currentTime.Seconds);
        }

        /// <summary>
        /// Updates the coin counter UI.
        /// </summary>
        /// <param name="currentCoins"></param>
        /// <param name="totalCoins"></param>
        private void UpdateCoins(int currentCoins, int totalCoins)
        {
            _coinsText.text = string.Format("{0}/{1}",
                currentCoins,
                totalCoins);
        } 

        #endregion
    }
}