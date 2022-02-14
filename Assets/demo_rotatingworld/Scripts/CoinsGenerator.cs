using SeaberyTest.GameManagement;
using SeaberyTest.Player;
using SeaberyTest.World;
using System.Collections;
using UnityEngine;

namespace SeaberyTest.Coins
{
	/// <summary>
	/// Class in charge of managing the generation of coins along the gameplay.
	/// It activates/deactivates a single coin object instead of instantiate/destroy a new one each time.
	/// </summary>
	[RequireComponent(typeof(AudioSource))]
	public class CoinsGenerator : MonoBehaviour
	{
		#region INSPECTOR VARIABLES
		
		[SerializeField]
		[Tooltip("Coin prefab that will be instantiated in the scene")]
		private GameObject _coinPrefab;

		[SerializeField]
		[Tooltip("Max radius where the coins will be generated")]
		private float _generationRadius = 7f;

		[Tooltip("Min distance from player where the coins will be generated")]
		[SerializeField]
		private float _minDistanceFromPlayer = 5f;

		#endregion

		#region PRIVATE VARIABLES
		
		private AudioSource _coinAudioSource;
		private Transform _worldReference;
		private Transform _playerReference;
		private bool _isWorldInstantiated = false;
		private bool _isPlayerInstantiated = false;
		private GameObject _currentCoinGenerated;
		private ParticleSystem _coinParticleSystem;

		#endregion

		#region UNITY EVENTS

		private void Awake()
		{
			_coinAudioSource = GetComponent<AudioSource>();

			// Events subscriptions
			GameController.OnGameInitialized += OnGameInitialized;
			WorldController.OnWorldCreated += OnWorldCreated;
			PlayerController.OnPlayerCreated += OnPlayerCreated;
			GameController.OnNewCoinTaked += OnNewCoinTaked;
		}

		private void OnDestroy()
		{
			// Events unsubscriptions
			GameController.OnGameInitialized -= OnGameInitialized;
			WorldController.OnWorldCreated -= OnWorldCreated;
			PlayerController.OnPlayerCreated -= OnPlayerCreated;
			GameController.OnNewCoinTaked -= OnNewCoinTaked;
		}

		#endregion

		#region PRIVATE METHODS

		/// <summary>
		/// Once the game is initialized for the first time or restarted, deactivates the generated coin (if it exists).
		/// </summary>
		private void OnGameInitialized()
		{
			_isWorldInstantiated = false;
			_isPlayerInstantiated = false;

			if (_currentCoinGenerated != null)
			{
				_currentCoinGenerated.SetActive(false);
			}
		}

		/// <summary>
		/// Once the world has been instantiated in the AR scene, if the player has been instantiated too, it starts to generate coins.
		/// </summary>
		/// <param name="worldReference">Reference to the instantiated world</param>
		private void OnWorldCreated(Transform worldReference)
		{
			_worldReference = worldReference;
			_isWorldInstantiated = true;
			if (_isPlayerInstantiated)
			{
				StartCoroutine(GenerateRandomCoin());
			}
		}

		/// <summary>
		/// Once the player has been instantiated in the AR scene, if the world has been instantiated too, it starts to generate coins.
		/// </summary>
		/// <param name="playerReference">Reference to the instantiated player</param>
		private void OnPlayerCreated(Transform playerReference)
		{
			_playerReference = playerReference;
			_isPlayerInstantiated = true;
			if (_isWorldInstantiated)
			{
				StartCoroutine(GenerateRandomCoin());
			}
		}

		/// <summary>
		/// Each time the player pick up a coin, it generates a new coin in the scene.
		/// </summary>
		/// <param name="currentCoins">Number of coins currently taken</param>
		private void OnNewCoinTaked(int currentCoins)
		{
			_coinAudioSource.Play();

			if (_currentCoinGenerated != null)
			{
				_currentCoinGenerated.SetActive(false);
			}

			// A new coin will be generated only if the player has still not picked up all the needed coins for win
			if (GameController.Instance.WonCoins < GameController.Instance.TotalCoinsToWin)
			{
				StartCoroutine(GenerateRandomCoin());
			}
		}

		/// <summary>
		/// Generates a new coin in a random position along the scene.
		/// This coroutine waits until find a valid position where put the coin (within the specified radius and far ecough of the player).
		/// </summary>
		private IEnumerator GenerateRandomCoin()
		{
			Vector2 random2DPosition = Vector2.zero;
			Vector3 randomPositionOnTheWorld = Vector3.zero;

			do
			{
				// Get a random position inside a 2D circle with the specified radius
				random2DPosition = Random.insideUnitCircle * _generationRadius;

				// Calculates the previous 2D random position into the 3D plane defined by the surface of the world and elevates it one unit
				randomPositionOnTheWorld = new Vector3(
					random2DPosition.x,
					1f,
					random2DPosition.y) + _worldReference.position;

				yield return null;
			} while ((_playerReference.position - randomPositionOnTheWorld).magnitude <= _minDistanceFromPlayer);

			if (_currentCoinGenerated == null)
			{
				// If it is the first coin generated, it is instantiated in the scene with the calculated random position and its reference is stored to be used the next times
				_currentCoinGenerated = Instantiate(_coinPrefab, randomPositionOnTheWorld, Quaternion.identity, transform);
				_coinParticleSystem = _currentCoinGenerated.GetComponentInChildren<ParticleSystem>();
			}
			else
			{
				// If the coin was already instantiated, it is simply moved to the calculated random position
				_currentCoinGenerated.transform.position = randomPositionOnTheWorld;
			}

			// Activates the coin game object
			_coinParticleSystem.Play();
			_currentCoinGenerated.SetActive(true);
		} 

		#endregion
	}
}