using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace demo.basket
{
    public class ObjectDestroyer : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Ball") {
                Destroy(other.gameObject);
            }

        }
    }
}
