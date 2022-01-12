# AR demo lessons

Unity version used: 2021.2.x

supported Android devices: https://developers.google.com/ar/discover/supported-devices
iOS: iPhone o iPad con iOS >= 12.0

---

creo Prefab

aggiungo package ARFoundation e ARcore

creo Prefab Plane ->
Empty
AR Plane
AR Plane visualizer
mesh filter
mesh rendererd
LInerendered (0,01) e no world space
creo materiale tranparent

poi script ARPlace

```
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace MyDemo
{
    public class ARPlaceOnTap : MonoBehaviour
    {
        public GameObject gameToInstatiate;

        private GameObject spawnedObject;
        private ARRaycastManager raycatManager;
        private Vector2 touchPosition;
        static List<ARRaycastHit> hits = new List<ARRaycastHit>();

        private void Awake()
        {
            raycatManager = GetComponent<ARRaycastManager>();
        }

        void Update()
        {
            if (Input.touchCount > 0) {
                touchPosition = Input.GetTouch(0).position;

                if (raycatManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon)) {

                    var hitPose = hits[0].pose;

                    if (spawnedObject == null) {
                        spawnedObject = Instantiate(gameToInstatiate, hitPose.position, hitPose.rotation);
                    } else {
                        spawnedObject.transform.position = hitPose.position;
                    }
                }
            }

        }
    }
}
```