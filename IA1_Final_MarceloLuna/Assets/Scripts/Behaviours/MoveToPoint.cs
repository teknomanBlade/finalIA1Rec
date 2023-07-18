using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MoveToPoint : IBehaviour, ISetteableTargetObservable, IObstacleBetweenObservable, IAttackTargetObservable
{
    List<ISetteableTargetObserver> _myObserversSetteableTarget = new List<ISetteableTargetObserver>();
    List<IObstacleBetweenObserver> _myObserversObstacleBetween = new List<IObstacleBetweenObserver>();
    List<IAttackTargetObserver> _myObserversAttackTarget = new List<IAttackTargetObserver>();
    private BaseModel _model;
    int index = 0;
    public MoveToPoint(BaseModel model)
    {
        _model = model;
        AddObserverAttackTarget(_model);
        AddObserverObstacleBetween(_model);
    }

    public void ExecuteState()
    {
        #region User Movement Leader
        _model.GetNodeFinalByLesserDistance(_model.TargetPosition);
        DetectPlayer(_model.Faction);
        if (_model.InSight())
        {
            Arrive();
            Move();
        }
        else 
        {
            TravelPath(_model.GetPath(_model.currentNode, _model.finalNode));
        }
        #endregion
    }
    public void TravelPath(List<Node> path) 
    {
        if(path == null || path.Count <= 0 || index >= path.Count) return;

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

    public void DetectPlayer(string faction)
    {
        var northerns = _model.npcs.Where(x => x.gameObject.GetComponent<BaseModel>().Faction.Contains("North")).ToList();
        var southerns = _model.npcs.Where(x => x.gameObject.GetComponent<BaseModel>().Faction.Contains("South")).ToList();
        if (faction.Contains("North"))
        {
            southerns.ForEach(x => EnemyDetection(x));
        }
        else
        {
            northerns.ForEach(x => EnemyDetection(x));
        }
    }

    public void EnemyDetection(BaseModel npc)
    {
        _model.DistanceToTarget = Vector3.Distance(_model.transform.position, npc.transform.position);
        _model.DirToTarget = (_model.transform.position - npc.transform.position).normalized;

        //Debug.Log("DISTANCIA AL OBJETIVO: " + viewModel.DistanceToTarget);
        //Debug.Log("HAY OBSTACULOS: " + viewModel.ObstaclesBetween);
        _model.AngleToTarget = Vector3.Angle(-_model.transform.forward, _model.DirToTarget);

        if (_model.DistanceToTarget < _model.DistanceThreshold && _model.AngleToTarget < _model.AngleThreshold)
        {
            if (Physics.Raycast(_model.transform.position, -_model.DirToTarget, out RaycastHit hit, _model.DistanceToTarget))
            {
                //Una vez que descartamos las primeras posibilidades, vamos a utilizar un raycast.            
                if (hit.collider.gameObject.layer == 6)
                {
                    TriggerObstacleBetween("HasObstaclesBetween");
                    Debug.Log("LLEGA A DETECTAR OBSTACULO - MOVE TO POINT?");
                    Debug.DrawRay(_model.transform.position, -_model.DirToTarget, Color.red);
                }
                else if (hit.collider.gameObject.layer == 8)
                {
                    Debug.DrawRay(_model.transform.position, -_model.DirToTarget, Color.green);
                    Debug.Log("LLEGA A DETECTAR ENEMIGO - MOVE TO POINT?");
                    TriggerSetteableTarget("SetAttackingTarget", hit.collider.gameObject.GetComponent<BaseModel>());
                    TriggerAttackTarget("IsAttacking");
                }
            }
        }
    }
    public void AddObserverSetteableTarget(ISetteableTargetObserver obs)
    {
        _myObserversSetteableTarget.Add(obs);
    }

    public void RemoveObserverSetteableTarget(ISetteableTargetObserver obs)
    {
        if (_myObserversSetteableTarget.Contains(obs))
        {
            _myObserversSetteableTarget.Remove(obs);
        }
    }

    public void TriggerSetteableTarget(string message, BaseModel target)
    {
        _myObserversSetteableTarget.ForEach(x => x.OnNotifySetteableTarget(message, target));
    }

    public void AddObserverObstacleBetween(IObstacleBetweenObserver obs)
    {
        _myObserversObstacleBetween.Add(obs);
    }

    public void RemoveObserverObstacleBetween(IObstacleBetweenObserver obs)
    {
        if (_myObserversObstacleBetween.Contains(obs))
        {
            _myObserversObstacleBetween.Remove(obs);
        }
    }

    public void TriggerObstacleBetween(string message)
    {
        _myObserversObstacleBetween.ForEach(x => x.OnNotifyObstacleBetween(message));
    }

    public void AddObserverAttackTarget(IAttackTargetObserver obs)
    {
        _myObserversAttackTarget.Add(obs);
    }

    public void RemoveObserverAttackTarget(IAttackTargetObserver obs)
    {
        if (_myObserversAttackTarget.Contains(obs))
        {
            _myObserversAttackTarget.Remove(obs);
        }
    }

    public void TriggerAttackTarget(string message)
    {
        _myObserversAttackTarget.ForEach(x => x.OnNotifyAttackTarget(message));
    }

}
