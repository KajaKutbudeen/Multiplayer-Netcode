using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

    public class SimpleMovement : MonoBehaviour
    {
    [Header("Current Debug")]
    [SerializeField] private float AirControlspeed;
    [SerializeField] private bool Inair = false;
    public state currentState;
   


    [Header("Speed Management")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private float accleration = 2f;
    [SerializeField] private float SpeedA = 2f;
    [SerializeField] private float deaccleartion = 2f;
    [SerializeField] private float SpeedD = 2f;
    private float DesiredVelocity = .5f;
    [SerializeField]
    float RayDistance = .1f;

    [Header("References")]
    private Rigidbody rb;
    private InputSystem_Actions playercontrols;
    [SerializeField] private Animator animator;
    [SerializeField] private SimplePlayerAnimationEvents _PAE;

    private Vector3 camforward;
    private Vector3 camright;
    public Camera cam;

    [Header("Jump")]
    [SerializeField] private int Walkjump;
    [SerializeField] private int runjump;
   // private bool _Standjumpanim = false;

    [Header("Inputactions")]
    private InputAction _move;
    private InputAction _jump;
    private InputAction _sprint;
    private InputAction _attack;

    [Header("Parameters")]
    private Vector2 movevalues;
    private Vector3 movedir;
    private float smoothdamptime = .05f;
    private float currentvelocity;

    [Header("Isgrounded")]
    public LayerMask ground;
    public Transform groundcheck;
    [SerializeField]
    private bool isgrounded;

    [Header("States")]
    public bool _moveing = true;


    [Header("Debug")]
  //  [SerializeField] private bool jumprunanim = false;
    [SerializeField] private float movevaluedebug;
    [SerializeField] private float velocitymag;
   // [SerializeField] private bool walk = false;

    public enum state { idle, walk, run, jump, Combat };

    RaycastHit hit;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        Cursor.lockState = CursorLockMode.Locked;
        playercontrols = new InputSystem_Actions();
    }
    private void Start()
    {
        currentState = state.idle;
        rb = GetComponent<Rigidbody>();
        SpeedD = speed;
    }

    private void OnEnable()
    {
        playercontrols.Enable();
        _move = playercontrols.Player.Move;
        _jump = playercontrols.Player.Jump;
        _sprint = playercontrols.Player.Sprint;
        _attack = playercontrols.Player.Attack;

        _move.Enable();
        _jump.performed += PlayerJump;
        _jump.canceled += PlayerOffJump;
        _jump.Enable();

        _attack.Enable();
        _attack.performed += PlayerAttack;
     //   _attack.canceled += PlayerAttack;

      //  _sprint.performed += PlayerSprint;
     //   _sprint.Enable();

    }
    private void Update()
    {
        if (isgrounded)
        {
            UpdateAnimations();      
        }
      //  movevaluedebug = movevalues.sqrMagnitude;

        RaycastSurface();

    }

    private void RaycastSurface()
    {       

        if (Physics.Raycast(groundcheck.position, transform.TransformDirection(Vector3.down),out hit ,RayDistance, ground) && Inair)
        {
            speed = SpeedD;
           Inair = false;           
        }
        else if(!isgrounded)
        {
            Inair = true;
        }
            animator.SetBool("InAir", Inair);
    }

    private void FixedUpdate()
    {
        if (_moveing && currentState!= state.Combat)
        {
            movement();
        }
        Turnaround();
        isgrounded = Physics.Raycast(groundcheck.position, Vector3.down, 0.05f, ground);        
    }
    private void movement()
    {
        movevalues = _move.ReadValue<Vector2>();

        camforward = cam.transform.forward;
        camforward.y = 0f;
        camforward = camforward.normalized;


        camright = cam.transform.right;
        camright.y = 0f;
        camright = camright.normalized;

        movedir = (movevalues.x * camright + movevalues.y * camforward) * speed;
        movedir.y = rb.linearVelocity.y;
       
       
        if (velocitymag > DesiredVelocity)
        {
            rb.linearVelocity = movedir;
        }       

    } 
    private void UpdateAnimations()
    {
        if (movevalues.sqrMagnitude > 0 && velocitymag < 1)
        {
            speed = SpeedD;
            velocitymag += Time.deltaTime * accleration;
            currentState = state.walk;
            //walk = true;
        }
        else if (movevalues.sqrMagnitude == 0 && velocitymag > 0f)
        {
            velocitymag -= Time.deltaTime * deaccleartion;
            currentState = state.walk;

        }
        else if (movevalues.sqrMagnitude > 0 && Input.GetKey(KeyCode.LeftShift))
        {
            currentState = state.run;
            velocitymag += Time.deltaTime * accleration;
            speed = SpeedA;
            velocitymag = Mathf.Clamp(velocitymag, 0, 3);


        }
        else if (movevalues.sqrMagnitude > 0 && !Input.GetKey(KeyCode.LeftShift) && velocitymag > 1.1f)
        {
            velocitymag -= Time.deltaTime * deaccleartion;
            speed = SpeedD;
        }
        else if (movevalues.sqrMagnitude == 0)
        {
            velocitymag = 0f;
            currentState = state.idle;
            //  walk = false;
        }

        // animator.SetBool("Walk", walk);



        animator.SetFloat("X", velocitymag);
    }  
   
    private void Turnaround()
    {
        if (movevalues.sqrMagnitude <= .1)
        {
            return;
        }

        var Targetrot = Mathf.Atan2(movedir.x, movedir.z) * Mathf.Rad2Deg;

        var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, Targetrot, ref currentvelocity, smoothdamptime);

        transform.rotation = Quaternion.Euler(0, angle, 0);
    }

    public void PlayerJump(InputAction.CallbackContext context)
    {
        if (!isgrounded) return;
        Inair = true;      
        animator.SetBool("Jump", isgrounded);
    }

    public void PlayerAttack(InputAction.CallbackContext context)
    {
        speed = 0;
        _moveing = false;
        currentState = state.Combat;
        animator.SetTrigger("Combat");
    }

    public void PlayerAttackOff()
    {
        currentState = state.idle;
        _moveing = true;
    }

    public void PlayerOffJump(InputAction.CallbackContext context)
    {
        animator.SetBool("Jump", false);
    }
    private void OnDisable()
    {
        playercontrols.Disable();
        _move.Disable();
        _jump.performed -= PlayerJump;
        _jump.canceled -= PlayerOffJump;
        _jump.Disable();

        _attack.Disable();
        _attack.performed -= PlayerAttack;
     //   _attack.canceled -= PlayerAttackOff;
       // _sprint.performed -= PlayerSprint;
     //   _sprint.Disable();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(groundcheck.transform.position, Vector3.down * RayDistance);
    }

}

