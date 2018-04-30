using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PowerUp : NetworkBehaviour
{

    CTFGameManager GameManager;

    void Start ()
    {
        GameManager = GameObject.FindGameObjectWithTag("GameManger").GetComponent<CTFGameManager>();
	}

	void Update ()
    {

    }

    [ClientRpc]
    public void RpcPickupPowerUp()
    {
        RpcPickupPowerUp();
    }
    

    public void CmdRpcPickupPower()
    {
        Destroy(gameObject);
    }

    [ClientRpc]
    public void RpcPowerUp(GameObject player)
    {
        PowerUpPower(player);
    }

    public void PowerUpPower(GameObject powerUpObject)
    {
        powerUpObject.GetComponent<PlayerController>().usedPowerUp = true;
        
        if (powerUpObject.GetComponent<PlayerController>().hasFlag)
        {
            powerUpObject.GetComponent<PlayerController>().m_linearSpeed = 3.0f;
            Debug.Log("HAS FLAG");
        }

        else
        {
            
            Debug.Log("NO FLAG");
        }
    }

    void OnTriggerEnter(Collider other)
    {

        if (!isServer || other.tag != "Player")
        {
            return;
        }

        GameManager.powerUpCount--;

        PowerUpPower(other.gameObject);
        RpcPowerUp(other.gameObject);

        CmdRpcPickupPower();
        RpcPickupPowerUp();
    }
}
