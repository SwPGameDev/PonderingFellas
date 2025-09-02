using UnityEngine;

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

    private void Awake()
    {
        _instance = this;
    }

    [Header("Team Blue")]
    public FellaBehavior.FellaStates blueState;

    [Header("Team Red")]
    public FellaBehavior.FellaStates redState;
}