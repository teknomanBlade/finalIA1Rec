using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowerSouthModel : BaseModel
{
    // Start is called before the first frame update
    void Awake()
    {
        HP = 100.0f;
        MaxHP = HP;
        Faction = "South";
        Rank = "Follower";
        Damage = 5.0f;
        FollowerSouthView = GetComponent<FollowerSouthView>();
        Controller = new FollowerSouthController(this, FollowerSouthView);
        
        #region EventFSM

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
            //currentBehaviour = new Idle(rb);
            Debug.Log("START FOLLOW...");
        };

        follow.OnUpdate += () =>
        {
            //currentBehaviour.ExecuteState();
        };

        follow.OnExit -= x =>
        {
            Debug.Log("END FOLLOW...");
        };

        attack.OnEnter += x =>
        {
            //CurrentState = NPCInputs.WANDER;
            //OnStateChanged((int)CurrentState);
            //currentBehaviour = new Wander(rb, Speed);
            Debug.Log("START ATTACK...");
        };

        attack.OnUpdate += () =>
        {
            //currentBehaviour.ExecuteState();
        };

        attack.OnExit -= x =>
        {
            Debug.Log("END ATTACK...");
        };

        escape.OnEnter += x =>
        {
            //CurrentState = NPCInputs.MOVE_TO_WAYPOINT;
            //OnStateChanged((int)CurrentState);
            //currentBehaviour = previousMovementBehaviour;
            Debug.Log("START ESCAPE...");
        };

        escape.OnUpdate += () =>
        {
            //currentBehaviour.ExecuteState();
        };

        escape.OnExit -= x =>
        {
            Debug.Log("END ESCAPE...");
        };

        die.OnEnter += x =>
        {
            /*CurrentState = NPCInputs.DIE;
            OnStateChanged((int)CurrentState);
            currentBehaviour = new Die(rb);
            enemy.Level.Points.Add(Points);*/
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

        follow.GetTransition(NPCInputs.ATTACK).OnTransition += x =>
        {
            //CurrentState = NPCInputs.ATTACK;
            Debug.Log("TRANSITION FOLLOW TO ATTACK...");
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

        FSM_NPCs = new EventFSM<NPCInputs>(follow);
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        Controller.OnUpdate();
    }
}
