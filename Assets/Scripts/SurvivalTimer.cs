using UnityEngine;
using UnityEngine.UI;

// Score = how long you survive. Shows time, best time and wave in the top-right.
// Builds its own UI at runtime. Put it on the WaveSpawner object.
public class SurvivalTimer : MonoBehaviour
{
    // static on purpose: it survives the scene reload, so "best" carries over between runs
    private static float bestTime;

    private Text label;
    private WaveSpawner spawner;

    void Start()
    {
        spawner = GetComponent<WaveSpawner>();
        if (spawner == null) spawner = FindAnyObjectByType<WaveSpawner>();

        GameObject canvasObj = new GameObject("TimerHUD", typeof(Canvas), typeof(CanvasScaler));
        Canvas canvas = canvasObj.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasObj.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);

        GameObject textObj = new GameObject("TimerText", typeof(RectTransform), typeof(Text), typeof(Outline));
        textObj.transform.SetParent(canvasObj.transform, false);
        RectTransform rt = textObj.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(1f, 1f);   // top-right corner
        rt.anchoredPosition = new Vector2(-30f, -20f);
        rt.sizeDelta = new Vector2(700f, 140f);

        label = textObj.GetComponent<Text>();
        label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        label.fontSize = 48;
        label.fontStyle = FontStyle.Bold;
        label.alignment = TextAnchor.UpperRight;
        label.color = Color.white;
        label.raycastTarget = false;
        textObj.GetComponent<Outline>().effectColor = new Color(0f, 0f, 0f, 0.8f);
    }

    void Update()
    {
        if (label == null) return;

        // timeSinceLevelLoad restarts at 0 after the reload; Time.time would not (bug S1)
        float t = Time.timeSinceLevelLoad;
        if (t > bestTime) bestTime = t;

        int wave = spawner != null ? spawner.CurrentWave : 0;
        label.text = t.ToString("0.0") + "s\n<size=28>BEST " + bestTime.ToString("0.0") + "s   WAVE " + wave + "</size>";
    }
}
