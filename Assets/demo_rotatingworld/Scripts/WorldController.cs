using SeaberyTest.GameManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace SeaberyTest.World
{
	/// <summary>
	/// Class in charge of instantiate the world in the AR scene.
	/// It will use 'ARPlaneManager' and 'ARRaycastManager' in order to find a position in the AR scene and instantiate the world prefab through a touch.
	/// </summary>
	[RequireComponent(typeof(ARSessionOrigin))]
	[RequireComponent(typeof(ARPlaneManager))]
	[RequireComponent(typeof(ARRaycastManager))]
	public class WorldController : MonoBehaviour
	{
		#region INSPECTOR VARIABLES
		
		[SerializeField]
		[Tooltip("World prefab that will be instantiated in the AR scene")]
		private GameObject _worldPrefab;

		#endregion

		#region PRIVATE VARIABLES
		
		private ARSessionOrigin _arSessionOrigin;
		private ARPlaneManager _arPlaneManager;
		private ARRaycastManager _arRaycastManager;
		private Vector2 _touchPosition;
		private List<ARRaycastHit> _hits = new List<ARRaycastHit>();
		private GameObject _worldReference;
		private Vector3 _originalScale;
		private Quaternion _originalRotation;

		#endregion

		#region PUBLIC EVENTS
		
		public delegate void WorldCreated(Transform worldReference);

		/// <summary>
		/// Executed when the world game object has been instantiated in the AR scene.
		/// </summary>
		public static event WorldCreated OnWorldCreated;

		#endregion

		#region UNITY EVENTS

		private void Awake()
		{
			_arSessionOrigin = GetComponent<ARSessionOrigin>();
			_arPlaneManager = GetComponent<ARPlaneManager>();
			_arRaycastManager = GetComponent<ARRaycastManager>();

			// Events subscriptions
			GameController.OnGameInitialized += OnGameInitialized;

			// Store original values of the ARSessionOrigin transform and deactivate the AR planes detection
			_originalScale = transform.localScale;
			_originalRotation = transform.rotation;
			_arPlaneManager.enabled = false;
		}

		private void Update()
		{
#if !UNITY_EDITOR
			// It does nothing while the ARPlaneManager is deactivated or the user does not touch in a valid position
			if (!_arPlaneManager.enabled || !GetTouchPosition(out _touchPosition))
				return;

			// Calculates the position touched by the user and instantiate the world game object on it
			// Just after that, the ARPlaneManager is deactivated and the gameplay starts
			if (_arRaycastManager.Raycast(_touchPosition, _hits, TrackableType.PlaneWithinPolygon))
			{
				InstantiateWorld(_hits[0].pose.position, Quaternion.identity);
				StartCoroutine(ActivatePlanesDetection(false));
				GameController.Instance.StartGameplay();
			}
#else
			// THIS CODE IS ONLY TO FACILITATE THE TESTING DURING THE EXECUTION ON EDITOR MODE
			// IT DIRECTLY SKIP THE PLANES DETECTION AND INSTANTIATES THE WORLD IN A FIXED POSITION IN THE SCENE
			if (!_arPlaneManager.enabled)
				return;

			InstantiateWorld(new Vector3(0f, -5f, 20f), Quaternion.identity);
			StartCoroutine(ActivatePlanesDetection(false));
			GameController.Instance.StartGameplay();
#endif
		}

		private void OnDestroy()
		{
			// Events unsubcriptions
			GameController.OnGameInitialized -= OnGameInitialized;
		}

		#endregion

		#region PRIVATE METHODS

		/// <summary>
		/// Once the game is initialized for the first time or restarted, the world game obejct is destroyed and the ARPlaneManager is activated.
		/// </summary>
		private void OnGameInitialized()
		{
			if (_worldReference != null)
			{
				Destroy(_worldReference.gameObject);
			}
			StartCoroutine(ActivatePlanesDetection(true));
		}

		/// <summary>
		/// Gets the positions on the screen touched by the user.
		/// </summary>
		/// <param name="touchPosition">2D position on the screen</param>
		/// <returns>True if there has been a valid touch in the screen</returns>
		private bool GetTouchPosition(out Vector2 touchPosition)
		{
			if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
			{
				touchPosition = Input.GetTouch(0).position;
				return true;
			}

			touchPosition = default;
			return false;
		}

		/// <summary>
		/// Instantiates the world game object in the AR scene, with a specific position and rotation.
		/// It will notify about it through the 'OnWorldCreated' event to the rest of the classes that have subscribed.
		/// </summary>
		/// <param name="position">Position in the scene</param>
		/// <param name="rotation">Rotation applied to the world game object</param>
		private void InstantiateWorld(Vector3 position, Quaternion rotation)
		{
			_worldReference = Instantiate(_worldPrefab, position, rotation);

			// Restore the original scale and rotation of the ARSessionOrigin
			transform.localScale = _originalScale;
			transform.rotation = _originalRotation;

			// Configures the ARSessionOrigin camera based on the instantiated world game object
			_arSessionOrigin.MakeContentAppearAt(
				_worldReference.transform,
				_worldReference.transform.position,
				_worldReference.transform.rotation);

			if (OnWorldCreated != null)
			{
				OnWorldCreated(_worldReference.transform);
			}
		}

		/// <summary>
		/// Activates/deactivates the ARPlaneManager for AR planes detection.
		/// It has been defined as a coroutine because it has to wait for a little delay in order to avoid some possible problems touching the screen just when the AR planes detection is activated again.
		/// </summary>
		/// <param name="isActive">True for activate it</param>
		/// <returns></returns>
		private IEnumerator ActivatePlanesDetection(bool isActive)
		{
			if (!isActive)
			{
				ActivateARPlaneManager(false);
				yield break;
			}
			else
			{
				yield return new WaitForSeconds(0.5f);
				ActivateARPlaneManager(true);
			}
		}

		/// <summary>
		/// Activate/deactivate the ARPlaneManager and show/hide the detected planes (trackables)
		/// </summary>
		/// <param name="isActive">True for activate it</param>
		private void ActivateARPlaneManager(bool isActive)
		{
			_arPlaneManager.enabled = isActive;

			foreach (ARPlane plane in _arPlaneManager.trackables)
			{
				plane.gameObject.SetActive(isActive);
			}
		} 

		#endregion
	}
}