using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class NpcMovements : MonoBehaviour
{
    public float speed = 2.0f;
    public float minDistance = 1f;

    public NavMeshSurface navMeshSurface;
    public NavMeshAgent navMeshAgent;

    public Transform target;

    public bool canMove = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    public void UpdateNavMesh()
    {
        navMeshSurface.BuildNavMesh();
    }
    public void MoveToTarget()
    {
        navMeshAgent.speed = speed;
        navMeshAgent.stoppingDistance = minDistance;
        navMeshAgent.SetDestination(target.position);
    }
}
