using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class Movement : MonoBehaviour
{
    private NavMeshAgent agent = null;
    private GameObject[] ObjectToGo;
    private int currentObject;


    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        ObjectToGo = GameObject.FindGameObjectsWithTag("Treadmill");
        Debug.Log("Numero de passadeiras no gym: " + ObjectToGo.Length.ToString());
    }

    void Update()
    {
        if(agent.hasPath == false)
        {
            currentObject = Random.Range(0, ObjectToGo.Length);
            agent.SetDestination(ObjectToGo[currentObject].transform.position);

        }
    }
}
