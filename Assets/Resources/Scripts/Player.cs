using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class Player : MonoBehaviour
{
    [SerializeField] private AudioSource footstepAudioSource;
    [SerializeField] private CinemachineBasicMultiChannelPerlin cinemachineNoise;
    Rigidbody rb;
    [SerializeField] private Vector3 movement = Vector3.zero;
    Vector2 direction = Vector2.zero;
    [SerializeField] private float normalSpeed = 5f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float currentSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    bool isJumped = false;
    bool isSprinting = false;
    Camera cam;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private bool isGrounded = false;
    RaycastHit hit;
    float camAmplitudeGain;
    float camFrequencyGain;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        //grouunding check
        Ray ray = new Ray(transform.position, -transform.up);
        isGrounded = Physics.Raycast(ray, out hit, 1.2f, groundLayer);
        //movement
        movement = (cam.transform.forward * direction.y) + (cam.transform.right * direction.x);
        movement.y = 0f;
        movement.Normalize();
        currentSpeed = isSprinting ? sprintSpeed : normalSpeed;
        if(isJumped && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
        }
        CameraEffects();
    }
    void CameraEffects()
    {
        if (direction != Vector2.zero)
        {
            camAmplitudeGain = isSprinting ? 1.5f : 1f;
            camFrequencyGain = isSprinting ? 0.2f : 0.15f;
            rb.linearVelocity = new Vector3(movement.x * currentSpeed, rb.linearVelocity.y, movement.z * currentSpeed);
        }
        else
        {
            camAmplitudeGain = 0.2f;
            camFrequencyGain = 0.1f;
            rb.linearVelocity = Vector3.MoveTowards(rb.linearVelocity, new Vector3(0, rb.linearVelocity.y, 0), Time.deltaTime * 10f);
        }
        cinemachineNoise.AmplitudeGain = Mathf.Lerp(cinemachineNoise.AmplitudeGain, camAmplitudeGain, Time.deltaTime * 10f);
        cinemachineNoise.FrequencyGain = Mathf.Lerp(cinemachineNoise.FrequencyGain, camFrequencyGain, Time.deltaTime * 10f);
    }
    public void Moving(CallbackContext ctx)
    {
        if (gameObject.name == "Player")
        {
            if (ctx.phase == InputActionPhase.Performed || ctx.phase == InputActionPhase.Canceled)
            {
                direction = ctx.ReadValue<Vector2>();
            }
        }
    }
    public void Sprint(CallbackContext ctx)
    {
        if (gameObject.name == "Player")
        {
            if (ctx.phase == InputActionPhase.Performed || ctx.phase == InputActionPhase.Canceled)
            {
                isSprinting = ctx.ReadValueAsButton();
            }
        }
    }
    public void Jump(CallbackContext ctx)
    {
        if (gameObject.name == "Player")
        {
            if (ctx.phase == InputActionPhase.Performed || ctx.phase == InputActionPhase.Canceled)
            {
                isJumped = ctx.ReadValueAsButton();
            }
        }
    }
    public void Interact(CallbackContext ctx)
    {
        if (gameObject.name == "Player")
        {
            if (ctx.phase == InputActionPhase.Performed)
            {
                Debug.Log("interact");
            }
        }
    }
}
