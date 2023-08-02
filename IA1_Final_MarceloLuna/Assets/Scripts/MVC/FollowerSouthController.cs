using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowerSouthController : IController
{
    private FollowerSouthModel _model;
    private FollowerSouthView _view;

    public FollowerSouthController(FollowerSouthModel _m, FollowerSouthView _v)
    {
        _model = _m;//Guardo la referencia del modelo que me llego en mi variable privada
        _view = _v; //Guardo la referencia de la vista que me llego en mi variable privada
    }

    public void OnUpdate()
    {
        _model.CheckBounds();
        _model.UpdateFSM_NPCs();
    }

}
