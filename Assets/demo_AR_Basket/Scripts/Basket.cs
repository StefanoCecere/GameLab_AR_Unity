using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace demo.basket
{
    public class Basket : MonoBehaviour
    {

        public delegate void HitScore();
        public static event HitScore BasketEvent;

        [SerializeField] FirstTrigger firstTrigger;
        [SerializeField] SecondTrigger secondTrigger;
        ScoreManager scoreManager;

        private void Awake()
        {
            scoreManager = FindObjectOfType<ScoreManager>();
        }


        private void Update()
        {
            if (!firstTrigger.triggered && secondTrigger.triggered)
                secondTrigger.triggered = false;

            if (firstTrigger.triggered && secondTrigger.triggered) {
                if (secondTrigger.isValidShoot) {
                    scoreManager.UpdateScore(secondTrigger.distaceToBasket);
                    BasketEvent?.Invoke();
                }

                secondTrigger.triggered = false;
                firstTrigger.triggered = false;
            }
        }

    }
}
