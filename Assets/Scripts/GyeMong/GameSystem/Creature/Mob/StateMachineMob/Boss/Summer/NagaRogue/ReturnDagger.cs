using UnityEngine;

public class ReturnDagger : MonoBehaviour
{
    [SerializeField] private LineRenderer ropeLine;
    private Transform _bossTr;

    public void Initiate(Transform bossTr)
    {
        _bossTr = bossTr;
    }
    
    // void Update()
    // {
    //     if (ropeLine.enabled)
    //     {
    //         ropeLine.SetPosition(0, _bossTr.position);
    //         ropeLine.SetPosition(1, transform.position);
    //     }
    // }
}
