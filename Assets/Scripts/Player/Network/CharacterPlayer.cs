using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

namespace HelloWorld
{

    public class CharacterPlayer : NetworkBehaviour
    {

        [Header("Current Debug")]
        public NetworkCharacterSwitch _characterswitch;
        public bool isgrd = false;
        [SerializeField] private float AirControlspeed;
        public state currentState;


        [Header("Jump")]
        public NetworkVariable<bool> IsGrounded = new NetworkVariable<bool>(
            default,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner
            
            );
        public NetworkVariable<bool> isGroundClient = new NetworkVariable<bool>(
             default,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner

            );

        [Header("References")]
        private Rigidbody _rb;
        public Animator _anim;

        [Header("Inputactions")]
        InputSystem_Actions _inputActions;
        InputAction _move;
        InputAction _jump;
        private InputAction _attack;

        [Header("Parameters")]
        private Vector2 movevalues;
        private Vector3 MoveDir;
        [SerializeField] private bool _moveing;
      
        [Header("Camera")]
        private Vector3 camforward;
        private Vector3 camright;
        public Camera cam;

        [Header("Velos")]
       [SerializeField] private float velocitymag;
       [SerializeField] private float Desiredvelocity = .5f;

        [Header("Isgrounded")]
        public LayerMask ground;
        public Transform groundcheck;


        [Header("Damp")]
        private float smoothdamptime = .05f;
        private float currentvelocity;

        [Header("Speed Management")]
        [SerializeField] private float speed = 2f;
        [SerializeField] private float accleration = 2f;
        [SerializeField] private float SpeedA = 2f;
        [SerializeField] private float deaccleartion = 2f;
        [SerializeField] private float SpeedD = 2f;

        public enum state { idle, walk, run, jump, Combat };

        RaycastHit hit;

        private void Awake()
        {
            _characterswitch = GameObject.Find("CharacterSelect").GetComponent<NetworkCharacterSwitch>();
        }
        public override void OnNetworkSpawn()
        {
        
            base.OnNetworkSpawn();
            Initialize();           
        }

        private void Initialize()
        {
            Application.targetFrameRate = 60;
            Cursor.lockState = CursorLockMode.Locked;
            currentState = state.idle;                    
        }
        private void OnEnable()
        {
            _inputActions = new InputSystem_Actions();
            _inputActions.Enable();

            _move = _inputActions.Player.Move;
            _jump = _inputActions.Player.Jump;
            _attack = _inputActions.Player.Attack;

            _move.Enable();
            _jump.performed += PlayerJump;
            _jump.canceled += PlayerOffJump;
            _jump.Enable();

            _attack.Enable();
            _attack.performed += PlayerAttack;
            

        }

        private void Start()
        {           
            _rb = GetComponent<Rigidbody>();
            cam = Camera.main;
            _moveing = true;
        }

        private void Update()
        {
            if (!IsOwner || !Application.isFocused) return;
            // Read input values
            movevalues = _move.ReadValue<Vector2>();
            //Check if this work for client
            if (!isgrd)//!IsGrounded.Value)
            {
                _anim.SetBool("InAir",true);
            }
        }
              
        private void FixedUpdate()
        {
            if (!IsOwner || !Application.isFocused) return;

            if (_moveing)
            {
                Movement();
            }
            Turnaround();
            //   UpdateGroundServerRpc(groundcheck,.05f,ground);

            // Update animation on the server
            UpdateAnimationServerRpc(velocitymag);

        }

        public void PlayerJump(InputAction.CallbackContext context)
        {
            if (!isgrd) return;// if (!IsGrounded.Value) return;          
            _anim.SetBool("Jump",isgrd);
            
        }
        public void PlayerOffJump(InputAction.CallbackContext context)
        {
            _anim.SetBool("Jump", false);
        }

        private void OnCollisionStay(Collision collision)
        {
            if(IsOwner)
            {
                if(collision.gameObject.CompareTag("Ground"))
                {
                   HandleGroundServerRpc(true);
                }              
            }
        }

