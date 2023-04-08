using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageTargetObserver
{
    void OnNotifyDamageTarget(string message, float damage);
}

