using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace demo.basket
{
    public class PlaneObjectSpawner : MonoBehaviour
    {
        [SerializeField] GameObject objectToSpawn;
        ARPlaneManager planeManager;

        List<TrackableId> verticalPlaneIds;
        TrackableId largestSpawnPlaneId;
        float largestSpawnPlaneArea;
        bool readyForNewSpawn;
        public bool isEnvironmentReady { get; private set; }
        public bool enableSpawning;
        GameObject spawnedObject;

        //public delegate void ScanComplete();
        //public event ScanComplete ScanCompleteEvent;


        // Start is called before the first frame update
        void Start()
        {
            enableSpawning = false;
            readyForNewSpawn = true;
            isEnvironmentReady = false;
            verticalPlaneIds = new List<TrackableId>();
            planeManager = FindObjectOfType<ARPlaneManager>();
            planeManager.planesChanged += UpdatePlanes;

            Basket.BasketEvent += SetReadyForNewSpawn;
        }

        // Update is called once per frame
        void Update()
        {

            if (verticalPlaneIds.Count > 0 && readyForNewSpawn && enableSpawning) {
                SpawnNewBasket();
            }

        }

        void SpawnNewBasket()
        {
            //Find largest plane to spawn basket
            largestSpawnPlaneArea = -1;
            foreach (TrackableId validPlaneId in verticalPlaneIds) {
                ARPlane plane = planeManager.GetPlane(validPlaneId);
                float planeArea = plane.size.x * plane.extents.y;
                if (largestSpawnPlaneArea < planeArea) {
                    largestSpawnPlaneId = validPlaneId;
                    largestSpawnPlaneArea = planeArea;
                }
            }
            foreach (TrackableId validPlaneId in verticalPlaneIds) {
                ARPlane plane = planeManager.GetPlane(validPlaneId);
                bool isActive = largestSpawnPlaneId == validPlaneId;
                plane.gameObject.SetActive(isActive);
            }

            ARPlane planeToSpawnObjectOn = planeManager.GetPlane(largestSpawnPlaneId);
            Vector3 spawnPosition = GetRandomPointOnPlane(planeToSpawnObjectOn);
            spawnedObject = Instantiate(objectToSpawn, spawnPosition, Quaternion.LookRotation(planeToSpawnObjectOn.normal, Vector3.up));
            //spawnedObject.GetComponentInChildren<Basket>().BasketEvent += SetReadyForNewSpawn;
            spawnedObject.GetComponentInChildren<OverlapPlaneDetecor>().PlaneOverlapEvent += RespawnBasket;
            readyForNewSpawn = false;
        }

        void DestroyBasket()
        {
            //spawnedObject.GetComponentInChildren<Basket>().BasketEvent -= SetReadyForNewSpawn;
            spawnedObject.GetComponentInChildren<OverlapPlaneDetecor>().PlaneOverlapEvent -= RespawnBasket;
            Destroy(spawnedObject);
        }

        void RespawnBasket()
        {
            DestroyBasket();
            SpawnNewBasket();
        }

        Vector3 GetRandomPointOnPlane(ARPlane planeToSpawnObjectOn)
        {
            Vector3 randomVector = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            randomVector = randomVector.normalized;
            float minPlaneDim = Mathf.Min(planeToSpawnObjectOn.extents.x, planeToSpawnObjectOn.extents.y) * 0.8f;
            randomVector = randomVector * Random.Range(minPlaneDim / 2f, minPlaneDim);
            Vector3 spawnPosition = planeToSpawnObjectOn.center + Vector3.Cross(planeToSpawnObjectOn.normal, randomVector);
            if (spawnPosition.y < Camera.main.transform.position.y - 0.2f)
                spawnPosition.y = Camera.main.transform.position.y - 0.2f;


            return spawnPosition;
        }

        void SetReadyForNewSpawn()
        {
            StartCoroutine(DestroyCoroutine(0.3f));
        }

        IEnumerator DestroyCoroutine(float waitDuration)
        {
            yield return new WaitForSeconds(waitDuration);
            DestroyBasket();
            readyForNewSpawn = true;
        }


        void UpdatePlanes(ARPlanesChangedEventArgs eventArgs)
        {
            var addedPlanes = eventArgs.added;

            foreach (ARPlane plane in addedPlanes) {
                if (plane.alignment == PlaneAlignment.Vertical) {
                    //Debug.Log("Added " + plane.trackableId);
                    plane.boundaryChanged += OnBoundaryChange;
                }
            }

            var removedPlanes = eventArgs.removed;
            foreach (ARPlane plane in removedPlanes) {
                if (plane.alignment == PlaneAlignment.Vertical) {
                    //Debug.Log("Removed " + plane.trackableId);
                    plane.boundaryChanged -= OnBoundaryChange;
                    verticalPlaneIds.Remove(plane.trackableId);
                }
            }
        }

        void OnBoundaryChange(ARPlaneBoundaryChangedEventArgs eventArgs)
        {
            ARPlane changedPlane = eventArgs.plane;
            if (!verticalPlaneIds.Contains(changedPlane.trackableId)) {
                if (changedPlane.alignment == PlaneAlignment.Vertical) {
                    if (changedPlane.size.x >= 0.3 && changedPlane.extents.y >= 0.3) {
                        //Debug.Log("Big enough " + changedPlane.trackableId);
                        //Debug.Log(changedPlane.extents);
                        verticalPlaneIds.Add(changedPlane.trackableId);
                        isEnvironmentReady = true;
                    }
                }
            }
        }

        private void OnDestroy()
        {
            planeManager.planesChanged -= UpdatePlanes;
        }
    }
}
