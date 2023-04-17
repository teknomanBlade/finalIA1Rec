using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISetteableTargetObserver
{
    void OnNotifySetteableTarget(string message, BaseModel target);
}
