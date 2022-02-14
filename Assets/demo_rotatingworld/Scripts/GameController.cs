using SeaberyTest.Common;
using UnityEngine;

namespace SeaberyTest.GameManagement
{
	/// <summary>
	/// Class in charge of managing the main logic of the game (Singleton class).
	/// </summary>
	[RequireComponent(typeof(AudioSource))]
	public class GameController : MonoSingleton<GameController>
	{
		#region INSPECTOR VARIABLES

		[Tooltip("Total time for game ending")]
		[SerializeField]
		private float _totalTime = 60f;

		[Tooltip("Number of coins needed for win")]
		[SerializeField]
		private int _totalCoinsToWin = 15; 

		#endregion

		#region PRIVATE VARIABLES

		private AudioSource _musicAudioSource;
		private float _timeLeft;
		private int _wonCoins;
		private bool _gameIsPaused;

		#endregion

		#region PUBLIC PROPERTIES

		/// <summary>
		/// Number of coins needed for win.
		/// </summary>
		public int TotalCoinsToWin { get => _totalCoinsToWin; }

		/// <summary>
		/// Time left for game ending.
		/// </summary>
		public float TimeLeft { get => _timeLeft; set => _timeLeft = value; }

		/// <summary>
		/// Current coins picked up by the player.
		/// </summary>
		public int WonCoins { get => _wonCoins; }

		/// <summary>
		/// Pause/unpause the main logic of the game.
		/// </summary>
		public bool GameIsPaused { get => _gameIsPaused; set => _gameIsPaused = value; }

		#endregion

		#region PUBLIC EVENTS

		public delegate void GameInitialized();
		public delegate void NewCoinTaked(int currentCoins);
		public delegate void GameOver();
		public delegate void Win();

		/// <summary>
		/// Executed when the game is initialized for the first time or restarted.
		/// </summary>
		public static event GameInitialized OnGameInitialized;

		/// <summary>
		/// Executed when the player pick up a coin.
		/// </summary>
		public static event NewCoinTaked OnNewCoinTaked;

		/// <summary>
		/// Executed when the player lose.
		/// </summary>
		public static event GameOver OnGameOver;

		/// <summary>
		/// Executed when the player win.
		/// </summary>
		public static event Win OnWin;

		#endregion

		#region SINGLETON INITIALIZATION
		
		/// <summary>
		/// Class initialization.
		/// </summary>
		protected override void Init()
		{
			base.Init();
			_musicAudioSource = GetComponent<AudioSource>();
		} 

		#endregion

		#region UNITY EVENTS

		private void Start()
		{
			InitializeGame();
		}

		private void Update()
		{
			if (!_gameIsPaused)
			{
				UpdateTimeLeft();
			}
		}

		#endregion

		#region PRIVATE METHODS

		/// <summary>
		/// Update the time left for the game ending.
		/// </summary>
		private void UpdateTimeLeft()
		{
			_timeLeft -= Time.deltaTime;
			if (_timeLeft <= 0f)
			{
				_timeLeft = 0f;
				PlayerLose();
			}
			else if (_wonCoins >= _totalCoinsToWin)
			{
				PlayerWin();
			}
		}

		/// <summary>
		/// Set the game state as 'Game Over'.
		/// It will notify about it through the 'OnGameOver' event to the rest of the classes that have subscribed.
		/// </summary>
		private void PlayerLose()
		{
			_gameIsPaused = true;

			if (OnGameOver != null)
			{
				OnGameOver();
			}
		}

		/// <summary>
		/// Set the game state as 'Player Win'.
		/// It will notify about it through the 'OnWin' event to the rest of the classes that have subscribed.
		/// </summary>
		private void PlayerWin()
		{
			_gameIsPaused = true;

			if (OnWin != null)
			{
				OnWin();
			}
		}

		#endregion

		#region PUBLIC METHODS

		/// <summary>
		/// Initialize/restart the main logic of the game.
		/// It will notify about it through the 'OnGameInitialized' event to the rest of the classes that have subscribed.
		/// </summary>
		public void InitializeGame()
		{
			_timeLeft = _totalTime;
			_wonCoins = 0;
			_gameIsPaused = true;
			_musicAudioSource.Stop();

			if (OnGameInitialized != null)
			{
				OnGameInitialized();
			}
		}

		/// <summary>
		/// Play the background music and unpause the game just when the world has been instantiated in the AR scene.
		/// </summary>
		public void StartGameplay()
		{
			_musicAudioSource.Play();
			_gameIsPaused = false;
		}

		/// <summary>
		/// Increment the player coins counter.
		/// It will notify about it through the 'OnNewCoinTaked' event to the rest of the classes that have subscribed.
		/// </summary>
		public void TakeCoin()
		{
			_wonCoins++;

			if (OnNewCoinTaked != null)
			{
				OnNewCoinTaked(_wonCoins);
			}
		}

		#endregion
	}
}