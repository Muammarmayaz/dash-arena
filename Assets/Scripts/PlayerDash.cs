using System.Collections.Generic;
using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private float dashCooldown = 2f;
    [SerializeField] private float iFrameExtra = 0.1f;

    [Header("Dash hits")]
    [SerializeField] private int dashDamage = 1;
    [SerializeField] private float hitRadius = 0.6f;

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
    private readonly HashSet<IDamageable> hitThisDash = new HashSet<IDamageable>();

    public bool IsDashing => Time.time < dashEndTime;
    public bool IsInvulnerable => Time.time < dashEndTime + iFrameExtra;

    void Awake()
    {
        mover = GetComponent<PlayerMover>();
        body = GetComponentInChildren<Renderer>();
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
            Vector3 from = transform.position;
            transform.position += dashDirection * dashSpeed * Time.deltaTime;
            HitAlongPath(from, transform.position);
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
        hitThisDash.Clear();                 // fresh dash, nobody hit yet (bug D2)
    }

    // Checks the whole stretch moved this frame, not just where we landed (bug D1)
    void HitAlongPath(Vector3 from, Vector3 to)
    {
        Vector3 lift = Vector3.up * 0.5f;    // enemy-body height, not floor level
        Collider[] hits = Physics.OverlapCapsule(from + lift, to + lift, hitRadius,
                                                 ~0, QueryTriggerInteraction.Collide);

        foreach (Collider hit in hits)
        {
            // Add() returns false if this enemy was already hit this dash
            if (hit.TryGetComponent(out IDamageable target) && hitThisDash.Add(target))
            {
                target.TakeDamage(dashDamage);
            }
        }
    }

    Vector3 DirectionToMouse()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Plane ground = new Plane(Vector3.up, transform.position);

        if (ground.Raycast(ray, out float distance))
        {
            Vector3 dir = ray.GetPoint(distance) - transform.position;
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.01f) return dir.normalized;
        }
        return transform.forward;
    }

    void UpdateColour()
    {
        if (IsDashing) body.material.color = dashColor;
        else if (Time.time < nextDashTime) body.material.color = cooldownColor;
        else body.material.color = readyColor;
    }
}