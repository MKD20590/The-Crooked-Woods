using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class NpcChildren : NpcMovements
{
    public int childrenIdx = 0;
    [SerializeField] private AudioSource scream;
    [SerializeField] private float minDistance = 3f;
    public GameObject hidingSpot;
    CapsuleCollider coll;

    Player player;
    bool isLost = true;
    public bool isHiding = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        coll = GetComponent<CapsuleCollider>();
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
        this.hidingSpot = hidingSpot.gameObject;
        isHiding = !isHiding;
        if(!isHiding)
        {
            coll.enabled = true;
            hidingSpot.transform.GetChild(0).gameObject.SetActive(false);
            hidingSpot.transform.GetChild(1).gameObject.SetActive(true);
            Vector3 outPosition = hidingSpot.transform.forward * 2f;
            transform.position = new Vector3(hidingSpot.transform.position.x + outPosition.x, transform.position.y, hidingSpot.transform.position.z + outPosition.z);
        }
        else
        {
            coll.enabled = false;
            hidingSpot.transform.GetChild(0).gameObject.SetActive(true);
            hidingSpot.transform.GetChild(1).gameObject.SetActive(false);
            transform.position = new Vector3(hidingSpot.position.x, transform.position.y, hidingSpot.position.z);
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
