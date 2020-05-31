using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using TMPro;

namespace KRUR.ARdemo
{
    public class DemoScenesMenu : MonoBehaviour
    {
        public GameObject MenuItemPrefab;

        void Start()
        {
            clearAllChild();
            buildMenu();
        }

        public void ChangeScene(int sceneNumber)
        {
            DemoManager.I.ChangeScene(sceneNumber);
        }

        private void buildMenu()
        {
            GameObject item = null;
            var totScenes = SceneManager.sceneCountInBuildSettings;
            for (int i = 0; i < totScenes; i++) {
                int sceneIndex = new int();
                sceneIndex = i;
                //Debug.Log("scena: " + SceneUtility.GetScenePathByBuildIndex(i));
                item = Instantiate(MenuItemPrefab, transform);
                item.transform.Find("Label").GetComponent<TextMeshProUGUI>().text = getSceneName(SceneUtility.GetScenePathByBuildIndex(i));
                item.GetComponent<Button>().onClick.AddListener(delegate () { ChangeScene(sceneIndex); });
            }
        }

        private string getSceneName(string ScenePath)
        {
            var namearray = Path.GetFileName(ScenePath).Split('.');
            return namearray[0];
        }

        private void clearAllChild()
        {
            foreach (Transform child in transform) {
                Destroy(child.gameObject);
            }
        }

    }
}