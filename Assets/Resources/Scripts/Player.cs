using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject helicopter;
    [SerializeField] private GameObject cursorGrab;

    [SerializeField] private int maxChildren = 5;

    public List<NpcChildren> rescuedChildren;
    [SerializeField] private List<Material> journal_childrenMaterial;
    [SerializeField] private List<Texture2D> journal_childrenNotFound;
    [SerializeField] private List<Texture2D> journal_childrenFound;
    [SerializeField] private List<GameObject> journal_childrenDead;
    [SerializeField] private NpcChildren companionChild;

    [SerializeField] private float hunger = 100f;
    [SerializeField] private Slider hungerBar;
    [SerializeField] private float stamina = 50f;
    [SerializeField] private Slider staminaBar;

    [SerializeField] private AudioClip footstepClip_grass;
    [SerializeField] private AudioClip footstepClip_path;
    [SerializeField] private AudioSource footstepAudioSource;
    [SerializeField] private AudioSource jumpAudioSource;

    [SerializeField] private List<AudioClip> callVoices;
    [SerializeField] private AudioSource callVoice;
    bool canCallChildren = true;

    [Range(0f, 1f)]
    [SerializeField] private float footstepPitchInterval;
    [SerializeField] private float footstepInterval_walk = 0.5f;
    [SerializeField] private float footstepInterval_run = 0.2f;
    [SerializeField] private CinemachineBasicMultiChannelPerlin cinemachineNoise;
    Rigidbody rb;
    [SerializeField] private Vector3 movement = Vector3.zero;
    [SerializeField] private Vector2 direction = Vector2.zero;
    [SerializeField] private float currentSpeed = 5f;
    [SerializeField] private float normalSpeed = 5f;

    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private bool isSprinting = false;

    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private bool isJumped = false;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private bool isGrounded = false;
    RaycastHit groundHit;

    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private bool canInteract = false;
    RaycastHit interactHit;

    Camera cam;
    float camAmplitudeGain;
    float camFrequencyGain;

    [SerializeField] private Animator anim;
    GameManager gm;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gm = FindFirstObjectByType<GameManager>();
        rb = GetComponent<Rigidbody>();
        cam = Camera.main;
        foreach(Material mat in journal_childrenMaterial)
        {
            mat.SetTexture("_Texture2D", journal_childrenNotFound[journal_childrenMaterial.IndexOf(mat)]);
        }
        hungerBar.maxValue = hunger;
        staminaBar.maxValue = stamina;
    }

    // Update is called once per frame
    void Update()
    {
        //bar
        hungerBar.value = hunger;
        staminaBar.value = stamina;

        //winning condition
        if (rescuedChildren.Count >= maxChildren && helicopter != null)
        {
            helicopter.SetActive(true);
        }

        //grounding check
        Ray rayGround1 = new Ray(new Vector3(transform.position.x, transform.position.y, transform.position.z + 0.5f), -transform.up);
        Ray rayGround2 = new Ray(new Vector3(transform.position.x, transform.position.y, transform.position.z - 0.5f), -transform.up);
        Ray rayGround3 = new Ray(new Vector3(transform.position.x + 0.5f, transform.position.y, transform.position.z), -transform.up);
        Ray rayGround4 = new Ray(new Vector3(transform.position.x - 0.5f, transform.position.y, transform.position.z), -transform.up);
        isGrounded = Physics.Raycast(rayGround1, out groundHit, 1.1f, groundLayer) ||
            Physics.Raycast(rayGround2, out groundHit, 1.1f, groundLayer) ||
            Physics.Raycast(rayGround3, out groundHit, 1.1f, groundLayer) ||
            Physics.Raycast(rayGround4, out groundHit, 1.1f, groundLayer);

        //raycast for interactable object
        Ray rayInteractable = new Ray(cam.transform.position, cam.transform.forward);
        canInteract = Physics.Raycast(rayInteractable, out interactHit, 2.5f, interactableLayer);
        cursorGrab.SetActive(canInteract);

        //hunger depleting
        hunger -= isSprinting ? Time.deltaTime * 1.5f : Time.deltaTime;

        //movement
        movement = (cam.transform.forward * direction.y) + (cam.transform.right * direction.x);
        movement.y = 0f;
        movement.Normalize();
        movement = Vector3.ProjectOnPlane(movement, groundHit.normal).normalized;

        if (isSprinting && stamina > 0)
        {
            stamina -= Time.deltaTime * 5f;
        }
        else if(!isSprinting && stamina < 50f)
        {
            stamina += Time.deltaTime * 3f;
        }
        currentSpeed = isSprinting && hunger > 0 && stamina > 0 ? sprintSpeed : normalSpeed;
        if(!gm.isWin)
        {
            if (isJumped && isGrounded)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
                //play 1 time footstep sound when jump
                jumpAudioSource.Stop();
                jumpAudioSource.transform.position = new Vector3(transform.position.x, transform.position.y - 0.7f, transform.position.z);
                jumpAudioSource.Play();
            }
            else if(!isGrounded)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y, rb.linearVelocity.z);
            }
            else
            {
                rb.linearVelocity = new Vector3(movement.x * currentSpeed, rb.linearVelocity.y, movement.z * currentSpeed);
            }
            CameraEffects();
        }
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
            if (ctx.phase == InputActionPhase.Performed && canInteract)
            {
                Debug.Log("interact");
                //food
                if(interactHit.collider.gameObject.tag == "Food")
                {
                    Apple apple = null;
                    interactHit.collider.TryGetComponent<Apple>(out apple);
                    if(apple != null) apple.Collected();
                }
                //collectibles
                else if (interactHit.collider.gameObject.tag == "Collectibles")
                {
                    //for future use
                }
                //compass
                else if (interactHit.collider.gameObject.tag == "Compass")
                {
                    Compass compass = null;
                    interactHit.collider.TryGetComponent<Compass>(out compass);
                    if (compass != null) compass.Collected();
                }
            }
        }
    }
    public void CycleInventory(CallbackContext ctx)
    {
        if (gameObject.name == "Player")
        {
            if (ctx.phase == InputActionPhase.Performed)
            {
                Debug.Log("cycle inventory");
                anim.SetTrigger("cycle");
            }
        }
    }
    public void CallChildren(CallbackContext ctx)
    {
        if (gameObject.name == "Player")
        {
            if (ctx.phase == InputActionPhase.Performed && canCallChildren)
            {
                callVoice.clip = callVoices[Random.Range(0, callVoices.Count)];
                callVoice.Play();
                Invoke("CallForChildren", 3.5f);
            }
        }
    }
    public void RescueChild(NpcChildren child)
    {
        journal_childrenMaterial[child.childrenIdx].SetTexture("_Texture2D", journal_childrenFound[child.childrenIdx]);
    }
    public void GetCompass()
    {

    }
    public float GetHunger()
    {
        return hunger;
    }
    public void GetCaught()
    {
        if(companionChild != null)
        {
            companionChild.GetEaten();
            companionChild = null;
        }
        else
        {
            int randomIdx = Random.Range(0, rescuedChildren.Count);
            rescuedChildren[randomIdx].GetEaten();
            rescuedChildren.RemoveAt(randomIdx);
            journal_childrenDead[randomIdx].SetActive(true);
            maxChildren--;
        }
        AddHunger(100f);
    }
    public void CallForChildren()
    {
        foreach(NpcChildren child in FindObjectsByType<NpcChildren>(FindObjectsSortMode.None))
        {
            child.CallOut();
        }
        Invoke("ResetCall", 5f);
    }
    void ResetCall()
    {
        canCallChildren = true;
    }
    public void AddHunger(float value)
    {
        hunger += value;
        if(hunger > 100f)
        {
            hunger = 100f;
        }
        else if(hunger < 0f)
        {
            hunger = 0f;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Win")
        {
            gm.Win();
        }
    }
}
