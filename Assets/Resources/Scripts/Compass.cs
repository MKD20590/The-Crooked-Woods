using UnityEngine;

public class Compass : Collectibles
{
    Player player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindFirstObjectByType<Player>();
    }

    public void Collected()
    {

        OnCollect();
    }
}
