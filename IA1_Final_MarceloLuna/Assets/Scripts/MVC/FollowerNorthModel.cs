using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowerNorthModel : BaseModel
{
    // Start is called before the first frame update
    void Awake()
    {
        HP = 100.0f;
        Faction = "North";
        Rank = "Follower";
        Damage = 5.0f;
        Speed = 4f;
        FollowerNorthView = GetComponent<FollowerNorthView>();
        Controller = new FollowerNorthController(this, FollowerNorthView);
    }

    // Update is called once per frame
    void Update()
    {
        Controller.OnUpdate();
    }
}
