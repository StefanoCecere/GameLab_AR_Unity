using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace demo.basket
{
    public class ScoreManager : MonoBehaviour
    {
        [SerializeField] GameObject pointDisplayUI;
        int basketCount;
        public int score { get; private set; }
        public int bestScore { get; private set; }
        float accuracy;

        [SerializeField] GameObject scoreUI;
        Canvas canvas;
        int screenWidth;
        int screenHeight;
        TextMeshProUGUI scoreTmp;

        // Start is called before the first frame update
        void Start()
        {
            score = 0;
            basketCount = 0;
            accuracy = 0;
            scoreTmp = scoreUI.GetComponent<TextMeshProUGUI>();
            scoreTmp.SetText(score.ToString());
            canvas = FindObjectOfType<Canvas>();
            screenWidth = Camera.main.pixelWidth;
            screenHeight = Camera.main.pixelHeight;

        }

        public void UpdateScore(float distaceToBasket)
        {
            basketCount += 1;
            int newPoint = (int)(distaceToBasket * distaceToBasket * 10);
            score += newPoint;
            DisplayPoint(newPoint);
            PlayVFX();
            scoreTmp.SetText(score.ToString());
        }

        public float CalculateAccuracy()
        {
            ObjectThrower objectThrower = FindObjectOfType<ObjectThrower>();

            if (objectThrower.numShots > 0)
                accuracy = basketCount * 1f / objectThrower.numShots;
            else
                accuracy = 0f;

            accuracy *= 100;

            return accuracy;
        }

        public void ResetScore()
        {
            accuracy = 0;
            basketCount = 0;
            score = 0;
            scoreTmp.SetText(score.ToString());
        }

        public void SetBestScore()
        {
            int prevBestScore = PlayerPrefs.GetInt("BestScore", -1);
            if (prevBestScore < score) {
                PlayerPrefs.SetInt("BestScore", score);
                bestScore = score;
            } else {
                bestScore = prevBestScore;
            }

        }

        void PlayVFX()
        {
            ParticleSystem[] particles = FindObjectOfType<Basket>().GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem p in particles) {
                p.Play();
            }
        }

        void DisplayPoint(int point)
        {
            StartCoroutine(PointDisplayCoroutine(point));
        }

        IEnumerator PointDisplayCoroutine(int point)
        {
            GameObject pointUI = Instantiate(pointDisplayUI);
            pointUI.transform.SetParent(canvas.transform);
            TextMeshProUGUI pointTmp = pointUI.GetComponent<TextMeshProUGUI>();
            pointTmp.SetText(point.ToString());
            float xPos = screenWidth / 2 + Random.Range(-screenWidth / 4, screenWidth / 4);
            float yPos = screenHeight / 2 + Random.Range(-screenHeight / 8, screenHeight / 10);
            pointUI.transform.position = new Vector3(xPos, yPos, 0);
            pointUI.transform.eulerAngles = new Vector3(0, 0, Random.Range(-30, 30));
            yield return new WaitForSeconds(0.3f);

            Color inintialColor = pointTmp.color;
            Color targetColor = new Color(inintialColor.r, inintialColor.g, inintialColor.b, 0);
            float transitionDuration = 0.33f;
            float elapsedTime = 0;
            while (elapsedTime <= transitionDuration) {
                elapsedTime += 0.01f;
                float t = elapsedTime / transitionDuration;
                pointTmp.color = Color.Lerp(inintialColor, targetColor, t);//new Color(inintialColor.r, inintialColor.g, inintialColor.b, 1 - t);
                yield return new WaitForSeconds(0.01f);

            }
            Destroy(pointUI);

        }

    }
}
