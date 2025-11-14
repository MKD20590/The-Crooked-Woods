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
    public bool monsterSpawned = false;
    GameManager gm;
    Player player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindFirstObjectByType<Player>();
        gm = FindFirstObjectByType<GameManager>();
    }
    private void FixedUpdate()
    {
        if (player.GetHunger() < minHunger && !monsterSpawned && !gm.isMonsterEating)
        {
            monsterSpawned = true;
            StartCoroutine(Spawn());
        }
    }

    public IEnumerator Spawn()
    {
        yield return new WaitForSeconds(Random.Range(minSpawnInterval,maxSpawnInterval));
        monsterSpawn.Play();
        int randPos = Random.Range(0, spawnPositions.Count);
        currentMonster.transform.position = spawnPositions[randPos].position;
        currentMonster.Spawned();
        currentMonster.gameObject.SetActive(true);
    }
}
