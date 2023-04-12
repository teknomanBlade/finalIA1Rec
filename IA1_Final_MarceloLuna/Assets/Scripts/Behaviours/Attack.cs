using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : IBehaviour
{
    private BaseModel _model;

    public Attack(BaseModel model)
    {
        _model = model;
    }

    public void ExecuteState()
    {

    }
}
