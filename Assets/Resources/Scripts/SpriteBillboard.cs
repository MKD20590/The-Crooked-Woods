using UnityEngine;

public class SpriteBillboard : MonoBehaviour
{
    [Header("Sprite billboard settings")]
    [SerializeField] private BillboardType billboardType;
    [SerializeField] private GameObject sprite;

    [Header("Lock Rotation")]
    [SerializeField] private bool lockX;
    [SerializeField] private bool lockY;
    [SerializeField] private bool lockZ;

    private Vector3 originalRotation;
    public enum BillboardType { LookAtCamera, CameraForward };
    private void Start()
    {
        originalRotation = sprite.transform.rotation.eulerAngles;
    }

    private void FixedUpdate()
    {
        switch (billboardType)
        {
            case BillboardType.LookAtCamera:
                sprite.transform.LookAt(Camera.main.transform.position, Vector3.up);
                break;
            case BillboardType.CameraForward:
                sprite.transform.forward = Camera.main.transform.forward;
                break;
            default:
                break;
        }
        Vector3 rotation = sprite.transform.rotation.eulerAngles;
        if (lockX) { rotation.x = originalRotation.x; }
        if (lockY) { rotation.y = originalRotation.y; }
        if (lockZ) { rotation.z = originalRotation.z; }
        sprite.transform.rotation = Quaternion.Euler(rotation);
    }
}
