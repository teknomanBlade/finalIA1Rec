using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISetteableTargetObservable
{
    void AddObserverSetteableTarget(ISetteableTargetObserver obs);
    void RemoveObserverSetteableTarget(ISetteableTargetObserver obs);
    void TriggerSetteableTarget(string message, BaseModel target);
}
