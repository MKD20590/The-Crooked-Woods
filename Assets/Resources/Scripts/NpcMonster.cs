using System.Collections;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class NpcMonster : NpcMovements
{
    [SerializeField] private AudioSource monsterVoice;
    [SerializeField] private CanvasGroup staticScreen;
    [SerializeField] private MonsterSpawner spawner;
    [SerializeField] private float minDistance = 30f;
    [SerializeField] private float minDuration = 10f;
    [SerializeField] private float maxDuration = 15f;
    [SerializeField] private float duration = 0f;
    public bool isSpawned = false;
    Player player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindFirstObjectByType<Player>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshSurface = FindFirstObjectByType<NavMeshSurface>();
        //UpdateNavMesh();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isSpawned)
        {
            // more opaque = nearer
            staticScreen.alpha = 1 - Mathf.InverseLerp(1, 20, Vector3.Distance(transform.position, player.transform.position));

            // if player is not hiding & is in range = chase the player
            if (Vector3.Distance(transform.position, player.transform.position) <= minDistance && 
                Vector3.Distance(transform.position, player.transform.position) > minDistanceStopping && 
                !player.isHiding)
            {
                MoveToTarget();
            }
            else if(Vector3.Distance(transform.position, player.transform.position) <= minDistanceStopping && 
                !player.isHiding)
            {
                player.GetCaught();
                isSpawned = false;
            }
            if(duration > 0 && isSpawned)
            {
                duration -= Time.deltaTime;
            }
            else
            {
                isSpawned = false;
            }
        }
        else
        {
            StopAllCoroutines();
            staticScreen.alpha = 0;
            spawner.monsterSpawned = false;
            gameObject.SetActive(false);
        }
    }
    IEnumerator PlayVoice()
    {
        while (isSpawned)
        {
            monsterVoice.Play();
            yield return new WaitForSeconds(Random.Range(5f, 10f));
        }
    }
    public void Spawned()
    {
        StartCoroutine(PlayVoice());
        isSpawned = true;
        duration = Random.Range(minDuration, maxDuration);
    }
}
