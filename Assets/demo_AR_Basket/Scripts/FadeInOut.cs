using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace demo.basket
{
    public class FadeInOut : MonoBehaviour
    {
        [SerializeField] CanvasGroup canvasElement;
        [SerializeField] float animationDuration;
        //[SerializeField] bool loop;
        [SerializeField] float start = 1;
        [SerializeField] float end = 0;
        public bool fadeIn = true;

        // Start is called before the first frame update
        void Start()
        {
            canvasElement.alpha = start;
        }

        // Update is called once per frame
        void Update()
        {
            float speed = Time.deltaTime * Mathf.Abs(start - end) / animationDuration;

            if (fadeIn) {
                if (canvasElement.alpha > end) {
                    canvasElement.alpha -= speed;
                } else {
                    fadeIn = false;
                }
            } else {
                if (canvasElement.alpha < start) {
                    canvasElement.alpha += speed;
                } else {
                    fadeIn = true;
                }
            }

        }
    }
}
