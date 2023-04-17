using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackTargetObserver
{
    void OnNotifyAttackTarget(string message);
}
