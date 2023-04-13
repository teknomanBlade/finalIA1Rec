using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToPoint : IBehaviour
{
    private BaseModel _model;

    public MoveToPoint(BaseModel model)
    {
        _model = model;
    }

    public void ExecuteState()
    {
        #region User Movement Leader
        _model.GetNodeFinalByLesserDistance(_model.TargetPosition);
        if (_model.InSight())
        {
            Arrive();
            Move();
        }
        #endregion
    }

    public void Move()
    {
        _model.GetNodeByLesserDistance();
        _model.transform.position += _model._velocity * Time.deltaTime;
        _model.transform.forward = _model._velocity;
        _model.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
    }

    public void Arrive()
    {
        if (_model.TargetPosition.Equals(Vector3.zero)) return;
        //if (GameManager.instance.InSight(transform.position, TargetPosition))
        //Debug.Log("ESTA A LA VISTA: " + GameManager.instance.InSight(transform.position, TargetPosition) + " - " + gameObject.name);

        Vector3 desired = (_model.TargetPosition - _model.transform.position).normalized;
        float dist = Vector3.Distance(_model.transform.position, _model.TargetPosition);
        float speed = _model.maxSpeed;
        if (dist <= _model.arriveRadius)
        {
            speed = _model.maxSpeed * (dist / _model.arriveRadius);
        }

        desired *= speed;
        Vector3 steering = Vector3.ClampMagnitude(desired - _model._velocity, _model.maxForce);

        ApplyForce(steering);
    }

    void ApplyForce(Vector3 force)
    {
        _model._velocity += force;
    }
}
