using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody rb;
    public Transform Among;
    public Collider GroundColl;
    public Checkpoints checkpoints;
    public Renderer Rend;
    public Material[] PlayerSkins;
    public Color[] TrailColors = { Color.red, new Color(0.1f, 1f, 0.05f), Color.yellow };

    [Space]
    [Header(" - Inputs/States - ")]
    public bool JumpInput;
    public float HorizontalInput, VerticalInput;
    public bool IsAirborne = false;
    public bool IsDrifting = false;
    public int DriftDir = 0;
    public float CoyoteTiming = 0.15f;
    private float CoyoteTimer = 0;

    [Header(" - Stats - ")]
    public float Mass = 100f;
    public float Speed = 0;
    public float Acceleration = 2;
    public float DefaultMaxSpeed = 10f;
    public float MaxSpeed = 10f;
    public float Deceleration = 4;
    public float TurnSpeed = 15;
    public float TurnDeceleration = 2;
    public float AirSpeedModifier = 0.25f;
    public float DriftBoost = 10f;
    public float BoostFalloffTime = 2f;
    public float DriftMultiplier = 0.75f;
    public float DriftChargeSpeed = 1f;
    public float DriftTurn = 20f;
    public float DriftRotation = 20f;
    public float DriftCharge = 0f;
    public float MaxSpeedMultiplier = 1f;
    public float JumpHeight = 2f;

    [Header(" - Camera - ")]
    public Camera cam;
    public float defaultFOV = 90f;

    [Header(" - Particles/Effects - ")]
    public ParticleSystem LeftDrift;
    public ParticleSystem RightDrift;
    public ParticleSystem DriftSmoke;
    public ParticleSystem ChargedUp;
    public TrailRenderer trend;
    public Animator animator;

    private Vector3 ReservedVelocity;
    public bool IsPaused = false;

    private bool flag = true;

    public void Pause()
    {
        IsPaused = true;
        ReservedVelocity = rb.velocity;
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }

    public void Resume()
    {
        IsPaused = false;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rb.velocity = ReservedVelocity;
    }

    void Awake()
    {
        Rend.material = PlayerSkins[PlayerPrefs.GetInt("SelSkin")];
        trend.material.color = TrailColors[PlayerPrefs.GetInt("SelSkin")];
    }

    // Start is called before the first frame update
    void Start()
    {
        checkpoints = FindObjectOfType<Checkpoints>();
        rb = GetComponent<Rigidbody>();
        MaxSpeed = DefaultMaxSpeed;
        rb.mass = Mass;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsPaused) return;

        HorizontalInput = Input.GetAxis("Horizontal");
        VerticalInput = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(KeyCode.Space)) JumpInput = true;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (DriftDir == 0) DriftDir = Mathf.RoundToInt(HorizontalInput);
        }
        else DriftDir = 0;
    }

    IEnumerator ModifyMaxSpeed(float boost = 5, float decel_time = 1) 
    {
        MaxSpeed += boost;
        Speed *= (MaxSpeed / (MaxSpeed - boost));
        Speed += boost/2;
        var decel_amount = boost / (decel_time * 50);
        for (int i = 0; i<decel_time*50; i++)
        {
            yield return new WaitForSeconds(0.02f);
            MaxSpeed -= decel_amount;
        }
    }

    void FixedUpdate()
    {
        if (IsPaused) return;

        var addedVel = transform.rotation * Vector3.forward * Speed;
        rb.velocity = new Vector3(addedVel.x, rb.velocity.y, addedVel.z);
        if (Mathf.Abs(rb.velocity.x) < 0.1f) rb.velocity = new Vector3(0f, rb.velocity.y, rb.velocity.z);
        if (Mathf.Abs(rb.velocity.y) < 0.1f) rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        if (Mathf.Abs(rb.velocity.z) < 0.1f) rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, 0f);

        float mod = 1f;
        if (VerticalInput < 0f && Speed > 0.3f) mod *= 4f;

        var turn = mod * HorizontalInput * Mathf.Clamp(Speed, -DefaultMaxSpeed / 8, DefaultMaxSpeed / 8) * TurnSpeed;
        if (IsDrifting) turn += DriftDir * DriftTurn;
        if (Mathf.Abs(HorizontalInput) > 0.2f || Mathf.Abs(Speed) > 0.5f) rb.angularVelocity = new Vector3(0, turn, 0);
        else rb.angularVelocity /= TurnDeceleration;

        if (IsAirborne) mod *= AirSpeedModifier;
        Speed += Acceleration * VerticalInput * Time.deltaTime * mod;

        if (Speed > MaxSpeed*MaxSpeedMultiplier) Speed = MaxSpeed*MaxSpeedMultiplier;
        if (Speed < -MaxSpeed / 5) Speed = -MaxSpeed / 5;

        if (Mathf.Abs(Speed) > 0f && Mathf.Abs(VerticalInput) < 0.3f) Speed /= Deceleration;

        if (Mathf.Abs(Speed) < 0.001f) Speed = 0;


        if (Mathf.Abs(rb.angularVelocity.y) < 0.2f) rb.angularVelocity = Vector3.zero;

        cam.fieldOfView = Mathf.Clamp(defaultFOV + Speed, 0, 140);

        if (JumpInput)
        {
            CoyoteTimer = Time.time + CoyoteTiming;
            JumpInput = false;
        }

        if (Time.time < CoyoteTimer && !IsAirborne)
        {
            Jump(JumpHeight);
            IsAirborne = true;
        }

        if (Speed > DefaultMaxSpeed*1.4f) trend.emitting = true;
        else trend.emitting = false;

        if (DriftDir != 0 && !IsAirborne && Speed > 2f)
        {                       
            IsDrifting = true;
        } else
        {
            if (IsDrifting && (DriftDir == 0 || IsAirborne))
            {
                MaxSpeedMultiplier = 1f;
                var calcBoost = DriftCharge;
                if (DriftCharge < 45) calcBoost = 0;
                StartCoroutine(ModifyMaxSpeed(DriftBoost * calcBoost / 100, BoostFalloffTime));
            }; // Boost
            IsDrifting = false; 
        }

        if (IsDrifting)
        {
            Among.localRotation = Quaternion.Euler((Mathf.Abs(DriftDir) * (-DriftRotation * 2)) - 90, 0, DriftDir * DriftRotation);
            MaxSpeedMultiplier = DriftMultiplier;
            DriftCharge += DriftChargeSpeed;
            if (DriftCharge > 75)
            {
                DriftCharge = 75;
                if (flag)
                {
                    ChargedUp.Play();
                    flag = false;
                }
            }
            if (DriftCharge > 0)
            {
                DriftSmoke.Play();
            }
            if (DriftCharge > 45)
            {
                // DriftSmoke.Stop();
                if (DriftDir < 0)
                {
                    RightDrift.Play();
                    LeftDrift.Stop();
                }
                if (DriftDir > 0)
                {
                    LeftDrift.Play();
                    RightDrift.Stop();
                }
            }
            else
            {
                RightDrift.Stop();
                LeftDrift.Stop();
            }
        }
        else
        {
            Among.localRotation = Quaternion.Euler(-90, 0, 0);
            MaxSpeedMultiplier = 1f;
            flag = true;
            DriftCharge = 0;
            RightDrift.Stop();
            LeftDrift.Stop();
            DriftSmoke.Stop();
        }

        float аааабббб = Mathf.Clamp(Mathf.Abs(Speed * 2 / DefaultMaxSpeed), 0, 1);
        animator.SetLayerWeight(0, 1-аааабббб);
        animator.SetLayerWeight(1, аааабббб);
        animator.SetBool("IsAirborne", IsAirborne);
        animator.SetBool("IsDrifting", IsDrifting);
        animator.SetFloat("DriftDir", DriftDir);
    }

    void Jump(float power)
    {
        rb.velocity += new Vector3(0, power, 0);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground") || other.gameObject.layer == LayerMask.NameToLayer("Booster")) IsAirborne = false;
        if (other.gameObject.CompareTag("Moving")) transform.parent = other.transform;
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground") || other.gameObject.layer == LayerMask.NameToLayer("Booster")) IsAirborne = false;
    }

    void OnTriggerExit(Collider other)
    {
        IsAirborne = true;
        transform.parent = null;
    }

    void OnCollisionStay(Collision collision)
    {
        if (IsAirborne) Speed = Speed / 4 - 2;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Booster")) StartCoroutine(ModifyMaxSpeed(collision.transform.GetComponent<BoosterInfo>().Boost, BoostFalloffTime));
        if (collision.gameObject.layer == LayerMask.NameToLayer("Jumper")) Jump(collision.transform.GetComponent<JumperInfo>().Power);
        if (collision.gameObject.layer == LayerMask.NameToLayer("Death")) checkpoints.RespawnPlayer();
    }
}