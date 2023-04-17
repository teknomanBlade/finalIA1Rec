using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackTargetObservable 
{
    void AddObserverAttackTarget(IAttackTargetObserver obs);
    void RemoveObserverAttackTarget(IAttackTargetObserver obs);
    void TriggerAttackTarget(string message);
}
