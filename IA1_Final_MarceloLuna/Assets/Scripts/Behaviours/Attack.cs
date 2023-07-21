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
        //Debug.Log("DISTANCE TO TARGET: " + _model.DistanceToTarget);
        _model.DirToTarget = (target.transform.position - _model.transform.position).normalized;

        if (_model.DistanceToTarget < _model.AttackDistanceThreshold)
        {
            Debug.Log("IS DAMAGING? " + _model.name);
            _model.StartCoroutine(TakeDamageCoroutine(target,_model.Damage));
            _model._velocity = Vector3.zero;
        }
        /*else
        {
            _pursuitSteeringBehaviour.SetOwner(_owner).SetRank(_rank).SetViewModel(_viewModel);
            _pursuitSteeringBehaviour.Execute();
        }*/

    }
    IEnumerator TakeDamageCoroutine(BaseModel target, float damage)
    {
        Debug.Log("ANTES DE DAÑO...");
        yield return new WaitForSeconds(1.5f);
        target.TakeDamage(_model.Damage);
        Debug.Log("DESPUES DE DAÑO... " + damage);
    }
    public void OnNotifySetteableTarget(string message, BaseModel target)
    {
        if (message.Equals("SetAttackingTarget"))
        {
            _target = target;
        }
    }
}
