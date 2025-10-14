using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class NpcChildren : NpcMovements
{
    [SerializeField] private AudioSource scream;
    [SerializeField] private float minDistance = 3f;
    Player player;
    bool isLost = true;
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
        if(Vector3.Distance(transform.position, player.transform.position) <= minDistance && isLost)
        {
            isLost = false;
            if(!player.rescuedChildren.Contains(this))
            {
                player.rescuedChildren.Add(this);
            }
            MoveToTarget();
        }
        else
        {
            isLost = true;
            if (player.rescuedChildren.Contains(this))
            {
                player.rescuedChildren.Remove(this);
            }
        }
    }
    public void GetEaten()
    {
        scream.Play();
        Destroy(gameObject);
    }
}
