using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderNorthModel : BaseModel
{
    public float arriveRadius;

    public float maxSpeed;
    public float maxForce;
    public float offsetY;
    public Vector3 _velocity;
    // Start is called before the first frame update
    void Awake()
    {
        HP = 150.0f;
        MaxHP = HP;
        Faction = "North";
        Rank = "Leader";
        Damage = 8.0f;
        DistanceNodeThreshold = 0.25f;
        LeaderNorthView = GetComponent<LeaderNorthView>();
        Controller = new LeaderNorthController(this, LeaderNorthView);
        #region EventFSM

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
            //CurrentState = LeaderInputs.IDLE;
            //OnStateChanged((int)CurrentState);
            //currentBehaviour = new Idle(rb);
            Debug.Log("START MOVE TO POINT...");
        };

        moveToPoint.OnUpdate += () =>
        {
            //currentBehaviour.ExecuteState();
        };

        moveToPoint.OnExit -= x =>
        {
            Debug.Log("END MOVE TO POINT...");
        };

        attack.OnEnter += x =>
        {
            //CurrentState = LeaderInputs.WANDER;
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
            //CurrentState = LeaderInputs.MOVE_TO_WAYPOINT;
            //OnStateChanged((int)CurrentState);
            //currentBehaviour = previousMovementBehaviour;
            Debug.Log("START MOVE TO POINT...");
        };

        escape.OnUpdate += () =>
        {
            //currentBehaviour.ExecuteState();
        };

        escape.OnExit -= x =>
        {
            Debug.Log("END MOVE TO POINT...");
        };

        die.OnEnter += x =>
        {
            /*CurrentState = LeaderInputs.DIE;
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
    public void Move() 
    {
        GetNodeByLesserDistance();
        transform.position += _velocity * Time.deltaTime;
        transform.forward = _velocity;
        transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
    }

    public void Arrive()
    {
        if (TargetPosition.Equals(Vector3.zero)) return;

        //if (GameManager.instance.InSight(transform.position, TargetPosition))
            //Debug.Log("ESTA A LA VISTA: " + GameManager.instance.InSight(transform.position, TargetPosition) + " - " + gameObject.name);

        Vector3 desired = (TargetPosition - transform.position).normalized;
        float dist = Vector3.Distance(transform.position, TargetPosition);
        float speed = maxSpeed;
        if (dist <= arriveRadius)
        {
            speed = maxSpeed * (dist / arriveRadius);
        }

        desired *= speed;
        Vector3 steering = Vector3.ClampMagnitude(desired - _velocity, maxForce);
        

        ApplyForce(steering);
    }

    void ApplyForce(Vector3 force)
    {
        _velocity += force;
    }
    
}
