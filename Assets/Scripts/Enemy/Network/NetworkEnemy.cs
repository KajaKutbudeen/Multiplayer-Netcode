using UnityEngine;
using Unity.Netcode;
using UnityEngine.AI;
using System.Collections;
using NUnit.Framework;
using System.Collections.Generic;
using System.Runtime.CompilerServices;


public class NetworkEnemy : NetworkBehaviour
{
    [Header("Debug")]
    public float _playernetworkDistance = 0f;


    private float hitWait = .5f;
    public PatrolObjectspawn _PS;

    [Header("speed management")]
    [SerializeField] private float acce = 1f;
    [SerializeField] private float dece = 1f;
    [SerializeField] private float speedtest = 2f;


    [Header("Patrol Targets")]
    [SerializeField] private int CurrentPatrolObject;
    public List<Transform> PatrolObjects;
    [SerializeField] private float _Distance;

    [Header("References")]
    public Animator _anim;
    private NavMeshAgent agent;
    public DetectPlayertrigger _dpt;

    [Header("Distances")]
    private bool islook = false;
    [SerializeField] float playerdistance = 2f;
    [SerializeField] float _playerWalkdist = 2f;
    [SerializeField] float _playerRunDist = 2f;
    [SerializeField] float _playerFarDist = 5f;
    [SerializeField] float _distance = 0f;

    [Header("Check Player")]
    [SerializeField] private bool isplayernear = false;
    [SerializeField] private bool isplayerdetected = false;
    [SerializeField] public GameObject[] Players;
    [SerializeField] public GameObject Player;

    private float currentvelocity;
    private float smoothdamptime = .05f;

    public state currentstate;
    public enum state { Patrol,Combat,wait, Death}

    private void Awake()
    {
        _PS = GameObject.FindFirstObjectByType<PatrolObjectspawn>();             
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Initialize();
    }
    private void Initialize()
    {
        currentstate = state.Patrol;
        agent = GetComponent<NavMeshAgent>();
        if (_PS != null)
        {
            GetLocations();
        }
    }

    private void Start()
    {
        CurrentPatrolObject = 0;
        UpdateAnimServerRpc(1f);
    }
    private void Update()
    {
        if(!IsOwner || !Application.isFocused) return;

        if (currentstate == state.Patrol)
        {
            Patrolling();
        }
        if (currentstate == state.Combat)
        {
            fighting();
        }
        CheckDistance();
        Turnaround();
        CheckStatus();
    }
    private void FixedUpdate()
    {
       
    }

    private void CheckDistance()
    {
        if(Player == null) return;

        _playernetworkDistance = Vector3.Distance(transform.position, Player.transform.position);
    }
    public void CheckStatus()
    {
        if (Player == null) return;

        isplayerdetected = _dpt.playerdetect;
        isplayernear = _dpt.playernear;
        if (isplayerdetected)
        {
            currentstate = state.Combat;
        }
        if (isplayernear)
        {
            if (Vector3.Distance(transform.position, Player.transform.position) > 10)
            {
                isplayernear = false;
                _dpt.playernear = isplayernear;
                currentstate = state.Patrol;
            }
        }
    }

    public int temp = 1;
    public int prev = -1;
    public int next = 0;


    public void Patrolling()
    {
        if (PatrolObjects == null) return;
        if(agent.speed < .5f)
        {
            agent.speed = 1f;
            accespeed(1f);
        }
        agent.SetDestination(PatrolObjects[CurrentPatrolObject].transform.position);
        _Distance = Vector3.Distance(transform.position, PatrolObjects[CurrentPatrolObject].transform.position);

        if (_Distance < 2)
        {
            next += temp;
            prev += temp;
            CurrentPatrolObject = next;

            if (CurrentPatrolObject == PatrolObjects.Count)
            {
                temp = -1;
                next = prev + temp;
                prev = next + temp;
                CurrentPatrolObject = next;
                StartCoroutine(stand());
            }

            else if (CurrentPatrolObject < 0)
            {
                temp = 1;
                next = 1;
                prev = 0;
                CurrentPatrolObject = next;
                StartCoroutine(stand());
            }

        }
    }

