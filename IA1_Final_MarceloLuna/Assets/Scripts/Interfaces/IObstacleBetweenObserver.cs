using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObstacleBetweenObserver
{
    void OnNotifyObstacleBetween(string message, GameObject obstacle);
}
