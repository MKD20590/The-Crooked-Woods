using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    [SerializeField] private List<Image> mapAreas;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OpenArea(string areaName)
    {
        switch(areaName)
        {
            case "Forest":
                mapAreas[0].enabled = true;
                break;
            case "Flower":
                mapAreas[1].enabled = true;
                break;
            case "Mountain":
                mapAreas[2].enabled = true;
                break;
            case "Lake":
                mapAreas[2].enabled = true;
                break;
            default:
                Debug.LogWarning("Area not found: " + areaName);
                break;
        }
    }
}
