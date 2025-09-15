using System.Collections.Generic;
using UnityEngine;

public class Squad : MonoBehaviour
{
    public List<GameObject> SquadMembers {  get; private set; }

    public void AddMember(GameObject go)
    {
        SquadMembers.Add(go);
    }

    public void RemoveMember(GameObject go)
    {
        SquadMembers.Remove(go);
    }
}
