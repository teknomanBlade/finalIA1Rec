using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idle : IBehaviour
{
    private BaseModel _model;
    private BaseModel _target;
    public Idle(BaseModel model, BaseModel target) 
    {
        _model = model;
        _target = target;
    }

    public void ExecuteState()
    {
        _model._velocity = Vector3.zero;
        if (_target._velocity != Vector3.zero) 
        {
            Debug.Log("SE MOVIO EL LIDER...");
        }
    }
}
