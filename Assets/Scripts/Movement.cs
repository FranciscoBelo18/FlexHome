using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using System.Collections.Generic;

public class Movement : MonoBehaviour
{
    private NavMeshAgent agent = null;
    private GameObject[] MachinesAvailables;
    private int ObjectToGo;
    public string[] MachinesTags;
    private List<GameObject> GymMachines = new List<GameObject>();
    private GameObject DesiredMachine = null;


    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        foreach (string tag in MachinesTags)
        {
            MachinesAvailables = GameObject.FindGameObjectsWithTag(tag);
            GymMachines.AddRange(MachinesAvailables);
        }
        Debug.Log("Maquinas no gym: " + GymMachines.Count);
        for(int i = 0; i < GymMachines.Count; i++)
        {
            Debug.Log(GymMachines[i].tag);
        }
    }

    void Update()
    {
        if(agent.hasPath == false && agent.isStopped == false)
        {
            ObjectToGo = Random.Range(0, GymMachines.Count);
            Debug.Log("int random: " + ObjectToGo);
            agent.SetDestination(GymMachines[ObjectToGo].transform.position);
            DesiredMachine = GymMachines[ObjectToGo];
            
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject == DesiredMachine)
        {
            switch (DesiredMachine.tag)
            { 
                case "Treadmill":
                    Debug.Log("Treadmill");
                    
                    agent.ResetPath();
                    agent.isStopped = true;
                    break;
                case "Bike":
                    Debug.Log("Bike");
                    agent.ResetPath();
                    agent.isStopped = true;
                    break;
                case "BackMachine":
                    Debug.Log("BackMachine");
                    agent.ResetPath();
                    agent.isStopped = true;
                    break;
                case "BackMachine2":
                    Debug.Log("BackMachine2");
                    agent.ResetPath();
                    agent.isStopped = true;
                    break;
                case "StationaryRowing":
                    Debug.Log("StationaryRowing");
                    agent.ResetPath();
                    agent.isStopped = true;
                    break;
            }
        }
    }
}
