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
    private Vector3 PositionToSpawn;
    private Animator animator;


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
        animator = GetComponent<Animator>();
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
            animator.SetBool("ReachedDestination", true);
            switch (DesiredMachine.tag)
            { 
                case "Treadmill":
                    Debug.Log("Treadmill");
                    Debug.Log("1) " + DesiredMachine.name); 

                    Debug.Log("2) " + DesiredMachine.transform.GetChild(0).name + " " + DesiredMachine.transform.GetChild(1).name);

                    //Debug.Log("2) " + DesiredMachine.transform.Find("Spawner").gameObject.transform.position);
                    //PositionToSpawn = DesiredMachine.transform.Find("Spawner").gameObject.transform.position;
                    
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
