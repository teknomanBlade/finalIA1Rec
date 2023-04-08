using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderNorthModel : BaseModel
{
    public float arriveRadius;

    public float maxSpeed;
    public float maxForce;
    public Vector3 _velocity;
    // Start is called before the first frame update
    void Awake()
    {
        HP = 150.0f;
        Faction = "North";
        Rank = "Leader";
        Damage = 8.0f;
        LeaderNorthView = GetComponent<LeaderNorthView>();
        Controller = new LeaderNorthController(this, LeaderNorthView);
    }

    // Update is called once per frame
    void Update()
    {
        Controller.OnUpdate();
    }
    public void Move() 
    {
        transform.position += _velocity * Time.deltaTime;
        transform.forward = _velocity;
    }

    public void Arrive()
    {
        if (TargetPosition.Equals(Vector3.zero)) return;

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
