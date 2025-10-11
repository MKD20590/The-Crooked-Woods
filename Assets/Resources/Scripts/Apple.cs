using UnityEngine;

public class Apple : Collectibles
{
    Player player;
    [SerializeField] private float duration = 25f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindFirstObjectByType<Player>();
    }
    private void Update()
    {
        if(duration > 0)
        {
            duration -= Time.deltaTime;
        }
        else
        {
            OnCollect();
        }
    }
    public void Collected()
    {
        if (player != null)
        {
            player.AddHunger(duration * 2f);
        }
        OnCollect();
    }
}
