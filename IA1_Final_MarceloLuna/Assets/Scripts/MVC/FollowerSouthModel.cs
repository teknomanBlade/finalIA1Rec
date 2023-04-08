using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowerSouthModel : BaseModel
{
    // Start is called before the first frame update
    void Awake()
    {
        HP = 100.0f;
        Faction = "South";
        Rank = "Follower";
        Damage = 5.0f;
        Speed = 4f;
        FollowerSouthView = GetComponent<FollowerSouthView>();
        Controller = new FollowerSouthController(this, FollowerSouthView);
    }

    // Update is called once per frame
    void Update()
    {
        Controller.OnUpdate();
    }
}
