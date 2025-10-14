using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class NpcMonster : NpcMovements
{
    public MonsterSpawner spawner;
    [SerializeField] private float minDistanceCaught = 0.7f;
    [SerializeField] private float minDistance = 30f;
    [SerializeField] private float minDuration = 10f;
    [SerializeField] private float maxDuration = 15f;
    [SerializeField] private float duration = 0f;
    GameManager gm;
    public bool isSpawned = false;
    Player player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gm = FindFirstObjectByType<GameManager>();
        player = FindFirstObjectByType<Player>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshSurface = FindFirstObjectByType<NavMeshSurface>();
        //UpdateNavMesh();
        duration = Random.Range(minDuration, maxDuration);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isSpawned)
        {
            if (Vector3.Distance(transform.position, player.transform.position) <= minDistance)
            {
                MoveToTarget();
            }
            else if(Vector3.Distance(transform.position, player.transform.position) <= minDistanceCaught)
            {
                gm.MonsterEats();
                player.GetCaught();
                isSpawned = false;
            }
            if(duration > 0)
            {
                duration -= Time.deltaTime;
            }
            else
            {
                isSpawned = false;
            }
        }
    }
}
