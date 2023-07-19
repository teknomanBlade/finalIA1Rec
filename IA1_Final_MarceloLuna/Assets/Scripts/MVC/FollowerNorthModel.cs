using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FollowerNorthModel : BaseModel
{
    // Start is called before the first frame update
    void Awake()
    {
        HP = 100.0f;
        MaxHP = HP;
        Faction = "North";
        Rank = "Follower";
        Damage = 5.0f;
        FollowerNorthView = GetComponent<FollowerNorthView>();
        Controller = new FollowerNorthController(this, FollowerNorthView);
        #region EventFSM

        var followBehaviour = new Follow(this);
        var attackBehaviour = new Attack(this);
        var escapeBehaviour = new Escape(this);
        var dieBehaviour = new Die(this);

        previousMovementBehaviour = followBehaviour;
        currentBehaviour = followBehaviour;

        var follow = new State<NPCInputs>("FOLLOW");
        var attack = new State<NPCInputs>("ATTACK");
        var escape = new State<NPCInputs>("ESCAPE");
        var die = new State<NPCInputs>("DIE");

        StateConfigurer.Create(follow)
            .SetTransition(NPCInputs.ATTACK, attack)
            .SetTransition(NPCInputs.ESCAPE, escape)
            .SetTransition(NPCInputs.DIE, die)
            .Done();

        StateConfigurer.Create(attack)
            .SetTransition(NPCInputs.FOLLOW, follow)
            .SetTransition(NPCInputs.ESCAPE, escape)
            .SetTransition(NPCInputs.DIE, die)
            .Done();

        StateConfigurer.Create(escape)
            .SetTransition(NPCInputs.ATTACK, attack)
            .SetTransition(NPCInputs.FOLLOW, follow)
            .SetTransition(NPCInputs.DIE, die)
            .Done();

        StateConfigurer.Create(die).Done();

        follow.OnEnter += x =>
        {
            //CurrentState = NPCInputs.IDLE;
            //OnStateChanged((int)CurrentState);
            CurrentNPCState = NPCInputs.FOLLOW;
            currentBehaviour = previousMovementBehaviour;
            Debug.Log("START MOVE TO POINT...");
        };

        follow.OnUpdate += () =>
        {
            currentBehaviour.ExecuteState();
        };

        follow.OnExit -= x =>
        {
            Debug.Log("END MOVE TO POINT...");
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
            Debug.Log("TRANSITION MOVE TO ATTACK...");
            //OnStateChanged((int)CurrentState);
        };

        attack.GetTransition(NPCInputs.FOLLOW).OnTransition += x =>
        {
            //CurrentState = NPCInputs.MOVE_TO_POINT;
            Debug.Log("TRANSITION ATTACK TO MOVE TO POINT...");
            //OnStateChanged((int)CurrentState);
        };

        escape.GetTransition(NPCInputs.FOLLOW).OnTransition += x =>
        {
            //CurrentState = NPCInputs.MOVE_TO_WAYPOINT;
            Debug.Log("TRANSITION ESCAPE TO MOVE TO POINT...");
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
            Debug.Log("TRANSITION MOVE TO POINT TO DIE...");
            //OnStateChanged((int)NPCInputs.DIE);
        };

        follow.GetTransition(NPCInputs.ESCAPE).OnTransition += x =>
        {
            //CurrentState = NPCInputs.WANDER;
            Debug.Log("TRANSITION MOVE TO POINT TO ESCAPE...");
            //OnStateChanged((int)NPCInputs.WANDER);
        };

        FSM_NPCs = new EventFSM<NPCInputs>(follow);
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        Controller.OnUpdate();
    }
}
