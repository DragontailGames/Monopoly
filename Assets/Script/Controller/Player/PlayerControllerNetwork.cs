using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerControllerNetwork : NetworkBehaviour 
{
    private void Start()
    {

    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        Debug.Log("Teste " + ComponentIndex);
    }
}
