using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [Header("Enemy prefabs")]
    [SerializeField] private GameObject chaserPrefab;
    [SerializeField] private GameObject heavyPrefab;
    [SerializeField] private GameObject rangedPrefab;

    [Header("Arena")]
    [SerializeField] private float arenaHalfSize = 9f;        // floor is 20x20, stay 1 inside the edge
    [SerializeField] private float spawnHeight = 0.5f;
    [SerializeField] private float minDistanceFromPlayer = 6f; // bug W1

    [Header("Waves")]
    [SerializeField] private int firstWaveSize = 3;
    [SerializeField] private int extraPerWave = 2;
    [SerializeField] private int maxAlive = 25;               // bug W6
    [SerializeField] private float timeBetweenSpawns = 0.3f;
    [SerializeField] private float timeBetweenWaves = 2f;
    [SerializeField] private float waveTimeout = 20f;

    private readonly List<GameObject> alive = new List<GameObject>();
    private Transform player;

    public int CurrentWave { get; private set; }

    void Start()
    {
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null) player = p.transform;

        StartCoroutine(RunWaves());
    }

    // A coroutine runs step by step across frames, so one wave can't trigger ten times (bug W3)
    IEnumerator RunWaves()
    {
        while (true)
        {
            CurrentWave++;
            int count = firstWaveSize + (CurrentWave - 1) * extraPerWave;
            Debug.Log("Wave " + CurrentWave + ": " + count + " enemies");

            for (int i = 0; i < count; i++)
            {
                while (AliveCount() >= maxAlive) yield return null;   // wait if arena is full
                Spawn(PickEnemy());
                yield return new WaitForSeconds(timeBetweenSpawns);
            }

            // Wait until everyone's dead OR the timeout runs out
            float waveStart = Time.time;
            while (AliveCount() > 0 && Time.time - waveStart < waveTimeout)
            {
                yield return null;
            }

            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }

    GameObject PickEnemy()
    {
        float r = Random.value;   // random number 0 to 1
        if (CurrentWave >= 3 && r < 0.2f) return heavyPrefab;
        if (CurrentWave >= 2 && r < 0.5f) return rangedPrefab;
        return chaserPrefab;
    }

    void Spawn(GameObject prefab)
    {
        if (prefab == null) return;
        alive.Add(Instantiate(prefab, RandomEdgePoint(), Quaternion.identity));
    }

    Vector3 RandomEdgePoint()
    {
        Vector3 pos = Vector3.zero;
        for (int tries = 0; tries < 20; tries++)
        {
            float along = Random.Range(-arenaHalfSize, arenaHalfSize);
            switch (Random.Range(0, 4))   // pick one of the 4 edges
            {
                case 0: pos = new Vector3(along, spawnHeight,  arenaHalfSize); break;
                case 1: pos = new Vector3(along, spawnHeight, -arenaHalfSize); break;
                case 2: pos = new Vector3( arenaHalfSize, spawnHeight, along); break;
                default: pos = new Vector3(-arenaHalfSize, spawnHeight, along); break;
            }
            if (player == null || Vector3.Distance(pos, player.position) >= minDistanceFromPlayer) break;
        }
        return pos;
    }

    int AliveCount()
    {
        alive.RemoveAll(e => e == null);   // dead enemies drop out of the list (bug W4)
        return alive.Count;
    }
}