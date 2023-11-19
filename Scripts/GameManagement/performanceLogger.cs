using UnityEngine;
public class PerformanceLogger : MonoBehaviour
{
    private float deltaTime = 0.0f;

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        LogPerformance();
    }

    void LogPerformance()
    {
        float fps = 1.0f / deltaTime;
        int totalMemory = (int)(System.GC.GetTotalMemory(false) / 1024);
        int activeObjects = FindObjectsOfType<GameObject>().Length;

        string logMessage = $"FPS: {fps:0.}, Total Memory (KB): {totalMemory}, Active GameObjects: {activeObjects}";
        Debug.Log(logMessage);
    }
}
