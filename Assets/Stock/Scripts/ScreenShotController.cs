using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ScreenShotController : MonoBehaviour
{
    [SerializeField] private string savingFolderName = "Output";
    [SerializeField] private int resWidth = 1920;
    [SerializeField] private int resHeight = 1080;

    [SerializeField] Button buttonTakePhoto;
    [SerializeField] private Camera screenShotCamera;
   
    private int screenShotSessionFileIndex;
   
    private void Start()
    {
        AddListeners();
    }

    private void AddListeners()
    {
        buttonTakePhoto.onClick.AddListener(() => { TakeScreenShot(); });
    }

    private void TakeScreenShot()
    {
        RenderTexture renderTexture = new RenderTexture(resWidth, resHeight, 24);
        screenShotCamera.targetTexture = renderTexture;
        Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
        screenShotCamera.Render();
        RenderTexture.active = renderTexture;
        screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
        screenShotCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(renderTexture);
        byte[] bytes = screenShot.EncodeToPNG();
        string directoryPath = GetFilePath(savingFolderName);
       
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(GetFilePath(savingFolderName));
        }

        string screenShotName = GetScreenShotName();
        string filePath = Path.Combine(directoryPath, screenShotName);
        File.WriteAllBytes(filePath, bytes);
        screenShotSessionFileIndex++;
    }

    private string GetScreenShotName()
    {
        return $"ScreenShot ({screenShotSessionFileIndex}){System.DateTime.Now.ToString("HH-mm-ss")}.png";
    }

    private string GetFilePath(string folderName)
    {
        return Path.Combine(Application.persistentDataPath, folderName, System.DateTime.Now.ToString("yyyy-MM-dd"));
    }
}