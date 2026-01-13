using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;

public class Player : MonoBehaviour
{
    [SerializeField] private List<Transform> teleportPositions;
    [SerializeField] private GameObject hand;

    [SerializeField] private GameObject helicopter;
    [SerializeField] private GameObject cursorGrab;

    [SerializeField] private int maxChildren = 5;

    [SerializeField] private List<NpcChildren> children;
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
    [SerializeField] private AudioSource jumpscareSFX;
    [SerializeField] private bool canCallChildren = true;

    [Range(0f, 1f)]
    [SerializeField] private float footstepPitchInterval;
    [SerializeField] private float footstepInterval_walk = 0.5f;
    [SerializeField] private float footstepInterval_run = 0.2f;
    Coroutine footstepCoroutine;
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
    public bool isHiding = false;
    bool canHide = true;

    [SerializeField] private Animator anim;
    GameManager gm;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gm = FindFirstObjectByType<GameManager>();
        rb = GetComponent<Rigidbody>();
        cam = Camera.main;
        foreach (Material mat in journal_childrenMaterial)
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
        isGrounded = Physics.Raycast(rayGround1, out groundHit, 2.0f, groundLayer) ||
            Physics.Raycast(rayGround2, out groundHit, 2.0f, groundLayer) ||
            Physics.Raycast(rayGround3, out groundHit, 2.0f, groundLayer) ||
            Physics.Raycast(rayGround4, out groundHit, 2.0f, groundLayer);

        //raycast for interactable object
        Ray rayInteractable = new Ray(cam.transform.position, cam.transform.forward);
        canInteract = isHiding ? true : Physics.Raycast(rayInteractable, out interactHit, 3.5f, interactableLayer);
        cursorGrab.SetActive(canInteract);

        //hunger depleting
        if(hunger > 40)
        {
            hunger -= isSprinting ? Time.deltaTime * 1.0f : Time.deltaTime;
        }
        else
        {
            hunger = 40;
        }

        //movement
        movement = (cam.transform.forward * direction.y) + (cam.transform.right * direction.x);
        movement.y = 0f;
        movement.Normalize();
        movement = Vector3.ProjectOnPlane(movement, groundHit.normal).normalized;

