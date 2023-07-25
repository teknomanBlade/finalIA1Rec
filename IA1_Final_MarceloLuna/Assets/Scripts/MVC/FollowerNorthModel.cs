using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FollowerNorthModel : BaseModel
{
    public BaseModel leaderNorth;
    // Start is called before the first frame update
    void Awake()
    {
        LoadAllNPCs();
        HP = 100.0f;
        MaxHP = HP;
        Faction = "North";
        Rank = "Follower";
        Damage = 5.0f;
        avoidWeight = 3.5f;
        FollowerNorthView = GetComponent<FollowerNorthView>();
        Controller = new FollowerNorthController(this, FollowerNorthView);
        #region EventFSM
        var idleBehaviour = new Idle(this, leaderNorth);
        var followBehaviour = new Follow(this, leaderNorth);
        var avoidObstaclesBehaviour = new AvoidObstacles(this, leaderNorth);
        followBehaviour.AddObserverObstacleBetween(avoidObstaclesBehaviour);
        var attackBehaviour = new Attack(this);
        var escapeBehaviour = new Escape(this);
        var dieBehaviour = new Die(this);

        previousMovementBehaviour = idleBehaviour;
        currentBehaviour = idleBehaviour;

        var idle = new State<NPCInputs>("IDLE");
        var follow = new State<NPCInputs>("FOLLOW");
        var avoidObstacles = new State<NPCInputs>("AVOID_OBSTACLES");
        var attack = new State<NPCInputs>("ATTACK");
        var escape = new State<NPCInputs>("ESCAPE");
        var die = new State<NPCInputs>("DIE");

        StateConfigurer.Create(idle)
            .SetTransition(NPCInputs.FOLLOW, follow)
            .SetTransition(NPCInputs.AVOID_OBSTACLES, avoidObstacles)
            .SetTransition(NPCInputs.ATTACK, attack)
            .SetTransition(NPCInputs.ESCAPE, escape)
            .SetTransition(NPCInputs.DIE, die)
            .Done();

        StateConfigurer.Create(follow)
            .SetTransition(NPCInputs.AVOID_OBSTACLES, avoidObstacles)
            .SetTransition(NPCInputs.IDLE, idle)
            .SetTransition(NPCInputs.ATTACK, attack)
            .SetTransition(NPCInputs.ESCAPE, escape)
            .SetTransition(NPCInputs.DIE, die)
            .Done();

        StateConfigurer.Create(avoidObstacles)
            .SetTransition(NPCInputs.FOLLOW, follow)
            .SetTransition(NPCInputs.IDLE, idle)
            .SetTransition(NPCInputs.ATTACK, attack)
            .SetTransition(NPCInputs.ESCAPE, escape)
            .SetTransition(NPCInputs.DIE, die)
            .Done();

        StateConfigurer.Create(attack)
            .SetTransition(NPCInputs.FOLLOW, follow)
            .SetTransition(NPCInputs.AVOID_OBSTACLES, avoidObstacles)
            .SetTransition(NPCInputs.IDLE, idle)
            .SetTransition(NPCInputs.ESCAPE, escape)
            .SetTransition(NPCInputs.DIE, die)
            .Done();

        StateConfigurer.Create(escape)
            .SetTransition(NPCInputs.ATTACK, attack)
            .SetTransition(NPCInputs.FOLLOW, follow)
            .SetTransition(NPCInputs.AVOID_OBSTACLES, avoidObstacles)
            .SetTransition(NPCInputs.IDLE, idle)
            .SetTransition(NPCInputs.DIE, die)
            .Done();

        StateConfigurer.Create(die).Done();

        idle.OnEnter += x =>
        {
            //CurrentState = NPCInputs.IDLE;
            //OnStateChanged((int)CurrentState);
            CurrentNPCState = NPCInputs.IDLE;
            currentBehaviour = previousMovementBehaviour;
            Debug.Log("START IDLE...");
        };

        idle.OnUpdate += () =>
        {
            currentBehaviour.ExecuteState();
        };

        idle.OnExit -= x =>
        {
            Debug.Log("END IDLE...");
        };

        follow.OnEnter += x =>
        {
            //CurrentState = NPCInputs.IDLE;
            //OnStateChanged((int)CurrentState);
            CurrentNPCState = NPCInputs.FOLLOW;
            currentBehaviour = followBehaviour;
            Debug.Log("START FOLLOW...");
        };

        follow.OnUpdate += () =>
        {
            currentBehaviour.ExecuteState();
        };

        follow.OnExit -= x =>
        {
            Debug.Log("END FOLLOW...");
        };

        avoidObstacles.OnEnter += x =>
        {
            //CurrentState = NPCInputs.IDLE;
            //OnStateChanged((int)CurrentState);
            CurrentNPCState = NPCInputs.AVOID_OBSTACLES;
            currentBehaviour = avoidObstaclesBehaviour;
            Debug.Log("START AVOID OBSTACLES...");
        };

        avoidObstacles.OnUpdate += () =>
        {
            currentBehaviour.ExecuteState();
        };

        avoidObstacles.OnExit -= x =>
        {
            Debug.Log("END AVOID OBSTACLES...");
        };

        attack.OnEnter += x =>
        {
            CurrentNPCState = NPCInputs.ATTACK;
            //OnStateChanged((int)CurrentState);
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
            CurrentNPCState = NPCInputs.ESCAPE;
            //OnStateChanged((int)CurrentState);
            currentBehaviour = escapeBehaviour;
            GetNodeFinalFromCornersList();
            Debug.Log("START MOVE TO POINT...");
        };

        escape.OnUpdate += () =>
        {
            currentBehaviour.ExecuteState();
        };

        escape.OnExit -= x =>
        {
            Debug.Log("END MOVE TO POINT...");
        };

        die.OnEnter += x =>
        {
            CurrentNPCState = NPCInputs.DIE;
            //OnStateChanged((int)CurrentState);
            currentBehaviour = dieBehaviour;
            Debug.Log("START DIE...");
        };

        die.OnUpdate += () =>
        {
            currentBehaviour.ExecuteState();
        };

        die.OnExit -= x =>
        {
            Debug.Log("END DIE...");
        };

        follow.GetTransition(NPCInputs.ATTACK).OnTransition += x =>
        {
            //CurrentState = NPCInputs.ATTACK;
            Debug.Log("TRANSITION FOLLOW TO ATTACK...");
            //OnStateChanged((int)CurrentState);
        };

        follow.GetTransition(NPCInputs.AVOID_OBSTACLES).OnTransition += x =>
        {
            //CurrentState = NPCInputs.ATTACK;
            Debug.Log("TRANSITION FOLLOW TO AVOID OBSTACLES...");
            //OnStateChanged((int)CurrentState);
        };

        attack.GetTransition(NPCInputs.FOLLOW).OnTransition += x =>
        {
            //CurrentState = NPCInputs.MOVE_TO_POINT;
            Debug.Log("TRANSITION ATTACK TO FOLLOW...");
            //OnStateChanged((int)CurrentState);
        };

        escape.GetTransition(NPCInputs.FOLLOW).OnTransition += x =>
        {
            //CurrentState = NPCInputs.MOVE_TO_WAYPOINT;
            Debug.Log("TRANSITION ESCAPE TO FOLLOW...");
            //OnStateChanged((int)NPCInputs.MOVE_TO_WAYPOINT);
        };

        attack.GetTransition(NPCInputs.ESCAPE).OnTransition += x =>
        {
            //CurrentState = NPCInputs.WANDER;
            Debug.Log("TRANSITION ATTACK TO ESCAPE...");
            //OnStateChanged((int)NPCInputs.WANDER);
        };

        follow.GetTransition(NPCInputs.DIE).OnTransition += x =>
        {
            //CurrentState = NPCInputs.DIE;
            Debug.Log("TRANSITION FOLLOW TO DIE...");
            //OnStateChanged((int)NPCInputs.DIE);
        };

        follow.GetTransition(NPCInputs.ESCAPE).OnTransition += x =>
        {
            //CurrentState = NPCInputs.WANDER;
            Debug.Log("TRANSITION FOLLOW TO ESCAPE...");
            //OnStateChanged((int)NPCInputs.WANDER);
        };

        FSM_NPCs = new EventFSM<NPCInputs>(idle);
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        Controller.OnUpdate();
    }
}
