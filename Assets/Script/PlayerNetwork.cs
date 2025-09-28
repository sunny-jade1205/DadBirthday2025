using Fusion;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
    [Networked]
    public Quaternion Rotation { get; set; }

    //[Networked(OnChanged = nameof(OnPlayerIndexChanged))]
    //public int playerIndex { get; set; }

    //本地
    [SerializeField]
    private Material allMaterial;
    [SerializeField]
    private List<Renderer> renderers = new List<Renderer>();
    //[SerializeField]
    //private Transform cameraPosition;
    [SerializeField]
    private bool isSpringJoint;
    [SerializeField]
    private Rigidbody mouthRigid;
    [SerializeField]
    private Camera playerCamera;

    private void Start()
    {
        if (Object.HasStateAuthority)
        {
            //playerCamera.transform.SetParent(cameraPivot);
            //playerCamera.transform.position = cameraPosition.position;
            //playerCamera.transform.rotation = cameraPosition.rotation;
            isSpringJoint = false;
        }

    }
    private void Update()
    {
        if (Object.HasInputAuthority && Input.GetMouseButton(0))
        {
            Vector3 mouse = Input.mousePosition;
            mouse.z = -playerCamera.transform.position.z;
            Ray ray = playerCamera.ScreenPointToRay(mouse);
            Debug.DrawRay(ray.origin, ray.direction * 0.35f, Color.yellow);

            RaycastHit[] hit3D = Physics.RaycastAll(ray, 0.35f);
            if (hit3D.Length > 0 && !isSpringJoint)
            {
                Debug.Log("撞到好多" + hit3D[0].collider.name);
                if (hit3D[0].collider.tag == "PlayerButton")
                {
                    isSpringJoint = true;
                    playerForceRPC(Object);
                }
                else
                {
                    isSpringJoint = false;
                }
            }
            if (hit3D.Length == 0)
            {
                isSpringJoint = false;
            }
        }
        if (Object.HasInputAuthority && !Input.GetMouseButton(0))
        {
            isSpringJoint = false;
        }
    }
    public override void FixedUpdateNetwork()
    {
        if (Object.HasStateAuthority)
        {
            Rotation = transform.rotation;
        }
        else
        {
            transform.rotation = Rotation;
        }
    }
    public override void Spawned()
    {
        base.Spawned();
        UpdateMaterial();
        if (Object.HasInputAuthority) // 或者是本地端判斷依遊戲框架
        {
            playerCamera.gameObject.SetActive(true);
        }
        else
        {
            playerCamera.gameObject.SetActive(false);
        }

    }

    //[Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    //public void RpcRequestChangeMaterial(int newIndex)
    //{
    //    if (Object.HasStateAuthority)
    //    {
    //        playerIndex = newIndex;
    //        UpdateMaterial();
    //    }
    //}
    //private static void OnPlayerIndexChanged(Changed<PlayerNetwork> changed)
    //{
    //    changed.Behaviour.UpdateMaterial();
    //}

    private void UpdateMaterial()
    {
        //switch (playerIndex)
        //{
        //    case 0: allMaterial = Resources.Load<Material>("P1"); break;
        //    case 1: allMaterial = Resources.Load<Material>("P2"); break;
        //    case 2: allMaterial = Resources.Load<Material>("P3"); break;
        //    case 3: allMaterial = Resources.Load<Material>("P4"); break;
        //    case 4: allMaterial = Resources.Load<Material>("P5"); break;
        //}
        foreach (var renderer in renderers)
        {
            renderer.material = allMaterial;
        }
    }
    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    void playerForceRPC(NetworkObject target)
    {
        target.GetComponent<PlayerNetwork>().mouthRigid.AddRelativeForce(new Vector3(0, 75, 100));
    }
}
