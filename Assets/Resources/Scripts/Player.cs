using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class Player : MonoBehaviour
{
    [SerializeField] private float hunger = 100f;
    [SerializeField] private float stamina = 50f;
    [SerializeField] private AudioSource footstepAudioSource;
    [SerializeField] private CinemachineBasicMultiChannelPerlin cinemachineNoise;
    Rigidbody rb;
    [SerializeField] private Vector3 movement = Vector3.zero;
    [SerializeField] private Vector2 direction = Vector2.zero;
    [SerializeField] private float normalSpeed = 5f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float currentSpeed = 5f;
    [SerializeField] private bool isSprinting = false;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private bool isJumped = false;
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
        isGrounded = Physics.Raycast(ray, out hit, 1.1f, groundLayer);

        //movement

        movement = (cam.transform.forward * direction.y) + (cam.transform.right * direction.x);
        movement.y = 0f;
        movement.Normalize();

        if (isSprinting && stamina > 0)
        {
            stamina -= Time.deltaTime * 5f;
        }
        else if(!isSprinting && stamina < 50f)
        {
            stamina += Time.deltaTime * 3f;
        }
        currentSpeed = isSprinting && stamina > 0 ? sprintSpeed : normalSpeed;
        if (isJumped && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
        }
        else
        {
            rb.linearVelocity = new Vector3(movement.x * currentSpeed, rb.linearVelocity.y, movement.z * currentSpeed);
        }
        CameraEffects();
    }
    void CameraEffects()
    {
        if (direction != Vector2.zero)
        {
            camAmplitudeGain = isSprinting ? 1.5f : 1f;
            camFrequencyGain = isSprinting ? 0.2f : 0.1f;
        }
        else
        {
            camAmplitudeGain = 0.2f;
            camFrequencyGain = 0.1f;
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
    public float GetHunger()
    {
        return hunger;
    }
    public void AddHunger(float value)
    {
        hunger += value;
    }
}
