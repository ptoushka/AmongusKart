using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BumperInfo : MonoBehaviour
{
    public float power = 50f;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Vector3 diff = collision.transform.position - transform.position;
            Vector3 knockBack = power * diff;
            knockBack.y = 0;
            collision.transform.GetComponent<Rigidbody>().AddForce(knockBack);
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Vector3 diff = collision.transform.position - transform.position;
            Vector3 knockBack = power * diff;
            knockBack.y = 0;
            collision.transform.GetComponent<Rigidbody>().AddForce(knockBack);
        }
    }
}
