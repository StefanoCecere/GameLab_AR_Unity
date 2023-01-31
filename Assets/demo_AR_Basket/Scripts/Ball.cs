using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace demo.basket
{
    public class Ball : MonoBehaviour
    {
        public delegate void BallHit(float speed, Vector3 pos, string tag);
        public static BallHit BallHitEvent;
        Rigidbody rb;
        public float distanceToBasket { get; set; }
        public bool isValid { get; set; }

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            distanceToBasket = 0;
        }
        // Start is called before the first frame update
        private void OnCollisionEnter(Collision collision)
        {
            BallHitEvent?.Invoke(rb.velocity.magnitude, transform.position, collision.gameObject.tag);
        }
    }
}
