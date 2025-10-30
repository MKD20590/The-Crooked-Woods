using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] private AudioSource monsterSpawn;
    [SerializeField] private List<Transform> spawnPositions;
    [SerializeField] private NpcMonster currentMonster;
    [SerializeField] private float minHunger = 20f;
    [SerializeField] private float minSpawnInterval = 5f;
    [SerializeField] private float maxSpawnInterval = 20f;
    Player player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindFirstObjectByType<Player>();
    }
    private void FixedUpdate()
    {
        if (player.GetHunger() < minHunger && !currentMonster.isSpawned)
        {
            currentMonster.isSpawned = true;
            StartCoroutine(Spawn());
        }
    }

    public IEnumerator Spawn()
    {
        yield return new WaitForSeconds(Random.Range(minSpawnInterval,maxSpawnInterval));
        monsterSpawn.Play();
        int randPos = Random.Range(0, spawnPositions.Count);
        currentMonster.transform.position = spawnPositions[randPos].position;
        currentMonster.gameObject.SetActive(true);
    }
}
