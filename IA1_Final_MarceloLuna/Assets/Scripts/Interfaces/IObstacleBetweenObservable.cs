using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObstacleBetweenObservable
{
    void AddObserverObstacleBetween(IObstacleBetweenObserver obs);
    void RemoveObserverObstacleBetween(IObstacleBetweenObserver obs);
    void TriggerObstacleBetween(string message);
}
