using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShooter : MonoBehaviour
{
    public Texture2D CaptureScreenshot()
    {
        int width = Screen.width;
        int height = Screen.height;
        
        RenderTexture rt = RenderTexture.GetTemporary(width, height, 24);
        Camera mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogError("Main camera not found");
            return null;
        }
        mainCamera.targetTexture = rt;
        RenderTexture.active = rt;
        mainCamera.Render();

        Texture2D screenShot = new Texture2D(width, height, TextureFormat.RGB24, false);
        screenShot.ReadPixels(new Rect(0, 0, width, height),0,0);
        screenShot.Apply();

        mainCamera.targetTexture = null;
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt);

        Texture2D resizedScreenShot = ResizeTexture(screenShot, width/6, height/6);
        Destroy(screenShot);
        
        return resizedScreenShot;
    }

    private Texture2D ResizeTexture(Texture2D originalTexture, int width, int height)
    {
        RenderTexture rt = RenderTexture.GetTemporary(width, height, 24);
        RenderTexture.active = rt;

        Graphics.Blit(originalTexture, rt);

        Texture2D resizedTexture = new Texture2D(width, height, TextureFormat.RGB24, false);
        resizedTexture.ReadPixels(new Rect(0, 0, width, height),0,0);
        resizedTexture.Apply();

        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt);

        return resizedTexture;
    }
}
