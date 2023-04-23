using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : IBehaviour, ISetteableTargetObserver
{
    private BaseModel _model;
    public BaseModel _target { get; private set; }
    public Attack(BaseModel model)
    {
        _model = model;
    }

    public void ExecuteState()
    {
        AttackTarget(_target);
    }
    public void AttackTarget(BaseModel target)
    {
        _model.DistanceToTarget = Vector3.Distance(_model.transform.position, target.transform.position);
        //Debug.Log("DISTANCE TO TARGET: " + _viewModel.DistanceToTarget);
        _model.DirToTarget = (target.transform.position - _model.transform.position).normalized;

        if (_model.DistanceToTarget < _model.AttackDistanceThreshold)
        {
            //target.TriggerDamagingTarget("IsDamaging", _model.Damage);
            _model._velocity = Vector3.zero;
        }
        else
        {
            //_pursuitSteeringBehaviour.SetOwner(_owner).SetRank(_rank).SetViewModel(_viewModel);
            //_pursuitSteeringBehaviour.Execute();
        }

    }
    public void OnNotifySetteableTarget(string message, BaseModel target)
    {
        if (message.Equals("SetAttackingTarget"))
        {
            _target = target;
        }
    }
}
