using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderSouthModel : BaseModel
{
    // Start is called before the first frame update
    void Awake()
    {
        HP = 150.0f;
        Faction = "South";
        Rank = "Leader";
        Damage = 8.0f;
        Speed = 6f;
        LeaderSouthView = GetComponent<LeaderSouthView>();
        Controller = new LeaderSouthController(this, LeaderSouthView);
    }

    // Update is called once per frame
    void Update()
    {
        Controller.OnUpdate();
    }
}
