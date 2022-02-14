using UnityEngine;

namespace SeaberyTest.TouchControls
{
    /// <summary>
    /// Class in charge of manage the behavior of scale the scene.
	/// The scale action will be triggered pinching the screen with two fingers.
    /// </summary>
    public class TouchControlScale : MonoBehaviour
	{
        #region INSPECTOR VARIABLES
        
        [SerializeField]
        [Tooltip("Multiplier that will be applied to the scalation. It will determine the sensitivity of the pinch in/out")]
        private float _scaleIndex = 0.05f;

        [SerializeField]
        [Tooltip("Min scale allowed for the ARSessionOrigin's transform. The smaller this value is, the more zoom in will be available")]
        private float _minScaleAllowed = 10f;
        
        [SerializeField]
        [Tooltip("Max scale allowed for the ARSessionOrigin's transform. The bigger this value is, the more zoom out will be available")]
        private float _maxScaleAllowed = 80f;

        #endregion

        #region PRIVATE VARIABLES

        private Touch _touch0;
        private Touch _touch1;
        private Vector2 _touch0PrevPos;
        private Vector2 _touch1PrevPos;
        private float _prevTouchDeltaMag;
        private float _touchDeltaMag;
        private float _deltaMagnitudeDiff;
        private float _newScale;

        #endregion

        #region UNITY EVENTS
        
        private void Update()
        {
            ScaleARScene();
        }

        #endregion

        #region PRIVATE METHODS

        private void ScaleARScene()
        {
            if (Input.touchCount == 2)
            {
                // Gets the information of the two touches
                _touch0 = Input.GetTouch(0);
                _touch1 = Input.GetTouch(1);

                // Calculates the position in the PREVIOUS FRAME of each touch and gets the distance between them
                _touch0PrevPos = _touch0.position - _touch0.deltaPosition;
                _touch1PrevPos = _touch1.position - _touch1.deltaPosition;
                _prevTouchDeltaMag = (_touch0PrevPos - _touch1PrevPos).magnitude;

                // Calculates the distance between the touches in the CURRENT FRAME
                _touchDeltaMag = (_touch0.position - _touch1.position).magnitude;

                // Gets the difference in the distances between PREVIOUS and CURRENT frame
                _deltaMagnitudeDiff = _prevTouchDeltaMag - _touchDeltaMag;

                // Calculates the scale value applying the specified multiplier index
                _newScale = Mathf.Clamp(
                    transform.localScale.x + _deltaMagnitudeDiff * _scaleIndex,
                    _minScaleAllowed,
                    _maxScaleAllowed);

                // Applies the scale to the ARSessionOrigin transform
                transform.localScale = Vector3.one * _newScale;
            }
        } 

        #endregion
    }
}
