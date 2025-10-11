using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class NpcMonster : NpcMovements
{
    public MonsterSpawner spawner;
    [SerializeField] private float minDistance = 15f;
    [SerializeField] private float minDuration = 15f;
    [SerializeField] private float maxDuration = 30f;
    [SerializeField] private float duration = 0f;
    Player player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindFirstObjectByType<Player>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshSurface = FindFirstObjectByType<NavMeshSurface>();
        UpdateNavMesh();
        duration = Random.Range(minDuration, maxDuration);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(Vector3.Distance(transform.position, player.transform.position) <= minDistance)
        {
            MoveToTarget();
        }
        else if(duration > 0)
        {
            duration -= Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
