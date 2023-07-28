using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderNorthModel : BaseModel
{
    
    // Start is called before the first frame update
    void Awake()
    {
        LoadAllNPCs();
        HP = 150.0f;
        MaxHP = HP;
        Faction = "North";
        Rank = "Leader";
        Damage = 8.0f;
        AttackDistanceThreshold = 1.5f;
        LeaderNorthView = GetComponent<LeaderNorthView>();
        Controller = new LeaderNorthController(this, LeaderNorthView);
        #region EventFSM
        var moveToPointBehaviour = new MoveToPoint(this);
        var attackBehaviour = new Attack(this);
        var escapeBehaviour = new Escape(this);
        var dieBehaviour = new Die(this);
        moveToPointBehaviour.AddObserverSetteableTarget(attackBehaviour);
        previousMovementBehaviour = moveToPointBehaviour;
        currentBehaviour = moveToPointBehaviour;

        var moveToPoint = new State<LeaderInputs>("MOVE TO POINT");
        var attack = new State<LeaderInputs>("ATTACK");
        var escape = new State<LeaderInputs>("ESCAPE");
        var die = new State<LeaderInputs>("DIE");

        StateConfigurer.Create(moveToPoint)
            .SetTransition(LeaderInputs.ATTACK, attack)
            .SetTransition(LeaderInputs.ESCAPE, escape)
            .SetTransition(LeaderInputs.DIE, die)
            .Done();

        StateConfigurer.Create(attack)
            .SetTransition(LeaderInputs.MOVE_TO_POINT, moveToPoint)
            .SetTransition(LeaderInputs.ESCAPE, escape)
            .SetTransition(LeaderInputs.DIE, die)
            .Done();

        StateConfigurer.Create(escape)
            .SetTransition(LeaderInputs.ATTACK, attack)
            .SetTransition(LeaderInputs.MOVE_TO_POINT, moveToPoint)
            .SetTransition(LeaderInputs.DIE, die)
            .Done();

        StateConfigurer.Create(die).Done();

        moveToPoint.OnEnter += x =>
        {
            CurrentLeaderState = LeaderInputs.MOVE_TO_POINT;
            //OnStateChanged((int)CurrentState);
            moveToPointBehaviour.index = 0;
            currentBehaviour = moveToPointBehaviour;
            Debug.Log("START MOVE TO POINT...");
        };

        moveToPoint.OnUpdate += () =>
        {
            currentBehaviour.ExecuteState();
        };

        moveToPoint.OnExit -= x =>
        {
            Debug.Log("END MOVE TO POINT...");
        };

        attack.OnEnter += x =>
        {
            CurrentLeaderState = LeaderInputs.ATTACK;
            currentBehaviour = attackBehaviour;
            Debug.Log("START ATTACK...");
        };

        attack.OnUpdate += () =>
        {
            currentBehaviour.ExecuteState();
        };

        attack.OnExit -= x =>
        {
            Debug.Log("END ATTACK...");
        };

        escape.OnEnter += x =>
        {
            CurrentLeaderState = LeaderInputs.ESCAPE;
            escapeBehaviour.index = 0;
            currentBehaviour = escapeBehaviour;
            GetNodeFinalFromCornersList();
            Debug.Log("START ESCAPE...");
        };

        escape.OnUpdate += () =>
        {
            currentBehaviour.ExecuteState();
            Debug.Log("EXECUTE ESCAPE...");
        };

        escape.OnExit -= x =>
        {
            Debug.Log("END ESCAPE...");
        };

        die.OnEnter += x =>
        {
            CurrentLeaderState = LeaderInputs.DIE;
            //OnStateChanged((int)CurrentState);
            currentBehaviour = dieBehaviour;
            Debug.Log("START DIE...");
        };

        die.OnUpdate += () =>
        {
            //currentBehaviour.ExecuteState();
        };

        die.OnExit -= x =>
        {
            Debug.Log("END DIE...");
        };

        moveToPoint.GetTransition(LeaderInputs.ATTACK).OnTransition += x =>
        {
            //CurrentState = LeaderInputs.ATTACK;
            Debug.Log("TRANSITION MOVE TO ATTACK...");
            //OnStateChanged((int)CurrentState);
        };

        attack.GetTransition(LeaderInputs.MOVE_TO_POINT).OnTransition += x =>
        {
            //CurrentState = LeaderInputs.MOVE_TO_POINT;
            Debug.Log("TRANSITION ATTACK TO MOVE TO POINT...");
            //OnStateChanged((int)CurrentState);
        };

        escape.GetTransition(LeaderInputs.MOVE_TO_POINT).OnTransition += x =>
        {
            //CurrentState = LeaderInputs.MOVE_TO_WAYPOINT;
            Debug.Log("TRANSITION ESCAPE TO MOVE TO POINT...");
            //OnStateChanged((int)LeaderInputs.MOVE_TO_WAYPOINT);
        };

        attack.GetTransition(LeaderInputs.ESCAPE).OnTransition += x =>
        {
            //CurrentState = LeaderInputs.WANDER;
            Debug.Log("TRANSITION ATTACK TO ESCAPE...");
            //OnStateChanged((int)LeaderInputs.WANDER);
        };

        moveToPoint.GetTransition(LeaderInputs.DIE).OnTransition += x =>
        {
            //CurrentState = LeaderInputs.DIE;
            Debug.Log("TRANSITION MOVE TO POINT TO DIE...");
            //OnStateChanged((int)LeaderInputs.DIE);
        };

        moveToPoint.GetTransition(LeaderInputs.ESCAPE).OnTransition += x =>
        {
            //CurrentState = LeaderInputs.WANDER;
            Debug.Log("TRANSITION MOVE TO ESCAPE...");
            //OnStateChanged((int)LeaderInputs.WANDER);
        };

        FSMLeaders = new EventFSM<LeaderInputs>(moveToPoint);
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        Controller.OnUpdate();
    }

}
