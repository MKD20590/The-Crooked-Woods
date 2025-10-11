using UnityEngine;

public class Collectibles : MonoBehaviour
{
    public void OnCollect()
    {
        // Default implementation (can be empty)
        Destroy(gameObject);
    }
}
