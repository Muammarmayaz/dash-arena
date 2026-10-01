using UnityEngine;
using UnityEngine.UI;

// Builds a simple health bar in the top-left corner at runtime.
// Put it on the Player next to PlayerHealth. No scene wiring needed.
[RequireComponent(typeof(PlayerHealth))]
public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Vector2 barSize = new Vector2(320f, 28f);
    [SerializeField] private Color backColor = new Color(0f, 0f, 0f, 0.6f);
    [SerializeField] private Color fullColor = new Color(0.35f, 0.85f, 0.45f);
    [SerializeField] private Color lowColor = new Color(0.9f, 0.25f, 0.2f);
    [SerializeField] private Color hurtFlashColor = Color.white;

    private PlayerHealth health;
    private Image fill;

    void Start()
    {
        health = GetComponent<PlayerHealth>();

        GameObject canvasObj = new GameObject("HUD", typeof(Canvas), typeof(CanvasScaler));
        Canvas canvas = canvasObj.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasObj.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);

        Image back = CreateImage("HealthBack", canvasObj.transform, backColor);
        RectTransform b = back.rectTransform;
        b.anchorMin = b.anchorMax = b.pivot = new Vector2(0f, 1f);   // top-left corner
        b.anchoredPosition = new Vector2(30f, -30f);
        b.sizeDelta = barSize;

        fill = CreateImage("HealthFill", back.transform, fullColor);
        RectTransform f = fill.rectTransform;
        f.anchorMin = Vector2.zero;
        f.anchorMax = Vector2.one;
        f.offsetMin = new Vector2(3f, 3f);
        f.offsetMax = new Vector2(-3f, -3f);
    }

    void Update()
    {
        if (fill == null) return;

        float pct = health.Health01;
        fill.rectTransform.anchorMax = new Vector2(pct, 1f);   // shrink the bar from the right

        if (health.IsHurtInvulnerable && Mathf.Repeat(Time.time * 10f, 1f) > 0.5f)
            fill.color = hurtFlashColor;                        // blink while invulnerable
        else
            fill.color = Color.Lerp(lowColor, fullColor, pct);
    }

    static Image CreateImage(string name, Transform parent, Color color)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        Image img = go.GetComponent<Image>();
        img.color = color;
        img.raycastTarget = false;
        return img;
    }
}
