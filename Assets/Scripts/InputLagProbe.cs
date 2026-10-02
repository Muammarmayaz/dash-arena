using System.IO;
using UnityEngine;

// TEMPORARY diagnostic (delete after the WASD bug is fixed).
// Auto-creates itself on Play. Writes InputProbe.log next to the Assets folder.
public class InputLagProbe : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Boot()
    {
        if (FindAnyObjectByType<InputLagProbe>() != null) return;
        var go = new GameObject("InputLagProbe");
        DontDestroyOnLoad(go);
        go.AddComponent<InputLagProbe>();
    }

    string path;
    Transform player;
    PlayerMover mover;
    PlayerDash dash;
    PlayerHealth health;
    Vector3 lastPos;
    float lastHealth = -1f;

    void Awake()
    {
        path = Path.Combine(Application.dataPath, "..", "InputProbe.log");
        File.WriteAllText(path, "probe start\n");
    }

    void Log(string s) { File.AppendAllText(path, $"{Time.realtimeSinceStartup:F3} f{Time.frameCount} {s}\n"); }

    void Update()
    {
        if (player == null)
        {
            var p = GameObject.FindWithTag("Player");
            if (p == null) return;
            player = p.transform; mover = p.GetComponent<PlayerMover>();
            dash = p.GetComponent<PlayerDash>(); health = p.GetComponent<PlayerHealth>();
            lastPos = player.position; Log("player found"); return;
        }

        float dt = Time.unscaledDeltaTime;
        if (dt > 0.05f) Log($"HITCH {dt * 1000f:F0} ms");

        bool keyHeld = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);
        float h = Input.GetAxisRaw("Horizontal"), v = Input.GetAxisRaw("Vertical");
        float moved = (player.position - lastPos).magnitude;

        if (health != null && !Mathf.Approximately(health.Health01, lastHealth))
        { Log($"HP {health.Health01:F2} keyHeld={keyHeld} axis=({h},{v}) moverEnabled={mover.enabled}"); lastHealth = health.Health01; }

        if (keyHeld && h == 0f && v == 0f) Log("KEY HELD BUT AXIS = 0");
        if ((h != 0f || v != 0f) && !dash.IsDashing && moved < 0.0001f)
            Log($"AXIS SET BUT NOT MOVING moverEnabled={mover.enabled} timeScale={Time.timeScale}");
        if (!mover.enabled && !dash.IsDashing) Log("MOVER DISABLED OUTSIDE DASH");
        if (Input.GetMouseButtonDown(0)) Log($"CLICK (dash) hurtInvuln={health.IsHurtInvulnerable}");

        lastPos = player.position;
    }
}
