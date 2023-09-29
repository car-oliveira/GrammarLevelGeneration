using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    int count;
    RaycastHit m_Hit;
    BoxCollider hitbox;
    Vector3 hitboxSize;
    Vector3 castBoxSize;
    bool result;

    // Start is called before the first frame update
    void Start()
    {
        //GameObject entry = this.transform.Find("Entry Connector").gameObject;
        //GameObject start = GameObject.Find("Start Room");
        //GameObject exit = start.transform.Find("Main Exit Connector").gameObject;
        //entry.transform.position = exit.transform.position;
        //entry.transform.forward = exit.transform.forward;
        hitbox = this.gameObject.GetComponent<BoxCollider>();
        hitboxSize = hitbox.bounds.size;
        castBoxSize = hitboxSize / 2;
        hitbox.enabled = false;
        bool result = Physics.CheckBox(hitbox.bounds.center, castBoxSize);
        //hitbox.enabled = true;
        Collider[] colliders = Physics.OverlapBox(hitbox.bounds.center, castBoxSize);

        foreach (Collider C in colliders)
        {
            Debug.Log("Collider: " + C.transform.name);
        }
        Debug.Log(result);
        Debug.Log("hitboxSize: " + hitboxSize);
        Debug.Log("castBoxSize: " + castBoxSize);

    }

    private void FixedUpdate()
    {

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Gizmos.DrawWireCube(transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        //if (count < 10)
        //{
        //    this.transform.Translate(this.transform.forward, Space.World);
        //    count++;
        //}
       
    }
}
