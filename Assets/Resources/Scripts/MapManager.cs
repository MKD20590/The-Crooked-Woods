using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    [SerializeField] private List<MeshRenderer> mapAreas;
    [SerializeField] private GameObject helicopter;
    Player player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindFirstObjectByType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if(player.IsAllChildrenRescued() && !helicopter.activeSelf)
        {
            helicopter.SetActive(true);
        }
    }
    public void OpenArea(string areaName)
    {
        switch(areaName)
        {
            case "Forest1":
                mapAreas[0].enabled = true;
                break;
            case "Forest2":
                mapAreas[1].enabled = true;
                break;
            case "Flower":
                mapAreas[2].enabled = true;
                break;
            case "Mountain":
                mapAreas[3].enabled = true;
                break;
            case "Lake":
                mapAreas[4].enabled = true;
                break;
            default:
                Debug.LogWarning("Area not found: " + areaName);
                break;
        }
    }
}
