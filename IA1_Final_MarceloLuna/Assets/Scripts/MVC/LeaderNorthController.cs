using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderNorthController : IController
{
    private LeaderNorthModel _model;
    private LeaderNorthView _view;

    public LeaderNorthController(LeaderNorthModel _m, LeaderNorthView _v)
    {
        _model = _m;//Guardo la referencia del modelo que me llego en mi variable privada
        _view = _v; //Guardo la referencia de la vista que me llego en mi variable privada
    }

    public void OnUpdate()
    {
        if (Input.GetMouseButtonDown(0))
            _model.GetPositionInScene();
            
        _model.Arrive();
        _model.Move();

        _model.UpdateFSMLeaders();
    }

}
