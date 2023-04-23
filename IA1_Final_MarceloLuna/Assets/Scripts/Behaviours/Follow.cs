using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Follow : IBehaviour
{
    private BaseModel _model;
    public float viewRadius;
    public float separationWeight;
    public float alignmentWeight;
    public float cohesionWeight;
    public float seekWeight;
    public BaseModel targetSeek;
    int index = 0;
    public Follow(BaseModel model)
    {
        _model = model;
        viewRadius = 2f;
        separationWeight = 2f;
        alignmentWeight = 1.5f;
        cohesionWeight = 1.2f;
        seekWeight = 4f;
        targetSeek = _model.npcs
            .Where(x => x.Rank.Equals("Leader") && x.Faction.Equals(_model.Faction))
            .FirstOrDefault();
    }

    public void ExecuteState()
    {
        _model.TargetPosition = targetSeek.TargetPosition;
        if (_model.InSight())
        {
            AddForce(Seek(targetSeek.transform.position) * seekWeight +
                 Separation() * separationWeight +
                 Alignment() * alignmentWeight +
                 Cohesion() * cohesionWeight);
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
    Vector3 Separation()
    {
        Vector3 desired = new Vector3();

        int count = 0;
        _model.npcs.ToList().ForEach(boid =>
            {
                Vector3 dist = (boid.transform.position - _model.transform.position);
                if (dist.magnitude < viewRadius)
                {
                    desired.x += dist.x;
                    desired.z += dist.z;
                    count++;
                }
            }
        );
        
        if (count <= 0) return desired;

        desired /= count;

        return CalculateSteering(-desired);
    }

    Vector3 Alignment()
    {
        Vector3 desired = new Vector3();

        int count = 0;
        _model.npcs.ToList().ForEach(boid =>
            {
                if (Vector3.Distance(_model.transform.position, boid.transform.position) < viewRadius)
                {
                    desired.x += boid._velocity.x;
                    desired.z += boid._velocity.z;
                    count++;
                }
            }
        );
        
        if (count == 0) return desired;
        desired /= count;
        return CalculateSteering(desired);

    }

    Vector3 Cohesion()
    {
        Vector3 desired = new Vector3();
        int count = 0;
        _model.npcs.ToList().ForEach(boid =>
            {
                if (Vector3.Distance(_model.transform.position, boid.transform.position) < viewRadius)
                {
                    desired += boid.transform.position;
                    count++;
                }
            }
        );
        
        if (count == 0) return desired;
        desired /= count;
        desired = (desired - _model.transform.position); //pos final - pos inicial
        desired.y = 0;

        return (CalculateSteering(desired));
    }

    public Vector3 CalculateSteering(Vector3 desired)
    {
        desired.Normalize();
        desired *= _model.maxSpeed;

        Vector3 steering = Vector3.ClampMagnitude(desired - _model._velocity, _model.maxForce / 10);
        return steering;
    }

    Vector3 Seek(Vector3 pos)
    {
        Vector3 desired = (pos - _model.transform.position).normalized * _model.maxSpeed;
        return CalculateSteering(desired);
    }
    void AddForce(Vector3 force)
    {
        _model._velocity += Vector3.ClampMagnitude(force, _model.maxSpeed);
    }

    public void Move() 
    {
        _model.GetNodeByLesserDistance();
        _model.transform.position += _model._velocity * Time.deltaTime;
        _model.transform.forward = _model._velocity;
        _model.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
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
                    //TriggerObstacleBetween("HasObstaclesBetween");
                    Debug.Log("LLEGA A DETECTAR OBSTACULO - MOVE TO POINT?");
                    Debug.DrawRay(_model.transform.position, -_model.DirToTarget, Color.red);
                }
                else if (hit.collider.gameObject.layer == 8)
                {
                    Debug.DrawRay(_model.transform.position, -_model.DirToTarget, Color.green);
                    Debug.Log("LLEGA A DETECTAR ENEMIGO - MOVE TO POINT?");
                    //TriggerSetteableTarget("SetAttackingTarget", hit.collider.gameObject.GetComponent<BaseModel>());
                    //TriggerAttackTarget("IsAttacking");
                }
            }
        }
    }
}
