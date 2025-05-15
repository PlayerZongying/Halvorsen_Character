using UnityEngine;
using System.IO;
using Unity.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class HighResolutionScreenshot : MonoBehaviour
{
    public Camera targetCamera; // 要截图的相机
    public int screenshotWidth = 3840; // 4K 的宽度
    public int screenshotHeight = 2160; // 4K 的高度
    public float pauseTime = 30f;
    [SerializeField, ReadOnly]
    private float elapsedTime = 0f;
    private bool triggered = false;
    public string baseFileName = "HighResScreenshot"; // 基础文件名
    public string saveDirectory = "Screenshots"; // 保存截图的文件夹
    
#if UNITY_EDITOR
    void OnGUI()
    {
        Event e = Event.current;
        if (EditorApplication.isPaused && e.type == EventType.KeyDown && e.keyCode == KeyCode.P)
        {
            Debug.Log("Paused Screenshot Triggered");
            CaptureHighResolutionScreenshot();
        }
    }
#endif
    

    void Update()
    {
        elapsedTime += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.P)) // 按下 P 键截屏
        {
            CaptureHighResolutionScreenshot();
        }
#if UNITY_EDITOR
        if (!triggered && Time.time >= pauseTime)
        {
            EditorApplication.isPaused = true; // 自动点击“暂停”按钮
            Debug.Log("Editor paused at " + Time.time + "s");
            triggered = true;
        }
#endif
        
    }

    void CaptureHighResolutionScreenshot()
    {
        // 确保保存目录存在
        string directoryPath = Path.Combine(Application.dataPath, saveDirectory);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        // 生成唯一的文件名
        string fileName = GenerateUniqueFileName(directoryPath, baseFileName, "png");

        // 创建一个高分辨率 RenderTexture
        RenderTexture renderTexture = new RenderTexture(screenshotWidth, screenshotHeight, 24);
        targetCamera.targetTexture = renderTexture;

        // 渲染相机画面到 RenderTexture
        Texture2D screenshot = new Texture2D(screenshotWidth, screenshotHeight, TextureFormat.RGB24, false);
        targetCamera.Render();

        // 将 RenderTexture 的内容读入到 Texture2D
        RenderTexture.active = renderTexture;
        screenshot.ReadPixels(new Rect(0, 0, screenshotWidth, screenshotHeight), 0, 0);
        screenshot.Apply();

        // 保存截图为 PNG 文件
        byte[] bytes = screenshot.EncodeToPNG();
        string filePath = Path.Combine(directoryPath, fileName);
        File.WriteAllBytes(filePath, bytes);

        Debug.Log($"High resolution screenshot saved to: {filePath}");

        // 清理
        targetCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(renderTexture);
    }

    string GenerateUniqueFileName(string directory, string baseName, string extension)
    {
        int index = 0;
        string fileName;

        do
        {
            fileName = $"{baseName}_{index}.{extension}";
            index++;
        } while (File.Exists(Path.Combine(directory, fileName)));

        return fileName;
    }
}