using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace MyDemo
{
    public class SpawnableManager : MonoBehaviour
    {
        public GameObject PrefabToInstatiate;
        public ARRaycastManager m_RaycastManager;

        private GameObject spawnedObject;
        private Touch touch;
        static List<ARRaycastHit> hits = new List<ARRaycastHit>();

        private void Start()
        {
            spawnedObject = null;
        }

        void Update()
        {
            if (Input.touchCount == 0) {
                return;
            }

            touch = Input.GetTouch(0);

            //if (m_RaycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon)) {

            //    var hitPose = hits[0].pose;

            //    if (spawnedObject == null) {
            //        spawnedObject = Instantiate(PrefabToInstatiate, hitPose.position, hitPose.rotation);
            //    } else {
            //        spawnedObject.transform.position = hitPose.position;
            //    }
            //}

            if (m_RaycastManager.Raycast(touch.position, hits)) {
                var hitPose = hits[0].pose;

                if (touch.phase == TouchPhase.Began) {
                    spawnedObject = Instantiate(PrefabToInstatiate, hitPose.position, hitPose.rotation);

                } else if (touch.phase == TouchPhase.Moved && spawnedObject != null) {
                    spawnedObject.transform.position = hitPose.position;
                }
                if (touch.phase == TouchPhase.Ended) {
                    spawnedObject = null;
                }
            }
        }
    }
}