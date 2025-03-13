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
    public GameObject teste;
    private Vector3 initialPosition;
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
        initialPosition = transform.position;
        Debug.Log("Pos inicil do NPC: " + initialPosition);
        MakeDestination();
    }

    void Update()
    {
        /*if(agent.hasPath == false && agent.isStopped == false)
        {
            ObjectToGo = Random.Range(0, GymMachines.Count);
            Debug.Log("int random: " + ObjectToGo);
            agent.SetDestination(GymMachines[ObjectToGo].transform.position);
            DesiredMachine = GymMachines[ObjectToGo];
            
        }*/

    }

    private void MakeDestination()
    {
        ObjectToGo = Random.Range(0, GymMachines.Count);
        Debug.Log("int random: " + ObjectToGo);
        agent.SetDestination(GymMachines[ObjectToGo].transform.position);
        DesiredMachine = GymMachines[ObjectToGo];
        GetChildInt(DesiredMachine);
    }

    private void GetChildInt(GameObject parent)
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
            animator.SetBool("ReachedDestination", true);
            switch (DesiredMachine.tag)
            { 
                case "Treadmill":
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
                    agent.isStopped = true;
                    agent.velocity = Vector3.zero;
                    agent.acceleration = 0;
                    Debug.Log("Bike");
                    Debug.Log("1) " + DesiredMachine.name);

                    break;
                case "BackMachine":
                    agent.isStopped = true;
                    agent.velocity = Vector3.zero;
                    agent.acceleration = 0;
                    Debug.Log("BackMachine");
                    Debug.Log("1) " + DesiredMachine.name);

                    break;
                case "BackMachine2":
                    agent.isStopped = true;
                    agent.velocity = Vector3.zero;
                    agent.acceleration = 0;
                    Debug.Log("BackMachine2");
                    Debug.Log("1) " + DesiredMachine.name);

                    break;
                case "StationaryRowing":
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
