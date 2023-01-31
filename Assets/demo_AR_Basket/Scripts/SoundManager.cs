using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace demo.basket
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] AudioClip[] bounceAudioClips;
        [SerializeField] AudioClip basketSound;
        AudioSource audioSource;
        GameManager gameManager;
        [SerializeField] AudioClip beepSound;

        // Start is called before the first frame update
        void Start()
        {
            Basket.BasketEvent += PlayBasketSound;
            audioSource = GetComponent<AudioSource>();
            Ball.BallHitEvent += PlayBounceSound;
            Timer.TimeAboutToEndEvent += StartBeeping;
            gameManager = FindObjectOfType<GameManager>();
        }

        void PlayBasketSound()
        {
            if (gameManager.currentGameState == GameManager.GameState.Gameplay)
                AudioSource.PlayClipAtPoint(basketSound, Camera.main.transform.position);
        }

        void PlayBounceSound(float speed, Vector3 pos, string tag)
        {

            audioSource.volume = speed / 2.5f;
            if (tag == "Hoop") {
                audioSource.pitch = 2;
            } else {
                audioSource.pitch = 1;
            }

            AudioClip bounceSound = bounceAudioClips[Random.Range(0, bounceAudioClips.Length)];
            audioSource.clip = bounceSound;
            audioSource.Play();
            //AudioSource.PlayClipAtPoint(bounceSound, pos);
        }

        void StartBeeping(float beepDuration)
        {
            StartCoroutine(StartBeepingAfter(0));
            StartCoroutine(StartBeepingAfter(1));
            StartCoroutine(StartBeepingAfter(2));
        }

        IEnumerator StartBeepingAfter(float waitDuration)
        {
            yield return new WaitForSeconds(waitDuration);
            AudioSource.PlayClipAtPoint(beepSound, Camera.main.transform.position);
        }

    }
}
