using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TriggerDoor : MonoBehaviour
{
    public Vector3 pos = Vector3.zero;
    public Vector3 dir = Vector3.zero;
    public float radius;
    public Animator Anim;
    // Start is called before the first frame update
    void Start()
    {
        radius = 1.2f;
        pos = transform.position;
        dir = transform.forward;
        Anim = GetComponentInParent<Animator>();
        Anim.SetBool("IsOpened", true);
    }

    // Update is called once per frame
    void Update()
    {
        var colliders = Physics.OverlapSphere(pos, radius, 1 << 8);
        if (colliders.Length > 0) 
        {
            colliders.Where(x => x.gameObject.name.Contains("Leader")).ToList().ForEach( npc => {
                Debug.Log("LEADER: " + npc.gameObject.name);
                StartCoroutine(CloseDoor());
            });
        }
    }

    IEnumerator CloseDoor() 
    {
        yield return new WaitForSeconds(0.6f);
        Anim.SetBool("IsOpened", false);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(pos, radius);
    }

}
