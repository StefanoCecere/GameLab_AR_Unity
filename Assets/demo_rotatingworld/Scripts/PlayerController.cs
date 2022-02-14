using SeaberyTest.GameManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SeaberyTest.Player
{
	/// <summary>
	/// Class in charge of move the player along the scene and manage his interactions
	/// </summary>
	public class PlayerController : MonoBehaviour
	{
		#region INSPECTOR VARIABLES

		[SerializeField]
		[Tooltip("Speed of the player")]
		private float _movementSpeed = 4f;

		[SerializeField]
		[Tooltip("Pointer prefab")]
		private GameObject _movementPointer;

		[SerializeField]
		[Tooltip("Particle System related to player movement")]
		private ParticleSystem _runParticles;

		#endregion

		#region PRIVATE VARIABLES

		private GameObject _pointer;
		private Touch _touch;
		private Ray _touchRay;
		private RaycastHit _rayHit;
		private Vector3 _positionToGo;
		private Coroutine _currentCoroutine;
		private Animator _playerAnimator;

		#endregion

		#region CONSTANTS

		private const int BLOCK_TOUCHES_LAYER = 8; 

		#endregion

		#region PUBLIC EVENTS

		public delegate void PlayerCreated(Transform playerReference);

		/// <summary>
		/// Executed when the player game object has been instantiated in the AR scene.
		/// </summary>
		public static event PlayerCreated OnPlayerCreated;

		#endregion

		#region UNITY EVENTS

		private void Awake()
		{
			_playerAnimator = GetComponent<Animator>();

			// Events subscriptions
			GameController.OnGameInitialized += OnGameInitialized;

			// Instantiates the pointer and deactivates it
			_pointer = Instantiate(_movementPointer);
			_pointer.SetActive(false);
		}

		private void Start()
		{
			// Notify about player instantiation to the rest of the classes that have subscribed
			if (OnPlayerCreated != null)
			{
				OnPlayerCreated(this.transform);
			}
		}

		void Update()
		{
#if !UNITY_EDITOR
			// It does nothing while the user does not touch in a valid position in the world for move the player
			if (!GetPositionToGo(out _positionToGo))
				return;
#else
			// THIS CODE IS ONLY TO FACILITATE THE TESTING DURING THE EXECUTION ON EDITOR MODE
			// IT GETS THE POSITION TO GO USING THE MOUSE BUTTON INSTEAD OF SCREEN TOUCH
			if (!GetPositionToGoByMouse(out _positionToGo))
				return;
#endif

			if (_currentCoroutine != null)
				StopCoroutine(_currentCoroutine);

			// Move the player to the specified position in the world and plays the run animation
			_currentCoroutine = StartCoroutine(Move(transform.position, _positionToGo, _movementSpeed));
		}

		private void OnTriggerEnter(Collider other)
		{
			// When the player go through a coin, notify to the GameController in order to increment his coins counter
			if (other.tag == "Coin")
			{
				GameController.Instance.TakeCoin();
			}
		}

		private void OnDestroy()
		{
			// Events unsubscriptions
			GameController.OnGameInitialized -= OnGameInitialized;
		}

		#endregion

		#region PRIVATE METHODS

		/// <summary>
		/// Once the game is initialized for the first time or restarted, the pointer game obejct is destroyed.
		/// </summary>
		private void OnGameInitialized()
		{
			Destroy(_pointer);
		}

		/// <summary>
		/// Reads the touch taps in the screen and get the corresponding 3D position on the surface of the instantiated world.
		/// </summary>
		/// <param name="positionToGo"></param>
		/// <returns>True if the position is valid</returns>
		private bool GetPositionToGo(out Vector3 positionToGo)
		{
			if (Input.touchCount == 1)
			{
				_touch = Input.GetTouch(0);
				if (!IsTouchingOverUIButton(_touch))
				{
					if (_touch.phase == TouchPhase.Ended)
					{
						// Throws a raycast from the screen touch to the scene to get the position to go
						_touchRay = Camera.main.ScreenPointToRay(_touch.position);
						if (Physics.Raycast(_touchRay, out _rayHit))
						{
							positionToGo = new Vector3(_rayHit.point.x, transform.position.y, _rayHit.point.z);
							return true;
						}
					}
				}
			}

			positionToGo = default;
			return false;
		}

		/// <summary>
		/// Reads the mouse clicks in the screen and get the corresponding 3D position on the surface of the instantiated world.
		/// THIS CODE IS ONLY TO FACILITATE THE TESTING DURING THE EXECUTION ON EDITOR MODE.
		/// </summary>
		/// <param name="positionToGo"></param>
		/// <returns></returns>
		private bool GetPositionToGoByMouse(out Vector3 positionToGo)
		{
			if (Input.GetMouseButtonUp(0))
			{
				// Throws a raycast from the mouse click to the scene to get the position to go
				_touchRay = Camera.main.ScreenPointToRay(Input.mousePosition);
				if (Physics.Raycast(_touchRay, out _rayHit))
				{
					positionToGo = new Vector3(_rayHit.point.x, transform.position.y, _rayHit.point.z);
					return true;
				}
			}

			positionToGo = default;
			return false;
		}

		/// <summary>
		/// Check if the user is touching over any UI element (layer = BLOCK_TOUCHES_LAYER).
		/// </summary>
		/// <param name="touch">Touch to check</param>
		/// <returns>True if the touch is over a UI element</returns>
		private bool IsTouchingOverUIButton(Touch touch)
		{
			PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
			eventDataCurrentPosition.position = new Vector2(touch.position.x, touch.position.y);
			List<RaycastResult> results = new List<RaycastResult>();
			EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

			return results.Any(r => r.gameObject.layer == BLOCK_TOUCHES_LAYER);
		}

		/// <summary>
		/// Move the player from 'fromPos' position to 'toPos' with a velocity of 'speed'.
		/// </summary>
		/// <param name="fromPos">Initial position</param>
		/// <param name="toPos">Final position</param>
		/// <param name="speed">Movement speed</param>
		/// <returns></returns>
		private IEnumerator Move(Vector3 fromPos, Vector3 toPos, float speed)
		{
			// Plays the running animation, move the pointer to the final position and starts to reproduce the particle effects
			PlayAnimation(true);
			ShowPointer(true, toPos);
			_runParticles.Play();

			// Calculates the player direction to the final position
			Vector3 vectorToTarget = toPos - fromPos;
			transform.forward = vectorToTarget.normalized;

			// Updates the player position along each frame, basing on the specified speed and the longitude of the path
			float playerStep = (speed / vectorToTarget.magnitude) * Time.deltaTime;
			float currentStep = 0;
			while (currentStep <= 1.0f)
			{
				currentStep += playerStep;
				transform.position = Vector3.Lerp(fromPos, toPos, currentStep);
				yield return new WaitForEndOfFrame();
			}
			transform.position = toPos;

			// Stops the running animation, hide the pointer and particle effects
			PlayAnimation(false);
			ShowPointer(false);
			_runParticles.Stop();
		}

		/// <summary>
		/// Play/Stop the running animation.
		/// </summary>
		/// <param name="isRunning">True for play the animation</param>
		private void PlayAnimation(bool isRunning)
		{
			if (_playerAnimator != null)
			{
				_playerAnimator.SetBool("IsRunning", isRunning);
			}
		}

		/// <summary>
		/// Show/Hide the player pointer.
		/// </summary>
		/// <param name="isVisible">True for show the pointer</param>
		/// <param name="pos">Position where the pointer will be positioned</param>
		private void ShowPointer(bool isVisible, Vector3 pos = default)
		{
			_pointer.transform.position = pos + Vector3.up * 0.05f;
			_pointer.SetActive(isVisible);
		} 

		#endregion
	}
}