    IEnumerator stand()
    {
        currentstate = state.wait;
        agent.ResetPath();
        agent.speed = 0f;
        UpdateAnimServerRpc(0f);
        yield return new WaitForSeconds(5f);
        UpdateAnimServerRpc(1f);
        agent.speed = 1f;
        currentstate = state.Patrol;
    }

    public void fighting()
    {
        if (Player == null)
        {
            currentstate = state.Patrol;           
            return;
        }
        if (islook == false)
        {
            transform.LookAt(Player.transform.position);
            islook = true;
        }
        agent.SetDestination(Player.transform.position);

        _distance = Vector3.Distance(transform.position, Player.transform.position);

        if (_distance < playerdistance)
        {
            /*    //  _anim.SetBool("Walk", false);
                //   _anim.SetBool("Run", false);
                //  agent.speed = 0;*/

            accespeed(0f);
            _anim.SetBool("Combat", true);


        }

        else if (_distance > playerdistance && _distance < _playerWalkdist)
        {
            /*    //     _anim.SetBool("Walk", true);
                //     _anim.SetBool("Run", false);
                //    agent.speed = 1f;*/

            _anim.SetBool("Combat", false);
            accespeed(1f);

        }

        else if (_distance > _playerWalkdist && _distance < _playerRunDist)
        {
            /*     //        _anim.SetBool("Walk", false);
                 //       _anim.SetBool("Run", true);
                 //        agent.speed = 2.5f; */

            _anim.SetBool("Combat", false);
            accespeed(2.5f);

        }
        else if (_distance > _playerFarDist)
        {
            /*   //  _anim.SetBool("Walk", false);
               //   _anim.SetBool("Run", false);
               //        agent.speed = 1f;*/

            _anim.SetBool("Combat", false);
            accespeed(1);

            currentstate = state.Patrol;
            islook = false;
        }

    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision != null)
        {
            if(collision.gameObject.CompareTag("Player"))
            {
                Debug.Log("Player");
            }
        }
    }
    public void accespeed(float value)
    {
        if (value > 0 && speedtest < 1f)
        {
            speedtest += Time.deltaTime * acce;
            speedtest = Mathf.Clamp(speedtest, 0, 1);
        }

        else if (value == 0 && speedtest > 0f)
        {
            speedtest -= Time.deltaTime * dece;

        }

        else if (value > 1 && speedtest < 2.5f)
        {
            speedtest += Time.deltaTime * acce;
            speedtest = Mathf.Clamp(speedtest, 1, 2.5f);
        }

        else if (value == 1 && speedtest > 1f)
        {
            speedtest -= Time.deltaTime * dece;
        }

        else if (value == 0)
        {
            speedtest = 0f;
        }

        agent.speed = speedtest;
        UpdateAnimServerRpc(agent.speed);
    }
    private void Turnaround()
    {
        if (agent.velocity.sqrMagnitude <= .1)
        {
            return;
        }

        var Targetrot = Mathf.Atan2(agent.velocity.x, agent.velocity.z) * Mathf.Rad2Deg;

        var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, Targetrot, ref currentvelocity, smoothdamptime);

        transform.rotation = Quaternion.Euler(0, angle, 0);
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateAnimServerRpc(float value)
    {
        _anim.SetFloat ("X", value);
    }

    [ClientRpc(RequireOwnership = false)]
    public void UpdateAnimClientRpc(float value)
    {
        _anim.SetFloat ("X", value);
    }

    public void GetLocations()
    {
        PatrolObjects = _PS.ReciveList(PatrolObjects, 0);
    }

    public void GotHit()
    {
        if (currentstate == state.Death) return;
        
            Debug.Log("Got Hit");
            _anim.SetBool("GetHit", true);
            StartCoroutine(HitStand());
        
    }

    IEnumerator HitStand()
    {
        accespeed(0f);
        currentstate = state.wait;
        yield return new WaitForSeconds(hitWait);
        Debug.Log("waited");
        _anim.SetBool("GetHit", false);
        currentstate = state.Combat;
    }
}


