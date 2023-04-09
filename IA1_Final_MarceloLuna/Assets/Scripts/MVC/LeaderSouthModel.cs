using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderSouthModel : BaseModel
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
        Faction = "South";
        Rank = "Leader";
        Damage = 8.0f;
        LeaderSouthView = GetComponent<LeaderSouthView>();
        Controller = new LeaderSouthController(this, LeaderSouthView);
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
