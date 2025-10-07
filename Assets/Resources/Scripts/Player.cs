using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class Player : MonoBehaviour
{
    [SerializeField] private float hunger = 100f;
    [SerializeField] private float stamina = 50f;
    [SerializeField] private AudioClip footstepClip_grass;
    [SerializeField] private AudioClip footstepClip_path;
    [SerializeField] private AudioSource footstepAudioSource;
    [SerializeField] private AudioSource jumpAudioSource;
    [Range(0f, 1f)]
    [SerializeField] private float footstepPitchInterval;
    [SerializeField] private float footstepInterval_walk = 0.5f;
    [SerializeField] private float footstepInterval_run = 0.2f;
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
        //grounding check
        Ray ray = new Ray(transform.position, -transform.up);
        isGrounded = Physics.Raycast(ray, out hit, 1.05f, groundLayer);

        //movement
        movement = (cam.transform.forward * direction.y) + (cam.transform.right * direction.x);
        movement.y = 0f;
        movement.Normalize();
        movement = Vector3.ProjectOnPlane(movement, hit.normal).normalized;

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
            //play 1 time footstep sound when jump
            jumpAudioSource.Stop();
            jumpAudioSource.transform.position = new Vector3(transform.position.x, transform.position.y - 0.7f, transform.position.z);
            jumpAudioSource.Play();
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
                if (ctx.phase == InputActionPhase.Performed)
                {
                    StopAllCoroutines();
                    StartCoroutine(FootstepSFX());
                }
                else
                {
                    StopAllCoroutines();
                }
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
            StopAllCoroutines();
            StartCoroutine(FootstepSFX());
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
    IEnumerator FootstepSFX()
    {
        while(true)
        {
            if(isGrounded)
            {
                //play footstep sound
                footstepAudioSource.transform.position = new Vector3(transform.position.x, transform.position.y-0.7f, transform.position.z);
                footstepAudioSource.pitch = Random.Range(1f - footstepPitchInterval, 1f + footstepPitchInterval);
                footstepAudioSource.Play();
            }
            else
            {
                footstepAudioSource.Stop();
            }
            float delay = 0;
            if(isGrounded && isSprinting && stamina <= 0 || isGrounded && !isSprinting)
            {
                delay = footstepInterval_walk;
            }
            else if(isGrounded && isSprinting && stamina > 0)
            {
                delay = footstepInterval_run;
            }
            else if (!isGrounded)
            {
                delay = 0;
            }
            yield return new WaitForSeconds(delay);                
        }
    }
    public void ChangeFootstepSFX(string surface)
    {
        switch (surface)
        {
            case "Grass":
                footstepAudioSource.clip = footstepClip_grass;
                break;
            case "Path":
                footstepAudioSource.clip = footstepClip_path;
                break;
            default:
                footstepAudioSource.clip = footstepClip_grass;
                break;
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
