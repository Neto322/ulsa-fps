using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

[RequireComponent(typeof(Rigidbody))]

public class PlayerController : NetworkBehaviour
{
    Rigidbody rb;
    PlayerInputs playerInputs;
    [SerializeField, Range(0.1f, 10f)]
    float moveSpeed = 5f;
    [SerializeField]
    Transform camTrs;
    [SerializeField, Range(0.1f, 180f)]
    float camRotSpeed = 90f;
    float camRotationAmounthY;
    float camRotationAmounthX;
    [SerializeField, Range(-150, 150)]
    float camMaxRotation = 45f;
    [SerializeField, Range(0.1f, 15f)]
    float jumpForce = 5f;
    [SerializeField, Range(0.01f, 10f)]
    float rayLength = 1f;
    [SerializeField]
    Color rayColor = Color.magenta;
    [SerializeField]
    LayerMask detectionLayer;
    [SerializeField]
    Vector3 rayPosition;
    [SerializeField, Range(1f, 5f)]
    float augmentedFactor = 1f;
    float augmentedSpeed = 1f;
    float baseSpeed = 1f;
    [SerializeField]
    List<Weapon> weapons;
    int weaponIndex = 0;
    AudioSource audioSource;
    [SerializeField]
    AudioClip walkFootStepSFX;
    [SerializeField, Range(-2f, 2f)]
    float walkFootStepPitch = 1;
    [SerializeField]
    AudioClip runFootStepSFX;
    [SerializeField, Range(-2f, 2f)]
    float runFootStepPitch = 1;
    [SerializeField]
    AudioClip jumpoSFX;
    bool isRunning;

    [SerializeField]
    Health_Component health_Component;

    public override void NetworkStart()
    {
        base.NetworkStart();
        /*foreach(MLAPI.Connection.NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
        {
            Debug.Log(client.PlayerObject.name);
        }*/
        /*NetworkManager.Singleton.OnClientConnectedCallback += client =>{
            Debug.Log(client);
        };*/
        //NetworkObject.name = Gamemanager.instance.currentUsername;
        //Debug.Log(NetworkObject.name);
        //NetworkManager.Singleton.LocalClientId;
    }

    void Awake()
    {
        rb ??= GetComponent<Rigidbody>();
        playerInputs ??= new PlayerInputs();
        audioSource ??= GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        playerInputs?.Enable();
    }

    void OnDisable()
    {
        playerInputs?.Disable();
    }

    void Start()
    {
        if(IsLocalPlayer)
        {
            Cursor.lockState = CursorLockMode.Locked;
            playerInputs.Gameplay.Jump.performed += _=> Jump();
            playerInputs.Gameplay.Run.performed += _=> Run();
            playerInputs.Gameplay.Run.canceled += _=> CancelRun();
            playerInputs.Gameplay.Shoot.performed += _=> Shoot();
            playerInputs.Gameplay.Movement.performed += _=> Movement();
            playerInputs.Gameplay.Movement.canceled += _=> CancelMovement();
            playerInputs.Gameplay.WeaponChange.performed += _=> WeaponChange();
        }
        else
        {
            camTrs.gameObject.SetActive(false);
        }
    }

    void WeaponChange()
    {
        if(WheelAxisYClampInt != 0f)
        {
            CurrentWeapon.Active(false);

            if(WheelAxisYClampInt + weaponIndex >= 0)
            {
                if(WheelAxisYClampInt + weaponIndex < weapons.Count)
                {
                    weaponIndex += WheelAxisYClampInt;
                }
                else
                {
                    if(WheelAxisYClampInt + weaponIndex >= weapons.Count)
                    {
                        weaponIndex = 0;
                    }
                }
            }
            else
            {
                weaponIndex = weapons.Count - 1;
            }

            CurrentWeapon.Active(true);
        }
    }

    void Shoot()
    {
        Debug.Log("Shot");
        CurrentWeapon.Shoot();
    }

    void Run()
    {
        augmentedSpeed = augmentedFactor;

        if(!Grounding) return;
        audioSource.clip = runFootStepSFX;
        audioSource.loop = true;
        audioSource.pitch = runFootStepPitch;
        audioSource?.Play();
        isRunning = true;
    }

    void CancelRun()
    {
        augmentedSpeed = baseSpeed;
        audioSource?.Stop();
        audioSource.clip = null;
        audioSource.loop = false;
        audioSource.pitch = 1f;
        if(Axis != Vector2.zero)
        {
            audioSource.clip = walkFootStepSFX;
            audioSource.loop = true;
            audioSource.pitch = walkFootStepPitch;
            audioSource?.Play();
        }
        isRunning = false;
    }

    void Movement()
    {
        if(audioSource.isPlaying || !Grounding) return;
        audioSource.clip = walkFootStepSFX;
        audioSource.loop = true;
        audioSource.pitch = walkFootStepPitch;
        audioSource?.Play();
    }

    void CancelMovement()
    {
        audioSource?.Stop();
        audioSource.clip = null;
        audioSource.loop = false;
        audioSource.pitch = 1f;
    }

    void Update()
    {
        if(!IsLocalPlayer) return;
        //if(!NetworkManager.Singleton.IsHost) return;
        camRotationAmounthX -= CamAxis.y * camRotSpeed * Time.deltaTime;
        camRotationAmounthX = Mathf.Clamp(camRotationAmounthX, -camMaxRotation, camMaxRotation);
        camTrs.localRotation = Quaternion.Euler(camRotationAmounthX, camTrs.rotation.y, camTrs.rotation.z);
        camRotationAmounthY += CamAxis.x * camRotSpeed * Time.deltaTime;
        rb.rotation = Quaternion.Euler(rb.rotation.x, camRotationAmounthY, rb.rotation.z);
        rb.position += Forward * moveSpeed * augmentedSpeed * Time.deltaTime;

        if(Grounding && !audioSource.isPlaying && Forward != Vector3.zero)
        {
            if(isRunning)
            {
                Run();
            }
            else
            {
                Movement();
            }
        }
    }

    void Jump()
    {
        if(!Grounding) return;
        audioSource?.Stop();
        audioSource.PlayOneShot(jumpoSFX);
        rb.AddForce(JumpDirection, ForceMode.Impulse);
        health_Component.TakeDamage(10);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = rayColor;
        Gizmos.DrawRay(RelativeRayPosition, -transform.up * rayLength);
    }

    bool Grounding => Physics.Raycast(RelativeRayPosition, -transform.up, rayLength, detectionLayer);

    Vector2 Axis => playerInputs.Gameplay.Movement.ReadValue<Vector2>();

    float CamAxisX => playerInputs.Gameplay.CamAxisX.ReadValue<float>();
    float CamAxisY => playerInputs.Gameplay.CamAxisY.ReadValue<float>();

    Vector2 CamAxis => new Vector2(CamAxisX, CamAxisY);

    Vector3 MovementAxis => new Vector3(Axis.x, 0f, Axis.y);

    Vector3 Forward => rb.rotation * MovementAxis;
    Vector3 JumpDirection => Vector3.up * jumpForce;

    Vector3 RelativeRayPosition => rayPosition + transform.position;

    int WheelAxisYClampInt => (int)Mathf.Ceil(WheelAxisYClamped);

    float WheelAxisYClamped => Mathf.Clamp(WheelAxisY, -1, 1);

    float WheelAxisY => playerInputs.Gameplay.WeaponChange.ReadValue<float>();

    Weapon CurrentWeapon => weapons[weaponIndex];
}
