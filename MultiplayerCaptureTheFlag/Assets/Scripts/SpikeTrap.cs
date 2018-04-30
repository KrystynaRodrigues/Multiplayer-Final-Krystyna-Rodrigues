using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{

    void OnCollisionEnter(Collision col)
    {

       if(col.collider.tag == "Player" )
        {
            Destroy(gameObject);
        }
    }

  
}

