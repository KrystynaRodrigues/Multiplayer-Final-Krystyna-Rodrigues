using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour
{
    public GameObject spikeTrapPrefab;
    public Transform spikeTrapSpawn;
    public GameObject flag;

    private GameObject flagObject;

    [SyncVar]
    public int playerId;

    [SyncVar]
    public bool usedPowerUp = false;

    [SyncVar]
    public bool hasFlag = false;

    [SyncVar]
    public bool getSpikeTrap = false;

    [SyncVar]
    public bool stunned = false;

    private float powerUpTimer = 5.0f;
    public float stunTimer = 0.0f;
    private bool startTimer = false;

    public float m_linearSpeed;
    public float m_jumpSpeed;

    private Rigidbody rigidBody = null;

    NetworkIdentity netIdentity;

    public Text playerNumber;

    

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        m_linearSpeed = 0.5f;
        m_jumpSpeed = 5.0f;

        StartCoroutine(Delay());

        if (!isServer)
        {
            return;
        }

        netIdentity = GetComponent<NetworkIdentity>();
        playerId = int.Parse(netIdentity.netId.ToString()) - 1;
    }

    public void Jump()
    {
        Vector3 jumpVelocity = Vector3.up * m_jumpSpeed;
        rigidBody.velocity += jumpVelocity;

    }

    [ClientRpc]
    public void RpcJump()
    {
        Jump();
    }

    [Command]
    public void CmdJump()
    {
        Jump();
        RpcJump();
    }

    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        if(usedPowerUp && powerUpTimer >= 0)
        {
            powerUpTimer -= Time.deltaTime;
        }
        else
        {
            usedPowerUp = false;
            powerUpTimer = 5.0f;
            m_linearSpeed = 1.0f;
        }

        var x = 0.0f;
        var z = 0.0f;

        if (!stunned)
        {
            x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
            z = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f * m_linearSpeed;
        }

        transform.Rotate(0, x, 0);
        transform.Translate(0, 0, z);
        

        if (Input.GetKeyDown(KeyCode.Space))
        {
            CmdJump();
        }

        if (Input.GetKeyDown(KeyCode.Q) && usedPowerUp == true && hasFlag == false)
        {
            CmdSpawnTrap();
        }
    }

	[Command]
    void CmdSpawnTrap()
    { 
        var spikeTrap = (GameObject)Instantiate(spikeTrapPrefab, spikeTrapSpawn.position, spikeTrapSpawn.rotation);
        spikeTrap.GetComponent<Rigidbody>().velocity = spikeTrap.transform.forward * 1;
        NetworkServer.Spawn(spikeTrap);
   
    }

    public override void OnStartLocalPlayer ()
    {
        GetComponent<MeshRenderer>().material.color = Color.blue;
    }

    void OnCollisionEnter(Collision col)
    {

        if(col.collider.tag == "Player")
        {
            stunned = true;
            StunPlayer();
        }

        if(col.collider.tag == "SpikeTrap")
        {
            stunned = true;
            StunPlayer();
        }
    }

    void StunPlayer()
    {
        if(stunned == true)
        {
            stunned = false;
            m_linearSpeed = 0.5f;
            Delay();
            DropFlag();

            Debug.Log("stunned");
        }
           
     }

    public void DropFlag()
    {
        if (!isServer)
        {
            return;
        }

        if (hasFlag == true)
        {
            flagObject = GameObject.FindGameObjectWithTag("Flag");
            NetworkServer.Destroy(flagObject);

            GameObject flagSpawn = Instantiate(flag, new Vector3(0, 3, 8.34f), new Quaternion());
            NetworkServer.Spawn(flagSpawn);
        

            hasFlag = false;
        }
    }


    IEnumerator Delay()
    {
        yield return new WaitForSeconds(5);
    }

}     

