using UnityEngine;
using UnityEngine.SceneManagement;

namespace KRUR.ARdemo
{
    public class DemoManager : MonoBehaviour
    {
        private static DemoManager _instance;

        public static DemoManager I
        {
            get { return _instance; }
        }

        void Awake()
        {
            if (_instance != null && _instance != this) {
                Destroy(this.gameObject);
                return;
            }

            _instance = this;

            DontDestroyOnLoad(this.gameObject);
        }

        public void ChangeScene(int sceneNumber)
        {
            //sceneNumber -= 1;
            SceneManager.LoadScene(sceneNumber, LoadSceneMode.Single);
            //Debug.Log("carico " + sceneNumber);
        }

    }
}