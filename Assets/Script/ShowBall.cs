using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowBall : NetworkBehaviour
{
    [SerializeField]
    private GameObject ballPrefab;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //[Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    //void CreateBalls(NetworkObject target)
    //{
    //    target.GetComponent<ShowBall>().ballPrefab;
    //}
}
