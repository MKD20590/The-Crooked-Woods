using System.Collections;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] private GameObject monsterPrefab;
    [SerializeField] private GameObject currentMonster;
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
        if (player.GetHunger() < minHunger && currentMonster == null)
        {
            StartCoroutine(Spawn());
        }
    }

    public IEnumerator Spawn()
    {
        yield return new WaitForSeconds(Random.Range(minSpawnInterval,maxSpawnInterval));
        int rand = Random.Range(0, 2);
        if(rand == 0)
        {
            Vector3 posisi = new Vector3(Random.Range(20f, 30f), Random.Range(0f, 2f), Random.Range(-20f, 20f));
            currentMonster = Instantiate(monsterPrefab, player.transform.position + posisi, Quaternion.identity);
        }
        else
        {
            Vector3 posisi = new Vector3(Random.Range(20f, 30f), Random.Range(-0f, -2f), Random.Range(-20f, 20f));
            currentMonster = Instantiate(monsterPrefab, player.transform.position - posisi, Quaternion.identity);
        }
    }
}
