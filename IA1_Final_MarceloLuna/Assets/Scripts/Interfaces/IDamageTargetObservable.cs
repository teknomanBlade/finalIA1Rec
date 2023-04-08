using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageTargetObservable
{
    void AddObserverDamageTarget(IDamageTargetObserver obs);
    void RemoveObserverDamageTarget(IDamageTargetObserver obs);
    void TriggerDamageTarget(string message, float damage);
}
