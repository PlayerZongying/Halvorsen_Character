#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;

public class ScreenshotWindow : EditorWindow
{
    public int width = 3840;
    public int height = 2160;
    private Camera targetCamera;

    [MenuItem("Tools/Screenshot Window")]
    public static void ShowWindow()
    {
        GetWindow<ScreenshotWindow>("Screenshot Tool");
    }

    void OnGUI()
    {
        GUILayout.Label("Screenshot Settings", EditorStyles.boldLabel);

        width = EditorGUILayout.IntField("Width", width);
        height = EditorGUILayout.IntField("Height", height);

        targetCamera = (Camera)EditorGUILayout.ObjectField("Target Camera", targetCamera, typeof(Camera), true);

        if (targetCamera == null)
        {
            EditorGUILayout.HelpBox("请指定一个摄像机，或确保场景中有主摄像机 (MainCamera)。", MessageType.Info);
        }

        if (GUILayout.Button("📸 截图 (支持暂停中使用)"))
        {
            TakeScreenshot();
        }
    }

    void TakeScreenshot()
    {
        Camera cam = targetCamera ?? Camera.main;
        if (cam == null)
        {
            Debug.LogError("找不到摄像机！");
            return;
        }

        RenderTexture rt = new RenderTexture(width, height, 24);
        cam.targetTexture = rt;

        Texture2D screenshot = new Texture2D(width, height, TextureFormat.RGB24, false);
        cam.Render();

        RenderTexture.active = rt;
        screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        screenshot.Apply();

        string folderPath = Path.Combine(Application.dataPath, "Screenshots");
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        string filename = $"screenshot_{System.DateTime.Now:yyyyMMdd_HHmmss}.png";
        string fullPath = Path.Combine(folderPath, filename);

        File.WriteAllBytes(fullPath, screenshot.EncodeToPNG());

        Debug.Log($"✅ 截图保存成功：{fullPath}");

        // 清理
        cam.targetTexture = null;
        RenderTexture.active = null;
        DestroyImmediate(rt);
    }
}
#endif