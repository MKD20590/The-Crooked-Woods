using UnityEngine;

public class MapTrigger : MonoBehaviour
{
    [SerializeField] private string areaName;
    MapManager mapManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        areaName = gameObject.name;
        mapManager = FindAnyObjectByType<MapManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            mapManager.OpenArea(areaName);
            gameObject.SetActive(false);
        }
    }
}
