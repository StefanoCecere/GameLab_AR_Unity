using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace demo.basket
{
    public class FirstTrigger : MonoBehaviour
    {
        public bool triggered;

        private void Start()
        {
            triggered = false;
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Ball") {
                triggered = true;
            }
        }
    }
}
