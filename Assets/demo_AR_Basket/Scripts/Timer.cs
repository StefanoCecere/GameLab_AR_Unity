using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace demo.basket
{
    public class Timer : MonoBehaviour
    {
        [SerializeField] GameObject timerUI;
        TextMeshProUGUI timerTmp;
        bool isStarted;
        public bool isFinished { get; private set; }
        float startTime;
        public float totalTime;

        public delegate void TimeEnd();
        public event TimeEnd TimerFinishedEvent;

        public delegate void TimeIsShort(float beepDuration);
        public static event TimeIsShort TimeAboutToEndEvent;
        [SerializeField] float startBeepAt;
        bool beepingStarted;



        // Start is called before the first frame update
        void Start()
        {
            timerTmp = timerUI.GetComponent<TextMeshProUGUI>();
            timerTmp.SetText(totalTime.ToString("N1").Replace(",", "."));
            startTime = -1f;
            isStarted = false;
            isFinished = false;
            beepingStarted = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space)) {
                StartTimer();
            }

            if (isStarted && startTime < 0) {
                startTime = Time.time;
            }

            if (isStarted && !isFinished) {
                float elapsedTime = Time.time - startTime;
                float remainingTime = totalTime - elapsedTime;

                if (remainingTime <= startBeepAt && !beepingStarted) {
                    beepingStarted = true;
                    TimeAboutToEndEvent?.Invoke(startBeepAt);
                }

                if (remainingTime <= 0) {
                    remainingTime = 0;
                    isFinished = true;
                    TimerFinishedEvent?.Invoke();
                }

                string timeStr = remainingTime.ToString("N1").Replace(",", ".");
                timerTmp.SetText(timeStr);
            }
        }

        public void StartTimer()
        {
            isStarted = true;
        }

        public void ResetTimer()
        {
            timerTmp.SetText(totalTime.ToString("N1").Replace(",", "."));
            startTime = -1f;
            isStarted = false;
            isFinished = false;
            beepingStarted = false;
        }
    }
}
