using UnityEngine;

public class MapTrigger : MonoBehaviour
{
    [SerializeField] private string areaName;
    MapManager mapManager;
    bool hasTriggered = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mapManager = FindAnyObjectByType<MapManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;
        hasTriggered = true;
        mapManager.OpenArea(areaName);
    }
}
