using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public enum DeathCause
{
    Fissure,
    MovingObstacle,
    Laser,
    Other
}

public class AnalyticsUploader : MonoBehaviour
{
    public static AnalyticsUploader Instance { get; private set; }

    [Header("Apps Script Web App URL")]
    public string endpoint =
        "https://script.google.com/macros/s/AKfycbw9M_oug82hPArKXtQEsRy-_8_MtHyMKrYEYPW0fpbpMgmxpzgnRnqPGNPKd1t_VXqM/exec";

    [Header("Must match API_TOKEN in Apps Script")]
    public string token = "888";

    private int runId = 0;

    void Awake()
    {
        // 单例模式（和你原来一样）
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        StartNewRun(); // 启动即为 Run 1
    }

    public void StartNewRun()
    {
        runId++;
        Debug.Log("Starting new run: " + runId);
    }

    public void LogDeath(DeathCause cause)
    {
        string timestamp = DateTime.UtcNow.ToString("o");

        var payload = new DeathEvent
        {
            token = token,
            run_id = runId,
            timestamp = timestamp,
            death_reason = cause.ToString()
        };

        StartCoroutine(PostEvent(payload));

        Debug.Log($"[Analytics] Logged death: {cause} (Run {runId})");
    }

    IEnumerator PostEvent(DeathEvent ev)
    {
        string json = JsonUtility.ToJson(ev);

        using var req = new UnityWebRequest(endpoint, "POST");
        req.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();

        Debug.Log($"Analytics Upload Result: {req.result}, Response: {req.downloadHandler.text}");

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogWarning("Upload failed: " + req.error);
        }
    }

    [Serializable]
    class DeathEvent
    {
        public string token;
        public int run_id;
        public string timestamp;
        public string death_reason;
    }
}