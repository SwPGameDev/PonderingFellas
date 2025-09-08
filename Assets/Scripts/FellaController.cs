using UnityEngine;
using System.Collections.Generic;
using NUnit.Framework;

public class FellaController : MonoBehaviour
{
    private static FellaController _instance;

    public static FellaController Instance
    {
        get
        {
            if (_instance == null)
                Debug.LogError("FellaController is null");
            return _instance;
        }
    }

    public List<GameObject> blueFellas;
    public List<GameObject> redFellas;


    public bool controlBoth;
    public FellaBehavior.FellaStates bothState;

    [Header("Team Blue")]
    public FellaBehavior.FellaStates blueState;

    [Header("Team Red")]
    public FellaBehavior.FellaStates redState;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        GameObject[] goHolder = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        foreach (GameObject go in goHolder)
        {
            if (go != null && go.TryGetComponent<FellaBehavior>(out FellaBehavior behavior))
            {
                if (behavior.currentTeam == Team.TeamID.Blue)
                {
                    blueFellas.Add(go);
                }
                else if (behavior.currentTeam == Team.TeamID.Red)
                {
                    redFellas.Add(go);
                }
            }
        }
    }

    private void Update()
    {
        if (controlBoth)
        {
            blueState = bothState;
            redState = bothState;
        }
    }
}