using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BaseModel : MonoBehaviour, IDamageTargetObservable, IObstacleBetweenObserver, IAttackTargetObserver
{
    protected List<IDamageTargetObserver> _myObserversDamageTarget = new List<IDamageTargetObserver>();
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
    public float offsetY;
    public Vector3 _velocity;
    public enum NPCInputs { ATTACK, FOLLOW, DIE, ESCAPE }
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
    public BaseModel[] npcs;
    // Start is called before the first frame update
    void Start()
    {
        npcs = FindObjectsOfType<BaseModel>();
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
        //_mFSM_NPCs.Update();
    }

    public void UpdateFSMLeaders()
    {
        _mFSMLeaders.Update();
    }

    public void TakeDamage(float damage)
    {
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
    public void AddObserverDamageTarget(IDamageTargetObserver obs)
    {
        _myObserversDamageTarget.Add(obs);
    }

    public void RemoveObserverDamageTarget(IDamageTargetObserver obs)
    {
        if (_myObserversDamageTarget.Contains(obs))
        {
            _myObserversDamageTarget.Remove(obs);
        }
    }

    public void TriggerDamageTarget(string message, float damage)
    {
        _myObserversDamageTarget.ForEach(x => x.OnNotifyDamageTarget(message, damage));
    }

    public void OnNotifyObstacleBetween(string message)
    {

    }

    public void OnNotifyAttackTarget(string message)
    {
        if (message.Equals("IsAttacking")) 
        {
            if (Rank.Equals("Leader"))
                FSMLeaders.SendInput(LeaderInputs.ATTACK);
            else
                FSM_NPCs.SendInput(NPCInputs.ATTACK);
        }
    }
}
