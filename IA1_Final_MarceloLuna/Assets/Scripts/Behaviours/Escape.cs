using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Escape : IBehaviour
{
    private BaseModel _model;
    int index = 0;
    public Escape(BaseModel model)
    {
        _model = model;
    }

    public void ExecuteState()
    {
        if (_model.InSight())
        {
            Arrive();
            Move();
        }
        else
        {
            TravelPath(_model.GetPath(_model.currentNode, _model.finalNode));
        }
    }

    public void TravelPath(List<Node> path)
    {
        if (path == null || path.Count <= 0 || index >= path.Count) return;

        Vector3 dir = path[index].transform.position - _model.transform.position;

        Move(dir);

        if (dir.magnitude < 0.1f)
        {
            index = index >= path.Count - 1 ? 0 : index + 1;
        }
    }

    public void Move(Vector3 dir)
    {
        float speed = _model.maxSpeed;
        _model.transform.position += speed * Time.deltaTime * dir.normalized;
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
