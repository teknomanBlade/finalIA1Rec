using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Follow : IBehaviour, ISetteableTargetObservable, IObstacleBetweenObservable, IAttackTargetObservable
{
    List<ISetteableTargetObserver> _myObserversSetteableTarget = new List<ISetteableTargetObserver>();
    List<IObstacleBetweenObserver> _myObserversObstacleBetween = new List<IObstacleBetweenObserver>();
    List<IAttackTargetObserver> _myObserversAttackTarget = new List<IAttackTargetObserver>();
    private BaseModel _model;
    public BaseModel targetSeek;
    int index = 0;
    public float viewRadius;
    public float separationWeight;
    public float alignmentWeight;
    public float cohesionWeight;
    public float seekWeight;
    public Follow(BaseModel model, BaseModel target)
    {
        _model = model;
        targetSeek = target;
        viewRadius = 3f;
        separationWeight = 0.75f;
        alignmentWeight = 1f;
        cohesionWeight = 0.25f;
        seekWeight = 0.4f;
    }

    public bool ChooseByNameAndFaction(BaseModel npc) 
    {
        var containsFollower = npc.Rank.Equals("Follower");
        var isSameFaction = npc.Faction.Equals(_model.Faction);

        return containsFollower && isSameFaction;
    }
    
    public void ExecuteState()
    {
        _model.TargetPosition = targetSeek.transform.position;
        DetectPlayer(_model.Faction);
        if (_model.InSight())
        {
            AddForce(
                     Separation() * separationWeight +
                     Alignment() * alignmentWeight +
                     Cohesion() * cohesionWeight +
                     Seek(targetSeek.transform.position) * seekWeight
                    );
            Move();
        }
        else 
        {
            TravelPath(_model.GetPath(_model.currentNode, targetSeek.finalNode));
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
        
        var allFactionBoids = _model.NPCs.Where(x => ChooseByNameAndFaction(x)).ToList();
        //Debug.Log("COUNT FACTION BOIDS SEPARATION: " + allFactionBoids.Count);

        foreach (var boid in allFactionBoids)
        {
            if (boid.gameObject.name.Equals(_model.gameObject.name)) continue;

            Vector3 dist = (boid.transform.position - _model.transform.position);
            if (dist.magnitude < viewRadius)
            {
                desired.x += dist.x;
                desired.z += dist.z;
                count++;
            }
        }
        
        if (count <= 0) return desired;

        desired /= count;

        return CalculateSteering(-desired);
    }

    Vector3 Alignment()
    {
        Vector3 desired = new Vector3();

        int count = 0;

        var allFactionBoids = _model.NPCs.Where(x => ChooseByNameAndFaction(x)).ToList();
        //Debug.Log("COUNT FACTION BOIDS ALIGNMENT: " + allFactionBoids.Count);
        foreach (var boid in allFactionBoids)
        {
            if (boid.gameObject.name.Equals(_model.gameObject.name)) continue;

            if (Vector3.Distance(_model.transform.position, boid.transform.position) < viewRadius)
            {
                desired.x += boid._velocity.x;
                desired.z += boid._velocity.z;
                count++;
            }
        }
        
        if (count == 0) return desired;
        desired /= count;
        return CalculateSteering(desired);

    }

    Vector3 Cohesion()
    {
        Vector3 desired = new Vector3();
        int count = 0;
        
        var allFactionBoids = _model.NPCs.Where(x => ChooseByNameAndFaction(x)).ToList();
        //Debug.Log("COUNT FACTION BOIDS COHESION: " + allFactionBoids.Count);

        foreach (var boid in allFactionBoids)
        {
            if (boid.gameObject.name.Equals(_model.gameObject.name)) continue;

            if (Vector3.Distance(_model.transform.position, boid.transform.position) < viewRadius)
            {
                desired += boid.transform.position;
                count++;
            }
        }

        if (count == 0) return desired;
        desired /= count;
        desired = (desired - _model.transform.position);
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
        var dirToLeader = (pos - _model.transform.position).normalized;
        var distanceToLeader = Vector3.Distance(pos, _model.transform.position);
        Debug.DrawRay(_model.transform.position, dirToLeader, Color.white);
        if (Physics.Raycast(_model.transform.position, -dirToLeader, out RaycastHit hit, distanceToLeader, _model.ObstaclesLayer)) 
        {
            //Debug.Log("HIT OBSTACLE");
            Debug.DrawRay(_model.transform.position, dirToLeader, Color.red);
            TriggerObstacleBetween("HasObstaclesBetween", hit.collider.gameObject);
            _model.FSM_NPCs.SendInput(BaseModel.NPCInputs.AVOID_OBSTACLES);
        }

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
        var northerns = _model.NPCs.Where(x => x.gameObject.GetComponent<BaseModel>().Faction.Contains("North")).ToList();
        var southerns = _model.NPCs.Where(x => x.gameObject.GetComponent<BaseModel>().Faction.Contains("South")).ToList();
        if (faction.Contains("North"))
        {
            southerns.ForEach(npc => EnemyDetection(npc));
        }
        else
        {
            northerns.ForEach(npc => EnemyDetection(npc));
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
                if (hit.collider.gameObject.layer == 8)
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

    public void TriggerObstacleBetween(string message, GameObject obstacle)
    {
        _myObserversObstacleBetween.ForEach(x => x.OnNotifyObstacleBetween(message, obstacle));
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
