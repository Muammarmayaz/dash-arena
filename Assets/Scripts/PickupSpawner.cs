using UnityEngine;

// Drops one health pickup at a random spot every so often.
// Put it on the WaveSpawner object. Drag your imported model into "Pickup Model";
// if it's empty, a red sphere is used instead.
public class PickupSpawner : MonoBehaviour
{
    [SerializeField] private GameObject pickupModel;
    [SerializeField] private float modelScale = 1f;
    [SerializeField] private float minDelay = 12f;
    [SerializeField] private float maxDelay = 20f;
    [SerializeField] private float areaHalfSize = 7f;          // stay away from the walls
    [SerializeField] private float spawnHeight = 0.8f;
    [SerializeField] private float minDistanceFromPlayer = 4f; // make the player go for it

    private GameObject current;
    private float nextSpawnTime;
    private Transform player;

    void Start()
    {
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null) player = p.transform;
        nextSpawnTime = Time.time + Random.Range(minDelay, maxDelay);
    }

    void Update()
    {
        // Only one at a time. The timer restarts once the current one is gone.
        if (current != null)
        {
            nextSpawnTime = Time.time + Random.Range(minDelay, maxDelay);
            return;
        }
        if (Time.time >= nextSpawnTime) Spawn();
    }

    void Spawn()
    {
        Vector3 pos = Vector3.zero;
        for (int tries = 0; tries < 20; tries++)
        {
            pos = new Vector3(Random.Range(-areaHalfSize, areaHalfSize), spawnHeight,
                              Random.Range(-areaHalfSize, areaHalfSize));
            if (player == null || Vector3.Distance(pos, player.position) >= minDistanceFromPlayer) break;
        }

        GameObject pickup = new GameObject("HealthPickup");
        pickup.transform.position = pos;

        GameObject visual;
        if (pickupModel != null)
        {
            visual = Instantiate(pickupModel, pickup.transform);
        }
        else
        {
            visual = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Destroy(visual.GetComponent<Collider>());
            visual.transform.SetParent(pickup.transform, false);
            visual.transform.localScale = Vector3.one * 0.6f;
            visual.GetComponent<Renderer>().material.color = new Color(0.85f, 0.15f, 0.25f);
        }
        visual.transform.localPosition = Vector3.zero;
        visual.transform.localScale *= modelScale;

        pickup.AddComponent<HealthPickup>();   // added last, so it finds the visual's renderers
        current = pickup;
    }
}
