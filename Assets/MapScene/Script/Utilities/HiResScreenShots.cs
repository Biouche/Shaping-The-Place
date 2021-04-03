using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HiResScreenShots : MonoBehaviour
{
    int resWidth;
    int resHeight;
    public Camera targetCamera;
    public Text InfoMessage;

    private bool takeHiResShot = false;

    void Awake()
    {
        resWidth = Display.displays[targetCamera.targetDisplay].systemWidth;
        resHeight = Display.displays[targetCamera.targetDisplay].systemHeight;
    }

    public static string ScreenShotName(int width, int height)
    {
        return string.Format("{0}/Screenshots/screen_{1}x{2}_{3}.png",
                             Application.streamingAssetsPath,
                             width, height,
                             System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
    }

    public void TakeHiResShot()
    {
        takeHiResShot = true;
    }

    void LateUpdate()
    {
        if (takeHiResShot)
        {

            RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
            targetCamera.targetTexture = rt;
            Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
            targetCamera.Render();
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
            targetCamera.targetTexture = null;
            RenderTexture.active = null; // JC: added to avoid errors
            Destroy(rt);
            byte[] bytes = screenShot.EncodeToPNG();
            string filename = ScreenShotName(resWidth, resHeight);
            System.IO.File.WriteAllBytes(filename, bytes);
            Debug.Log(string.Format("Took screenshot to: {0}", filename));
            takeHiResShot = false;

            //Display message
            StartCoroutine(DisplayTempMessage());
        }

    }
    public IEnumerator DisplayTempMessage()
    {
        InfoMessage.text = "Screenshot has been saved";
        yield return new WaitForSeconds(4.5f);
        InfoMessage.text = "";
    }
}