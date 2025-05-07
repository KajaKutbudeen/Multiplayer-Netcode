using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{

    [Header("Debug")]
    [SerializeField] float hitWait = 1f;
    [SerializeField] private float acce = 1f;
    [SerializeField] private float dece = 1f;
    [SerializeField] private float speedtest = 2f;
   
 /*
  
  */

    [Header("Check Player")]
    [SerializeField] private bool isplayernear = false;
    [SerializeField] private bool isplayerdetected = false;


    [Header("Distances")]
    private bool islook = false;
    [SerializeField] float playerdistance = 2f;
    [SerializeField] float _playerWalkdist = 2f;
    [SerializeField] float _playerRunDist = 2f;
    [SerializeField] float _playerFarDist = 5f;
    [SerializeField] float _distance = 0f;

    [Header("Patrol Targets")]
    [SerializeField] private int CurrentPatrolObject;
    public GameObject[] PatrolObjects;
    [SerializeField] private float _Distance;


    [Header("Player Target")]
    public GameObject Player;

    [Header("References")]
    public Animator _anim;
    public GameObject _Detectobj;
    public GameObject _End;
    public LayerMask playercheck;
    public DetectPlayertrigger _detectPlayertrigger;
    NavMeshAgent agent;

    private float currentvelocity;
    private float smoothdamptime = .05f;

    public enum state {wait,Patrol, Combat, Death};

    public state currentstate = state.Patrol;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        CurrentPatrolObject = 0;
    }

    private void Update()
    {
        if (currentstate == state.Death)
        {
            agent.ResetPath();
        }

          CheckStatus();
        if (currentstate == state.Patrol)
        {
            Patrolling();
    //        Agentvelo = agent.velocity;
    //        agentmag = agent.velocity.magnitude;        
        }

        if (currentstate == state.Combat)
        {
            fighting();
        }
        UpdateAnims();
        Turnaround();
        
    }

    RaycastHit hit;

    public void CheckStatus()
    {


        isplayerdetected = _detectPlayertrigger.playerdetect;
        isplayernear = _detectPlayertrigger.playernear;
        if (isplayerdetected)
        {
            currentstate = state.Combat;
        }
        if (isplayernear)
        {
            if(Vector3.Distance(transform.position,Player.transform.position) > 10)
            {
                isplayernear = false;
                _detectPlayertrigger.playernear = isplayernear;
                
            }
        }
    }
    /*   public void Detection()
       {
           Vector3 endpos = transform.position+ (Vector3.forward.normalized * rayDistance);
           if (Physics.Raycast(transform.position + cubesize,endpos + cubesize,out hit,rayDistance,playercheck))
           {
             //  Debug.Log("hit");
               if (hit.collider != null)
               {
                //   Debug.Log(hit.collider.name);
               }
           }
       }*/

    

    public void fighting()
    {
        if (Player == null) return;
        if (islook == false)
        {
            transform.LookAt(Player.transform.position);
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

            _anim.SetBool("Combat",false);
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

    private void UpdateAnims()
    {
        if (currentstate == state.wait) return;

        _anim.SetFloat("X",agent.speed);
     /*   if (agent.velocity.magnitude > desiredvelo)
        {
            _anim.SetBool("Walk", true);
        }
        else
        {
            _anim.SetBool("Walk", false);
        }*/
    }

   public int temp = 1;
    public int prev = -1;
    public int next = 0;

    private void Patrolling()
    {

        if (PatrolObjects == null) return;

        agent.SetDestination(PatrolObjects[CurrentPatrolObject].transform.position);

        _Distance = Vector3.Distance(transform.position, PatrolObjects[CurrentPatrolObject].transform.position);

        if(_Distance < 2)
        {
            next += temp;
            prev += temp;
            CurrentPatrolObject = next;

            if(CurrentPatrolObject == PatrolObjects.Length)
            {
                temp = -1;
                next = prev + temp;
                prev = next + temp;
                CurrentPatrolObject = next;
                StartCoroutine(stand());
            }

            else if(CurrentPatrolObject < 0)
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
        _anim.SetFloat("X", 0);
        yield return new WaitForSeconds(5f);
        //     agent.speed = 1f;
        accespeed(1f);
        currentstate = state.Patrol;
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
    }

    public void GotHit()
    {
        if (currentstate == state.Death) return;                  
        
        Debug.Log("Got Hit");
        _anim.SetBool("GetHit", true);
      StartCoroutine(HitStand());      
    }
    public void Death()
    {
        currentstate = state.Death;
        _anim.SetTrigger("Death");
        StartCoroutine(DeathWait());
       
    }

    IEnumerator DeathWait()
    {
        yield return new WaitForSeconds(2f);
        GetComponent<Enemy>().enabled = false;
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

