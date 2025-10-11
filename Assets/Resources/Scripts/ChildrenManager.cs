using System.Collections.Generic;
using UnityEngine;

public class ChildrenManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> children;
    [SerializeField] private List<Transform> positions;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        List<int> index = new List<int>();
        for(int i = 0; i < children.Count; i++)
        {
            index.Add(i);
        }
        foreach (Transform pos in positions)
        {
            int idx = Random.Range(0, index.Count);
            children[index[idx]].transform.position = pos.position;
            index.RemoveAt(idx);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