        //stamina depleting
        if (isSprinting && stamina > 0)
        {
            stamina -= Time.deltaTime * 3f;
        }
        //stamina recharge
        else if (!isSprinting && stamina < 50f)
        {
            stamina += Time.deltaTime * 5f;
        }
        currentSpeed = isSprinting && hunger > 0 && stamina > 0 ? sprintSpeed : normalSpeed;
        if (!gm.isWin && !gm.isMonsterEating && !isHiding)
        {
            if (isJumped && isGrounded)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
                //play 1 time footstep sound when jump
                jumpAudioSource.Stop();
                jumpAudioSource.pitch = 1f;
                jumpAudioSource.transform.position = new Vector3(transform.position.x, transform.position.y - 0.7f, transform.position.z);
                jumpAudioSource.Play();
            }
            //jump mechanic 2
/*            else if (!isGrounded)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y, rb.linearVelocity.z);
            }*/
            //jump mechanic 1
            else
            {
                rb.linearVelocity = new Vector3(movement.x * currentSpeed, rb.linearVelocity.y, movement.z * currentSpeed);
            }
        }
        else if(isHiding || gm.isMonsterEating)
        {
            StopCoroutine(footstepCoroutine);
            rb.linearVelocity = Vector3.zero;
            direction = Vector3.zero;
            isSprinting = false;
        }
        CameraEffects();
    }
    void CameraEffects()
    {
        if (direction != Vector2.zero && isGrounded && !isHiding)
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
        if (ctx.phase == InputActionPhase.Performed || ctx.phase == InputActionPhase.Canceled)
        {
            direction = !isHiding && !gm.isWin ? ctx.ReadValue<Vector2>() : Vector2.zero;
            if (ctx.phase == InputActionPhase.Performed)
            { 
                footstepCoroutine = StartCoroutine(FootstepSFX());
            }
            else if(ctx.phase == InputActionPhase.Canceled)
            {
                StopCoroutine(footstepCoroutine);
            }
        }
    }
    public void Sprint(CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Performed || ctx.phase == InputActionPhase.Canceled)
        {
            isSprinting = !isHiding && !gm.isWin ? ctx.ReadValueAsButton() : false;
        }
    }
    public void Jump(CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Performed || ctx.phase == InputActionPhase.Canceled)
        {
            isJumped = !isHiding && !gm.isWin ? ctx.ReadValueAsButton() : false;
        }
    }
    void ResetHide()
    {
        canHide = true;
    }
    IEnumerator FootstepSFX()
    {
        while (true)
        {
            if (isGrounded)
            {
                //play footstep sound
                footstepAudioSource.Stop();
                footstepAudioSource.transform.position = new Vector3(transform.position.x, transform.position.y - 0.7f, transform.position.z);
                footstepAudioSource.pitch = Random.Range(1f - footstepPitchInterval, 1f + footstepPitchInterval);
                footstepAudioSource.Play();
            }
            else
            {
                footstepAudioSource.Stop();
            }
            float delay = 0;
            if (isGrounded && isSprinting && stamina <= 0 || isGrounded && !isSprinting)
            {
                delay = footstepInterval_walk;
            }
            else if (isGrounded && isSprinting && stamina > 0)
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
        if (gameObject.name == "Player" && !gm.isWin)
        {
            if (ctx.phase == InputActionPhase.Performed && canInteract)
            {
                Debug.Log("interact");
                //food
                if (interactHit.collider.gameObject.tag == "Food")
                {
                    Apple apple = null;
                    interactHit.collider.TryGetComponent<Apple>(out apple);
                    if (apple != null) apple.Collected();
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
                //tent
                else if (interactHit.collider.gameObject.tag == "Tent")
                {
                    //for children hiding mechanic
                    if (rescuedChildren.Count > 0)
                    {
                        //pas tentnya closed & anak lg hiding di dlmnya
                        if(interactHit.collider.transform.GetChild(0).gameObject.activeSelf)
                        {
                            //children yg lg hiding di tentnya
                            foreach (NpcChildren child in rescuedChildren)
                            {
                                if (child.hidingSpot == interactHit.collider.gameObject)
                                {
                                    child.Hiding(interactHit.collider.transform);
                                    return;
                                }
                            }
                            Hide();
                        }

                        //pas tentnya open
                        else
                        {
                            if(companionChild != null && !companionChild.isHiding)
                            {
                                companionChild.Hiding(interactHit.collider.transform);
                                return;
                            }
                            else
                            {
                                for(int i = 0; i < rescuedChildren.Count; i++)
                                {
                                    if (!rescuedChildren[i].isHiding)
                                    {
                                        rescuedChildren[i].Hiding(interactHit.collider.transform);
                                        return;
                                    }
                                }
                                Hide();
                            }
                        }
                    }
                    else
                    {
                        Hide();
                    }
                }
                else if(isHiding)
                {
                    Hide();
                }
            }
        }
    }
    void Hide()
    {
        if (canHide)
        {
            isHiding = !isHiding;
            canHide = false;
            hand.SetActive(!isHiding);
            if (isHiding)
            {
                interactHit.collider.GetComponent<CapsuleCollider>().enabled = false;
                interactHit.collider.transform.GetChild(0).gameObject.SetActive(true);
                interactHit.collider.transform.GetChild(1).gameObject.SetActive(false);
                transform.position = new Vector3(interactHit.collider.transform.position.x, transform.position.y, interactHit.collider.transform.position.z);
                rb.useGravity = false;
                CapsuleCollider collider = GetComponent<CapsuleCollider>();
                collider.enabled = !isHiding;
                isSprinting = false;
                direction = Vector2.zero;
                isJumped = false;
            }
            else
            {
                interactHit.collider.transform.GetChild(0).gameObject.SetActive(false);
                interactHit.collider.transform.GetChild(1).gameObject.SetActive(true);
                Vector3 outPosition = interactHit.collider.transform.forward * 2f;
                transform.position = new Vector3(interactHit.collider.transform.position.x + outPosition.x, transform.position.y, interactHit.collider.transform.position.z + outPosition.z);
                rb.useGravity = true;
                CapsuleCollider collider = GetComponent<CapsuleCollider>();
                collider.enabled = !isHiding;
                interactHit.collider.GetComponent<CapsuleCollider>().enabled = true;
            }
            Invoke("ResetHide", 1f);
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
                canCallChildren = false;
                callVoice.clip = callVoices[Random.Range(0, callVoices.Count)];
                callVoice.Play();
                StartCoroutine(CallForChildren());
            }
        }
    }
    IEnumerator CallForChildren()
    {
        foreach (NpcChildren child in children)
        {
            yield return new WaitForSeconds(Random.Range(1f,2f));
            if (!rescuedChildren.Contains(child))
            {
                child.CallOut();
            }
            Debug.Log("calling child " + child.childrenIdx);
        }
        canCallChildren = true;
    }
    public void RescueChild(NpcChildren child)
    {
        journal_childrenMaterial[child.childrenIdx].SetTexture("_Texture2D", journal_childrenFound[child.childrenIdx]);
        rescuedChildren.Add(child);
    }
    public bool IsAllChildrenRescued()
    {
        return rescuedChildren.Count >= maxChildren;
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
        if (companionChild != null)
        {
            jumpscareSFX.Play();
            gm.MonsterEats();
            AddHunger(hungerBar.maxValue);
            rescuedChildren.Remove(companionChild);
            companionChild.GetEaten();
            companionChild = null;
        }
        else
        {
            List<NpcChildren> children = new List<NpcChildren>();
            foreach (NpcChildren child in rescuedChildren)
            {
                if(!child.isHiding)
                {
                    children.Add(child);
                }
            }
            if(children.Count <= 0)
            {
                gm.MonsterGetPlayer();
                transform.position = teleportPositions[Random.Range(0, teleportPositions.Count)].position;
            }
            else
            {
                jumpscareSFX.Play();
                gm.MonsterEats();
                AddHunger(hungerBar.maxValue);
                int randomIdx = Random.Range(0, children.Count);
                journal_childrenDead[children[randomIdx].childrenIdx].SetActive(true);
                children[randomIdx].GetEaten();
                rescuedChildren.Remove(children[randomIdx]);
                maxChildren--;
            }
        }
    }
    public void AddHunger(float value)
    {
        hunger += value;
        if (hunger > hungerBar.maxValue)
        {
            hunger = hungerBar.maxValue;
        }
    }
    public int GetRescuedChildrenCount()
    {
        return rescuedChildren.Count;
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Win" && rescuedChildren.Find(x => !x.GetComponent<NavMeshAgent>().enabled) == null)
        {
            gm.Win();
        }
    }
}
