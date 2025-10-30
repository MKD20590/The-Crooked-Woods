using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class NpcMonster : NpcMovements
{
    [SerializeField] private MonsterSpawner spawner;
    [SerializeField] private float minDistanceCaught = 0.2f;
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
            if (Vector3.Distance(transform.position, player.transform.position) <= minDistance && 
                Vector3.Distance(transform.position, player.transform.position) > minDistanceCaught && 
                !player.isHiding)
            {
                MoveToTarget();
            }
            else if(Vector3.Distance(transform.position, player.transform.position) <= minDistanceCaught && 
                !player.isHiding)
            {
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
        else
        {
            spawner.monsterSpawned = false;
            gameObject.SetActive(false);
        }
    }
    public void Spawned()
    {
        isSpawned = true;
        duration = Random.Range(minDuration, maxDuration);
    }
}
