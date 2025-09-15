using UnityEngine;
using System.Collections.Generic;

public class SquadManager : MonoBehaviour
{
    public List<Squad> SquadsList;

    public void CreateSquad()
    {
        GameObject createdSquad = new GameObject();
        SquadsList.Add(createdSquad.AddComponent<Squad>());
    }
}
