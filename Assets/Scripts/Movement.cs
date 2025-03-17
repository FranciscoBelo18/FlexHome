using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using System.Collections.Generic;

public class Movement : MonoBehaviour
{
    private NavMeshAgent agent;
    private GameObject[] MachinesAvailables;
    private int ObjectToGo;
    public string[] MachinesTags;
    private List<GameObject> GymMachines = new List<GameObject>();
    private GameObject DesiredMachine = null;
    private Animator animator;
    private int ChildInt;

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
        MakeDestination();
    }

    void Update()
    {

    }


    private void MakeDestination()
    {
        ObjectToGo = Random.Range(0, GymMachines.Count);
        Debug.Log("int random: " + ObjectToGo);
        if(VerifySpawnerState(GymMachines[ObjectToGo]))
        {
            agent.SetDestination(GymMachines[ObjectToGo].transform.position);
            DesiredMachine = GymMachines[ObjectToGo];
            GetChildIntSpawner(DesiredMachine);
        }
        else
        {
            MakeDestination();
        }
    }
    
    private bool VerifySpawnerState(GameObject machine)
    {
        for(int i = 0; i < machine.transform.childCount; i++)
        {
            if(machine.transform.GetChild(i).name == "Spawner")
            {
                Debug.Log("Spawner: " + machine.transform.GetChild(i).gameObject.activeSelf);
                return machine.transform.GetChild(i).gameObject.activeSelf;
            }
        }
        return true;
    }

    private void GetChildIntSpawner(GameObject parent)
    {
        for(int i = 0; i < parent.transform.childCount; i++)
        {
            if (parent.transform.GetChild(i).name == "Spawner")
            {
                ChildInt = i;
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject == DesiredMachine)
        {
            switch (DesiredMachine.tag)
            { 
                case "Treadmill":
                    animator.SetBool("ReachedDestination", true);
                    agent.isStopped = true;
                    agent.velocity = Vector3.zero;
                    agent.acceleration = 0;
                    agent.enabled = false;
                    DesiredMachine.GetComponent<Collider>().enabled = false;
                    Debug.Log("Treadmill");
                    Debug.Log("1) " + DesiredMachine.name); 
                    
                    transform.position = DesiredMachine.transform.GetChild(ChildInt).position;
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                    Debug.Log("Posicao do NPC: " + transform.position);
                    Debug.Log("Posicao do Spawner: " + DesiredMachine.transform.GetChild(ChildInt).position);

                    
                    break;
                case "Bike":
                    animator.SetBool("ReachedDestination", true);
                    agent.isStopped = true;
                    agent.velocity = Vector3.zero;
                    agent.acceleration = 0;
                    Debug.Log("Bike");
                    Debug.Log("1) " + DesiredMachine.name);

                    break;
                case "BackMachine":
                    animator.SetBool("ReachedDestination", true);
                    agent.isStopped = true;
                    agent.velocity = Vector3.zero;
                    agent.acceleration = 0;
                    Debug.Log("BackMachine");
                    Debug.Log("1) " + DesiredMachine.name);

                    break;
                case "BackMachine2":
                    animator.SetBool("ReachedDestination", true);
                    agent.isStopped = true;
                    agent.velocity = Vector3.zero;
                    agent.acceleration = 0;
                    Debug.Log("BackMachine2");
                    Debug.Log("1) " + DesiredMachine.name);

                    break;
                case "StationaryRowing":
                    animator.SetBool("ReachedDestination", true);
                    agent.isStopped = true;
                    agent.velocity = Vector3.zero;
                    agent.acceleration = 0;
                    Debug.Log("StationaryRowing");
                    Debug.Log("1) " + DesiredMachine.name);

                    break;
            }
        }
    }
}    
