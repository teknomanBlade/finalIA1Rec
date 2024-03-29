using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BaseModel : MonoBehaviour, /*IObstacleBetweenObserver,*/ IAttackTargetObserver
{
    protected IController _controller;
    public IController Controller
    {
        get { return _controller; }
        set { _controller = value; }
    }
    private FollowerSouthView _fsv;
    public FollowerSouthView FollowerSouthView
    {
        get { return _fsv; }
        set { _fsv = value; }
    }
    private FollowerNorthView _fnv;
    public FollowerNorthView FollowerNorthView
    {
        get { return _fnv; }
        set { _fnv = value; }
    }
    private LeaderSouthView _lsv;
    public LeaderSouthView LeaderSouthView
    {
        get { return _lsv; }
        set { _lsv = value; }
    }
    private LeaderNorthView _lnv;
    public LeaderNorthView LeaderNorthView
    {
        get { return _lnv; }
        set { _lnv = value; }
    }
    private float _tick;
    private float _attackRate;
    private float _count;
    private float _distanceNodeThreshold;

    public float DistanceNodeThreshold
    {
        get { return _distanceNodeThreshold; }
        set { _distanceNodeThreshold = value; }
    }
    private float _distanceNodeFinalThreshold;

    public float DistanceNodeFinalThreshold
    {
        get { return _distanceNodeFinalThreshold; }
        set { _distanceNodeFinalThreshold = value; }
    }
    [SerializeField]
    protected float _hp;
    public float HP
    {
        get { return _hp; }
        set { _hp = value; }
    }
    protected float _maxHp;
    public float MaxHP
    {
        get { return _maxHp; }
        set { _maxHp = value; }
    }
    protected float _damage;
    public float Damage
    {
        get { return _damage; }
        set { _damage = value; }
    }
    protected float _speed;
    public float Speed
    {
        get { return _speed; }
        set { _speed = value; }
    }
    protected string _faction;
    public string Faction
    {
        get { return _faction; }
        set { _faction = value; }
    }
    protected string _rank;
    public string Rank
    {
        get { return _rank; }
        set { _rank = value; }
    }
    public LayerMask ObstaclesLayer;
    public LayerMask NPCSLayer;
    public LayerMask FloorMask;
    public Vector3 TargetPosition;
    public Node currentNode;
    public Node finalNode;
    
    
    private LeaderInputs _currentLeaderState;
    public LeaderInputs CurrentLeaderState
    {
        get { return _currentLeaderState; }
        set { _currentLeaderState = value; }
    }
    private NPCInputs _currentNPCState;
    public NPCInputs CurrentNPCState
    {
        get { return _currentNPCState; }
        set { _currentNPCState = value; }
    }
    protected IBehaviour currentBehaviour;
    protected IBehaviour previousMovementBehaviour;
    public float arriveRadius;
    public float maxSpeed;
    public float maxForce;
    public Vector3 _velocity;
    public enum NPCInputs { IDLE,ATTACK, FOLLOW, AVOID_OBSTACLES , DIE, ESCAPE }
    protected EventFSM<NPCInputs> _mFSM_NPCs;
    public EventFSM<NPCInputs> FSM_NPCs
    {
        get { return _mFSM_NPCs; }
        set { _mFSM_NPCs = value; }
    }
    public enum LeaderInputs { ATTACK, MOVE_TO_POINT, DIE, ESCAPE }
    protected EventFSM<LeaderInputs> _mFSMLeaders;
    public EventFSM<LeaderInputs> FSMLeaders
    {
        get { return _mFSMLeaders; }
        set { _mFSMLeaders = value; }
    }
    [SerializeField]
    protected float _distanceToTarget;
    public float DistanceToTarget
    {
        get { return _distanceToTarget; }
        set { _distanceToTarget = value; }
    }
    protected float _angleToTarget;
    public float AngleToTarget
    {
        get { return _angleToTarget; }
        set { _angleToTarget = value; }
    }
    protected float _distanceThreshold;
    public float DistanceThreshold
    {
        get { return _distanceThreshold; }
        set { _distanceThreshold = value; }
    }
    [SerializeField]
    protected float _attackDistanceThreshold;
    public float AttackDistanceThreshold
    {
        get { return _attackDistanceThreshold; }
        set { _attackDistanceThreshold = value; }
    }
    protected float _angleThreshold;
    public float AngleThreshold
    {
        get { return _angleThreshold; }
        set { _angleThreshold = value; }
    }

    protected Vector3 _dirToTarget;
    public Vector3 DirToTarget
    {
        get { return _dirToTarget; }
        set { _dirToTarget = value; }
    }
    public bool ObstaclesBetween;
    [SerializeField]
    private BaseModel[] _npcs;
    public BaseModel[] NPCs 
    { 
        get 
        {
            
            return _npcs; 
        } 
    }
    // Start is called before the first frame update
    void Start()
    {
        AngleThreshold = 60f;
        DistanceThreshold = 0.9f;
        DistanceNodeThreshold = 0.4f;
        DistanceNodeFinalThreshold = 0.4f;
        GetNodeByLesserDistance();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void UpdateFSM_NPCs() 
    {
        StartCoroutine(FSMWaitForInstance());
    }
    IEnumerator FSMWaitForInstance() 
    {
        yield return new WaitUntil(() => _mFSM_NPCs != null);
        _mFSM_NPCs.Update();
    }
    public void UpdateFSMLeaders()
    {
        _mFSMLeaders.Update();
    }

    public void TakeDamage(float damage)
    {
        Debug.Log("TAKING DAMAGE: " + damage + " - " + name);
        if (HP <= 0)
        {
            SetFSMInput(
                 Rank,
                 () => { _mFSM_NPCs.SendInput(NPCInputs.DIE); }
                ,() => { _mFSMLeaders.SendInput(LeaderInputs.DIE); } );
            return;
        }

        if (HP < 15)
        {
            Debug.Log("ESCAPE LIFE: " + HP);
            SetFSMInput(
                 Rank,
                 () => { _mFSM_NPCs.SendInput(NPCInputs.ESCAPE); }
                , () => { _mFSMLeaders.SendInput(LeaderInputs.ESCAPE); });
        }
        else 
        {
            Debug.Log("LIFE: " + HP);
            HP -= Mathf.Clamp(HP - damage, 0.0f, MaxHP);
        }

    }

    public void SetFSMInput(string Rank, Action followerAction = null, Action leaderAction = null)
    {
        if (Rank.Equals("Follower"))
        {
            followerAction?.Invoke();
        }
        else
        {
            leaderAction?.Invoke();
        }
    }

    public void GetPositionInScene()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitData;
        if (Physics.Raycast(ray, out hitData, 1000, FloorMask))
        {
            TargetPosition = hitData.point;
        }
    }

    public List<Node> GetPath(Node initial, Node final) 
    {
        return GameManager.instance.thetaStar.GetPath(initial, final);
    }
    public bool InSight()
    {
        return GameManager.instance.InSight(transform.position, TargetPosition);
    }
  
    public void GetNodeByLesserDistance()
    {
        StartCoroutine(NodeByLesserDistance());
    }

    IEnumerator NodeByLesserDistance() 
    {
        yield return new WaitUntil(() => GameManager.instance != null);
        GameManager.instance.AllNodes.ForEach(x => {
            //Debug.Log("DISTANCE TO NODE " + x.name + ": " + Vector3.Distance(transform.position, x.transform.position)); 
            if (!x.isBlocked && Vector3.Distance(transform.position, x.transform.position) <= DistanceNodeThreshold)
            {
                //Debug.Log("ENTRA EN IF DISTANCIA?");
                currentNode = x;
                //Debug.Log("CURRENT NODE: " + currentNode.name);
            }
        });
    }

    public void GetNodeFinalByLesserDistance(Vector3 position)
    {
        if (position.Equals(Vector3.zero)) return;

        GameManager.instance.AllNodes.ForEach(x => {
            //Debug.Log("DISTANCE TO NODE " + x.name + ": " + Vector3.Distance(transform.position, x.transform.position)); 
            if (!x.isBlocked && Vector3.Distance(position, x.transform.position) <= DistanceNodeFinalThreshold)
            {
                //Debug.Log("ENTRA EN IF DISTANCIA?");
                finalNode = x;
                //Debug.Log("FINAL NODE: " + finalNode.name);
            }
        });
    }

    public void GetNodeFinalFromCornersList() 
    {
        var q = GameManager.instance.CornerNodes;
        var rnd = new System.Random();
        finalNode = q.OrderBy(x => rnd.Next()).Take(1).FirstOrDefault();
        TargetPosition = finalNode.transform.position;
    }

    public void OnNotifyAttackTarget(string message)
    {
        if (message.Equals("IsAttacking")) 
        {
            Debug.Log("ATTACKING...");
            if (Rank.Equals("Leader"))
            {
                Debug.Log("LEADER ATTACK...");
                FSMLeaders.SendInput(LeaderInputs.ATTACK);
            }
            else 
            { 
                Debug.Log("FOLLOWER ATTACK...");
                FSM_NPCs.SendInput(NPCInputs.ATTACK);
            }
        }
    }
    public void CheckBounds()
    {
        Vector3 newPosition = transform.position;

        if (transform.position.z > 12) newPosition.z = transform.position.z - 0.5f;
        if (transform.position.z < -0.2) newPosition.z = transform.position.z + 0.5f;
        if (transform.position.x > 12) newPosition.x = transform.position.x - 0.5f;
        if (transform.position.x < -12) newPosition.x = transform.position.x + 0.5f;

        transform.position = newPosition;
    }
    /*public void OnNotifyDamageTarget(string message, float damage)
    {
        if (message.Equals("IsDamaging")) 
        {
            if (Faction.Equals("North"))
            {
                StartCoroutine(TakeDamageCoroutine(damage));
            }
            else 
            {
                StartCoroutine(TakeDamageCoroutine(damage));
            }
            
        }
    }

    IEnumerator TakeDamageCoroutine(float damage) 
    {
        Debug.Log("ANTES DE DA�O...");
        yield return new WaitForSeconds(1.5f);
        TakeDamage(damage);
        Debug.Log("DESPUES DE DA�O... " + damage);
    }*/

    protected void LoadAllNPCs() 
    {
        _npcs = FindObjectsOfType<BaseModel>();
        //Debug.Log("NPCs List: " + _npcs.Length);
    }
   

   
}
