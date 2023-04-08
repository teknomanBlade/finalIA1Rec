using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseModel : MonoBehaviour, IDamageTargetObservable
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
    protected float _hp;
    public float HP
    {
        get { return _hp; }
        set { _hp = value; }
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
    // Start is called before the first frame update
    void Start()
    {

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
        //_mFSMLeaders.Update();
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
            HP -= damage;
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
}