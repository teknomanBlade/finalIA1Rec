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
        Debug.Log("EXECUTE IDLE...");
        _model._velocity = Vector3.zero;
        if (_target._velocity != Vector3.zero)
        {
            Debug.Log("SE MOVIO EL LIDER...");
            //Debug.Log("NPCS LENGTH: " + _model.NPCs.Length);
            _model.FSM_NPCs.SendInput(BaseModel.NPCInputs.FOLLOW);
        }
        /*else 
        {
            _model._velocity = Vector3.zero;
            _model.FSM_NPCs.SendInput(BaseModel.NPCInputs.IDLE);
        }*/
    }
}
