using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private float dashCooldown = 2f;
    [SerializeField] private float iFrameExtra = 0.1f;

    [Header("Feedback colours")]
    [SerializeField] private Color readyColor = Color.white;
    [SerializeField] private Color dashColor = Color.cyan;
    [SerializeField] private Color cooldownColor = new Color(0.3f, 0.3f, 0.3f);

    private PlayerMover mover;
    private Renderer body;
    private Camera cam;
    private Vector3 dashDirection;
    private float dashEndTime;
    private float nextDashTime;

    public bool IsDashing => Time.time < dashEndTime;
    public bool IsInvulnerable => Time.time < dashEndTime + iFrameExtra;

    void Awake()
    {
        mover = GetComponent<PlayerMover>();
        body = GetComponentInChildren<Renderer>();   // the Capsule child
        cam = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && Time.time >= nextDashTime)
        {
            StartDash();
        }

        if (IsDashing)
        {
            transform.position += dashDirection * dashSpeed * Time.deltaTime;
        }
        else if (!mover.enabled)
        {
            mover.enabled = true;
        }

        UpdateColour();
    }

    void StartDash()
    {
        dashDirection = DirectionToMouse();
        dashEndTime = Time.time + dashDuration;
        nextDashTime = Time.time + dashCooldown;
        mover.enabled = false;
    }

    Vector3 DirectionToMouse()
    {
        // Fire a ray from the camera through the mouse, see where it hits the player's height
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Plane ground = new Plane(Vector3.up, transform.position);

        if (ground.Raycast(ray, out float distance))
        {
            Vector3 dir = ray.GetPoint(distance) - transform.position;
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.01f) return dir.normalized;
        }
        return transform.forward;   // mouse right on top of you (bug D4)
    }

    void UpdateColour()
    {
        if (IsDashing) body.material.color = dashColor;
        else if (Time.time < nextDashTime) body.material.color = cooldownColor;
        else body.material.color = readyColor;
    }
}