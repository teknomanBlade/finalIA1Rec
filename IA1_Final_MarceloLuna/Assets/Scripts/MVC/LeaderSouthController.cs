using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderSouthController : IController
{
    private LeaderSouthModel _model;
    private LeaderSouthView _view;

    public LeaderSouthController(LeaderSouthModel _m, LeaderSouthView _v)
    {
        _model = _m;//Guardo la referencia del modelo que me llego en mi variable privada
        _view = _v; //Guardo la referencia de la vista que me llego en mi variable privada
    }

    public void OnUpdate()
    {
        _model.UpdateFSMLeaders();
    }

}
