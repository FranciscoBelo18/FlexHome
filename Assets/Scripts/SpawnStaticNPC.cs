using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using System.Collections.Generic;

public class SpawnStaticNPC : MonoBehaviour
{
    private GameObject[] MachinesAvailables;
    public string[] MachinesTags;
    private List<GameObject> GymMachines; 
    private GameObject DesiredMachine = null;
    private int ChildInt;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (string tag in MachinesTags)
        {
            if(tag == "Treadmill"){
                MachinesAvailables = GameObject.FindGameObjectsWithTag(tag);
                GymMachines.AddRange(MachinesAvailables);
            }     
        }
        SelectMachineToSpawn();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SelectMachineToSpawn()
    {
        int ObjectToGo = Random.Range(0, GymMachines.Count);
        Debug.Log("int random: " + ObjectToGo);
        DesiredMachine = GymMachines[ObjectToGo];
        if(VerifySpawnerState(DesiredMachine))
        {   
            Debug.Log("Maquina escolhida: " + GymMachines[ObjectToGo].name);
            transform.position = DesiredMachine.transform.GetChild(ChildInt).position;
            DesiredMachine.transform.GetChild(ChildInt).gameObject.SetActive(false);
        }
        else
        {
            SelectMachineToSpawn();
        }
    }

    private bool VerifySpawnerState(GameObject machine)
    {
        for(int i = 0; i < machine.transform.childCount; i++)
        {
            if(machine.transform.GetChild(i).tag == "Spawner")
            {
                return machine.transform.GetChild(i).gameObject.activeSelf;
            }
        }
        return false;
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
}
