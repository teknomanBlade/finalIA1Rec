using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Die : IBehaviour
{
    private BaseModel _model;

    public Die(BaseModel model)
    {
        _model = model;
    }
    public void ExecuteState()
    {

    }
}
