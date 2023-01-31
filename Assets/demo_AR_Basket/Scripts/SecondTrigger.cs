using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace demo.basket
{
    public class SecondTrigger : MonoBehaviour
    {
        public bool triggered;
        public float distaceToBasket;
        public bool isValidShoot;
        private void Start()
        {
            triggered = false;
        }
        private void OnTriggerEnter(Collider other)
        {
            Ball ball = other.GetComponent<Ball>();
            if (ball) {
                triggered = true;
                distaceToBasket = ball.distanceToBasket;
                isValidShoot = ball.isValid;
            }
        }
    }
}