        public void PlayerAttack(InputAction.CallbackContext context)
        {
            speed = 0;
            _moveing = false;
            currentState = state.Combat;
            _anim.SetTrigger("Combat");
        }

        public void PlayerAttackOff()
        {
            currentState = state.idle;
            speed = SpeedD;
            _moveing = true;
        }

        private void OnCollisionExit(Collision collision)
        {
            if (IsOwner)
            {
                if (collision.gameObject.CompareTag("Ground"))
                {
                    HandleGroundServerRpc(false);
                }
                
            }
        }
        [ServerRpc]
        public void HandleGroundServerRpc(bool value)
        {           
           // IsGrounded.Value = value;
           isgrd = value;
            _anim.SetBool("InAir", !isgrd);
           HandleGroundClientRpc(value);
        }

        [ClientRpc]
        public void HandleGroundClientRpc(bool value)
        {
            // isGroundClient.Value = value;
            isgrd = value;
            _anim.SetBool("InAir", !isgrd);
        }

        [ServerRpc]
        public void UpdateAnimationServerRpc(float velocity)
        {
            velocitymag = velocity; // Update the server's velocity magnitude
            _anim.SetFloat("X", velocitymag);
            UpdateAnimationClientRpc(velocitymag); // Notify clients
        }

        [ClientRpc]
        public void UpdateAnimationClientRpc(float velocity)
        {
            _anim.SetFloat("X", velocity);
        }


        public void Movement()
        {
            camforward = cam.transform.forward;
            camforward.y = 0f;
            camforward = camforward.normalized;


            camright = cam.transform.right;
            camright.y = 0f;
            camright = camright.normalized;
            MoveDir = (movevalues.x * camright + movevalues.y * camforward) * speed;
            MoveDir.y = _rb.linearVelocity.y;
            if (movevalues.sqrMagnitude > 0 && velocitymag < 1)
            {
                velocitymag += Time.deltaTime * accleration;

            }
            else if (movevalues.sqrMagnitude == 0 && velocitymag > 0f)
            {
                velocitymag -= Time.deltaTime * deaccleartion;

            }
            else if (velocitymag > 1 && Input.GetKey(KeyCode.LeftShift))
            {
                // currentState = state.run;
                velocitymag += Time.deltaTime * accleration;
                speed = SpeedA;
                velocitymag = Mathf.Clamp(velocitymag, 1f, 3f);
            }
            else if (!Input.GetKey(KeyCode.LeftShift) && velocitymag > 1.1f)
            {
                velocitymag -= Time.deltaTime * deaccleartion;
                speed = SpeedD;
            }

            else if (movevalues.sqrMagnitude == 0)
            {
                velocitymag = 0f;
            }

            //  MoveDir = new Vector3(movevalues.x * speed, _rb.linearVelocity.y, movevalues.y * speed);

            if (velocitymag > Desiredvelocity)
            {
                _rb.linearVelocity = MoveDir;
            }
        }


        private void Turnaround()
        {
            if (movevalues.sqrMagnitude <= .1)
            {
                return;
            }

            var Targetrot = Mathf.Atan2(MoveDir.x, MoveDir.z) * Mathf.Rad2Deg;

            var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, Targetrot, ref currentvelocity, smoothdamptime);

            transform.rotation = Quaternion.Euler(0, angle, 0);
        }

        private void OnDisable()
        {
            _inputActions.Disable();
            _move.Disable();
            _jump.performed -= PlayerJump;
            _jump.canceled -= PlayerOffJump;
            _jump.Disable();

            _attack.performed -= PlayerAttack;
            _attack.Disable();
        }
    }

  /*  struct PlayerData : INetworkSerializable
    {
        public LayerMask grdcheck;
        public float valeu;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref grdcheck);
            serializer.SerializeValue(ref valeu);
        }

    }*/
}
