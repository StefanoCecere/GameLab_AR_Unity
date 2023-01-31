using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace demo.basket
{
    public class ObjectThrower : MonoBehaviour
    {
        [SerializeField] GameObject objectToThrow;

        Camera cam;
        Vector2 inputStartPos;
        [SerializeField] float forceFactor;
        Timer timer;
        bool firstBall;
        GameObject thrownObject;
        Basket basket;
        Vector3 objectStartPos;
        public bool isFixStartPos = true;
        public float ballWaitDuration = 1f;
        bool throwStarted;
        bool throwEnabled;
        public int numShots { get; private set; }
        // Start is called before the first frame update
        void Start()
        {
            cam = FindObjectOfType<Camera>();
            timer = FindObjectOfType<Timer>();

            InitilizeParams();
            if (isFixStartPos) {
                StartSpawnCoroutine(0);
            }
        }

        private void InitilizeParams()
        {
            firstBall = true;
            throwStarted = false;
            throwEnabled = true;
            numShots = 0;
        }



        // Update is called once per frame
        void Update()
        {
            // If there is no tap, then simply do nothing until the next call to Update().
            //if (Input.touchCount == 0)
            //    return;

            //var touch = Input.GetTouch(0);
            //if (touch.phase != TouchPhase.Began)
            //    return;

            //Vector2 inputPos = touch.position;
            UpdateBallPosition();

            if (Input.GetMouseButtonDown(0) && throwEnabled) {
                //Dont allow shoot before initilization
                throwStarted = true;
                inputStartPos = Input.mousePosition;
                throwEnabled = false;
                if (!isFixStartPos) {
                    Ray startRay = cam.ScreenPointToRay(inputStartPos);
                    Vector3 objectStartPos = startRay.origin + startRay.direction.normalized * 0.7f;
                    thrownObject = Instantiate(objectToThrow, objectStartPos, Quaternion.identity);
                    thrownObject.GetComponent<Rigidbody>().isKinematic = true;
                }
            }

            if (Input.GetMouseButtonUp(0) && throwStarted) {
                Vector2 inputEndPos = Input.mousePosition;

                Ray startRay = cam.ScreenPointToRay(inputStartPos);
                Ray endRay = cam.ScreenPointToRay(inputEndPos);
                Vector3 forceVectorOnScreen = endRay.origin - startRay.origin;
                float forceMagnitude = forceVectorOnScreen.magnitude * forceFactor;

                Vector3 forceVector3D = endRay.direction.normalized + forceVectorOnScreen.normalized;
                Vector3 forceVectorDirection = forceVector3D.normalized;

                Rigidbody rb = thrownObject.GetComponent<Rigidbody>();
                rb.isKinematic = false;
                rb.velocity = forceVectorDirection * forceMagnitude;

                throwStarted = false;
                numShots += 1;

                //Calculate distace to basket for score calculation
                basket = FindObjectOfType<Basket>();
                float distanceToBasket = Vector3.Distance(cam.transform.position, basket.transform.position);
                Ball ball = thrownObject.GetComponent<Ball>();
                ball.distanceToBasket = distanceToBasket;
                ball.isValid = !timer.isFinished;


                if (isFixStartPos) {
                    StartSpawnCoroutine(ballWaitDuration);
                }


                if (firstBall) {
                    timer.StartTimer();
                    firstBall = false;
                }
            }
        }

        private void StartSpawnCoroutine(float waitDuration)
        {
            StartCoroutine(CountForNewBallSpawn(waitDuration));
        }
        IEnumerator CountForNewBallSpawn(float waitDuration)
        {
            yield return new WaitForSeconds(waitDuration);
            thrownObject = Instantiate(objectToThrow, objectStartPos, Quaternion.identity);
            thrownObject.GetComponent<Rigidbody>().isKinematic = true;
            throwEnabled = true;
        }

        private void UpdateBallPosition()
        {
            if (thrownObject) {
                if (thrownObject.GetComponent<Rigidbody>().isKinematic) {
                    Ray startRay = cam.ScreenPointToRay(new Vector3(cam.pixelWidth / 2, cam.pixelHeight / 6, 0));
                    objectStartPos = startRay.origin + startRay.direction.normalized * 0.7f;
                    thrownObject.transform.position = objectStartPos;
                }
            }
        }
        public void ResetThrower()
        {
            if (thrownObject && !isFixStartPos) {
                thrownObject.GetComponent<Rigidbody>().isKinematic = false;
            }
            InitilizeParams();
        }
    }
}
