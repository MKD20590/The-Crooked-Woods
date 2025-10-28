using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class NpcChildren : NpcMovements
{
    public int childrenIdx = 0;
    [SerializeField] private AudioSource scream;
    [SerializeField] private float minDistance = 3f;

    Player player;
    bool isLost = true;
    bool isHiding = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindFirstObjectByType<Player>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshSurface = FindFirstObjectByType<NavMeshSurface>();
        SetTarget(player.transform);
        //UpdateNavMesh();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Vector3.Distance(transform.position, player.transform.position) <= minDistance && isLost)
        {
            isLost = false;
            player.RescueChild(this);
        }
        if(!isLost && !isHiding)
        {
            MoveToTarget();
        }
    }
    public void CallOut()
    {
        scream.Play();
    }
    public void Hiding(Transform hidingSpot)
    {
        isHiding = !isHiding;
        if(!isHiding)
        {
            SetTarget(player.transform);
        }
        else
        {
            SetTarget(hidingSpot);
        }
    }
    void SetTarget(Transform target)
    {
        this.target = target;
    }
    public void GetEaten()
    {
        Destroy(gameObject);
    }
}
