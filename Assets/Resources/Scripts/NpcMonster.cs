using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class NpcMonster : NpcMovements
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshSurface = FindFirstObjectByType<NavMeshSurface>();
        UpdateNavMesh();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MoveToTarget();
    }
}
