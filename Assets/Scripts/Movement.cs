using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "treadmill")
        {
            Debug.Log("passadeira");
        }
    }

}
