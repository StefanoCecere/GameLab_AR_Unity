using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace demo.basket
{
    public class OverlapPlaneDetecor : MonoBehaviour
    {
        public bool triggered;
        public bool isOverlaped;
        public delegate void PlaneOverlap();
        public event PlaneOverlap PlaneOverlapEvent;

        private void Start()
        {
            triggered = false;
            isOverlaped = false;
        }

        private void OnTriggerEnter(Collider other)
        {

            ARPlane potetialOverlapedPlane = other.gameObject.GetComponent<ARPlane>();
            if (potetialOverlapedPlane) {
                triggered = true;
                Debug.Log("Overlap detected");
                isOverlaped = true;
                PlaneOverlapEvent?.Invoke();
                //potetialOverlapedPlane.gameObject.SetActive(false);
            }
        }
    }
}
