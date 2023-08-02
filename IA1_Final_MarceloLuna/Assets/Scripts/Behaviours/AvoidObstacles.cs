using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidObstacles : IBehaviour, IObstacleBetweenObserver
{
    private BaseModel _model;
    private BaseModel _target;
    private GameObject _obstacle;
    public float avoidWeight;
    public int index = 0;
    public AvoidObstacles(BaseModel model, BaseModel target)
    {
        _model = model;
        _target = target;
        avoidWeight = 6f;
    }

    public void ExecuteState()
    {
        Debug.Log("EXECUTE AVOID...");
        _model.TargetPosition = _target.transform.position;
        _model.finalNode = _target.finalNode;
        if (!_model.InSight())
        {
            //Debug.Log("NO VE AL LIDER - AVOID");
            if (_obstacle.CompareTag("Walls"))
            {
                Debug.Log("THETA STAR...");
                TravelPath(_model.GetPath(_model.currentNode, _model.finalNode));
            }
            else 
            {
                Debug.Log("AVOIDANCE...");
                ApplyForce(Seek(_target.transform.position) + ObstacleAvoidance() * avoidWeight);
                Move();
            }
        }
        else
        {
            //Debug.Log("VE AL LIDER - AVOID");
            ApplyForce(Seek(_target.transform.position));
            Move();
        }
    }
    public void TravelPath(List<Node> path)
    {
        if (path == null || path.Count <= 0 /*|| index >= path.Count*/) return;

        if (index > path.Count - 1)
        {
            //Debug.Log("VUELVE EL INDEX EN 0 - ANTES: "+ index);
            //pathToFollow.Clear();
            index = 0;
            //Debug.Log("VUELVE EL INDEX EN 0 - DESPUES: " + index);
        }
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
            Debug.Log("OBSTACLE: " + _obstacle.name);
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
        var distanceToTarget = Vector3.Distance(target, _model.transform.position);
        //Debug.Log("DISTANCE TO TARGET: " + distanceToTarget);
        if (distanceToTarget <= 0.5f) 
        {
            Debug.Log("ESTA CERCA DEL LIDER...");
            _model.FSM_NPCs.SendInput(BaseModel.NPCInputs.FOLLOW);
        }
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
