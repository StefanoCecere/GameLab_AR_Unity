using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace demo.basket
{
    public class Screenshot : MonoBehaviour
    {
        [SerializeField] string fileName = "screenshot";
        [SerializeField] string saveDirectory;
        string saveFileName;


        private void Update()
        {
            if (Input.GetKeyDown("space")) {
                TakeScreenshot(saveDirectory);
            }
        }

        // Start is called before the first frame update
        void TakeScreenshot(string saveDirectory)
        {
            DirectoryInfo d = new DirectoryInfo(saveDirectory); //Assuming Test is your Folder

            FileInfo[] Files = d.GetFiles(); //Getting Text files
            int id = 1;
            if (File.Exists(Path.Combine(d.FullName, fileName + ".png"))) {
                string newFileName = "";
                foreach (FileInfo file in Files) {
                    newFileName = fileName + "_" + id.ToString() + ".png";
                    if (System.IO.File.Exists(Path.Combine(d.FullName, newFileName))) {
                        id++;
                    }
                }
                saveFileName = newFileName;
            } else {
                saveFileName = fileName + ".png";
            }

            string path = Path.Combine(d.FullName, saveFileName);
            Debug.Log("Saved: " + path);

            ScreenCapture.CaptureScreenshot(path);
        }
    }
}
