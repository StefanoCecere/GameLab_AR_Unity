using SeaberyTest.World;
using UnityEngine;

namespace SeaberyTest.TouchControls
{
	/// <summary>
	/// Class in charge of manage the behavior of rotate the scene.
	/// The rotation action will be triggered dragging two fingers along the screen.
	/// </summary>
	public class TouchControlRotate : MonoBehaviour
	{
		#region INSPECTOR VARIABLES
		
		[SerializeField]
		[Tooltip("Speed that will be applied to the rotation. It will determine the sensitivity of the dragging")]
		private float _rotationSpeed = 5f;

		#endregion

		#region PRIVATE VARIABLES
		
		private bool _readyForRotate = false;
		private Touch _touch0;
		private Touch _touch1;
		private Vector2 _touchVector;
		private float _angle;
		private Transform _worldReference;

		#endregion

		#region UNITY EVENTS

		private void Awake()
		{
			// Events subscriptions
			WorldController.OnWorldCreated += OnWorldCreated;
		}

		private void Update()
		{
			// It will work once the world has been instantiated in the AR scene
			if (_readyForRotate)
			{
				RotateARScene();
			}
		}

		private void OnDestroy()
		{
			// Events unsubscriptions
			WorldController.OnWorldCreated -= OnWorldCreated;
		}

		#endregion

		#region PRIVATE METHODS

		/// <summary>
		/// Once the world has been instantiated in the AR scene, it gets its reference and the rotation system will be ready from this moment on.
		/// </summary>
		/// <param name="worldReference">Reference to the instantiated world</param>
		private void OnWorldCreated(Transform worldReference)
		{
			_worldReference = worldReference;
			_readyForRotate = true;
		}

		/// <summary>
		/// Rotates the ARSessionOrigin game object using two fingers in the screen
		/// </summary>
		private void RotateARScene()
		{
			if (Input.touchCount == 2)
			{
				// Gets the information of the two touches and calculates the sum of their delta position vectors
				_touch0 = Input.GetTouch(0);
				_touch1 = Input.GetTouch(1);
				_touchVector = _touch0.deltaPosition + _touch1.deltaPosition;

				if (_touch0.phase == TouchPhase.Moved && _touch1.phase == TouchPhase.Moved)
				{
					// Calculates the rotation angle, based on the X axis of the vector previously calculated
					_angle = _rotationSpeed * _touchVector.x * Time.deltaTime;

					// Applies the rotation of the ARSessionOrigin around the world transform
					transform.RotateAround(
						_worldReference.position,
						Vector3.up,
						_angle);
				}
			}
		}

		#endregion
	}
}