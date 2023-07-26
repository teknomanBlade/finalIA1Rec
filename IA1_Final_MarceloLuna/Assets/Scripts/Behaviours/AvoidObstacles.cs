using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidObstacles : IBehaviour, IObstacleBetweenObserver
{
    private BaseModel _model;
    private BaseModel _target;
    private GameObject _obstacle;
    public AvoidObstacles(BaseModel model, BaseModel target)
    {
        _model = model;
        _target = target;
    }

    public void ExecuteState()
    {
        _model.TargetPosition = _target.transform.position;
        if (!_model.InSight())
        {
            Debug.Log("NO VE AL LIDER");
            ApplyForce(Seek(_target.transform.position) + ObstacleAvoidance() * _model.avoidWeight);
            Move();
        }
        else
        {
            Debug.Log("VE AL LIDER");
            _model.FSM_NPCs.SendInput(BaseModel.NPCInputs.FOLLOW);
            /*ApplyForce(Seek(_target.transform.position));
            Move();*/
        }
    }

    Vector3 ObstacleAvoidance()
    {
        /*Vector3 pos = _model.transform.position;
        Vector3 dir = _model.transform.forward;
        float dist = _model._velocity.magnitude;
        Debug.DrawLine(pos, pos + (dir * dist));
        if (Physics.SphereCast(pos, 1, dir, out RaycastHit hit, dist, 1 << _model.ObstaclesLayer.value))
        {*/
        if (_obstacle) 
        { 
            //Debug.Log("OBSTACLE: " + _obstacle.name);
            Vector3 dirToObject = _obstacle.transform.position - _model.transform.position;
            float angleInBetween = Vector3.SignedAngle(_model.transform.forward, dirToObject, Vector3.up);

            Vector3 desired = angleInBetween >= 0 ? -_model.transform.right : _model.transform.right; // va a depender de lo que nosotros queremos
            desired.Normalize();
            desired *= _model.maxSpeed;

            Vector3 steering = Vector3.ClampMagnitude(desired - _model._velocity, _model.maxForce / 10);
            return steering;
        }
        //}
        return Vector3.zero;
    }
    Vector3 Seek(Vector3 target)
    {
        Vector3 desired = (target - _model.transform.position).normalized * _model.maxSpeed;
        Vector3 steering = Vector3.ClampMagnitude(desired - _model._velocity, _model.maxForce / 10);
        return steering;
    }
    public void Move()
    {
        _model.GetNodeByLesserDistance();
        _model.transform.position += _model._velocity * Time.deltaTime;
        _model.transform.forward = _model._velocity;
        _model.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
    }
    void ApplyForce(Vector3 force)
    {
        _model._velocity += Vector3.ClampMagnitude(force, _model.maxSpeed); //clampear si necesario
    }
    public void OnNotifyObstacleBetween(string message, GameObject obstacle)
    {
        if (message.Equals("HasObstaclesBetween")) 
        {
            //Debug.Log("SE ENVIO EL OBSTACULO?? " + obstacle.name);
            _obstacle = obstacle;
        }
    }
}
