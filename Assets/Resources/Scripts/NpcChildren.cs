using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class NpcChildren : NpcMovements
{
    [SerializeField] private float minDistance = 3f;
    Player player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindFirstObjectByType<Player>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshSurface = FindFirstObjectByType<NavMeshSurface>();
        UpdateNavMesh();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(Vector3.Distance(transform.position, player.transform.position) <= minDistance)
        {
            MoveToTarget();
        }
    }
}
