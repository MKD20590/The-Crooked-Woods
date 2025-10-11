using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class NpcMovements : MonoBehaviour
{
    public float speed = 2.0f;
    public float minDistanceStopping = 1f;

    public NavMeshSurface navMeshSurface;
    public NavMeshAgent navMeshAgent;

    public Transform target;

    public bool canMove = true;
    public void UpdateNavMesh()
    {
        navMeshSurface.BuildNavMesh();
    }
    public void MoveToTarget()
    {
        navMeshAgent.speed = speed;
        navMeshAgent.stoppingDistance = minDistanceStopping;
        navMeshAgent.SetDestination(target.position);
    }
}
