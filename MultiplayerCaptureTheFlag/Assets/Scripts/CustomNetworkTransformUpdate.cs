using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TransformMessage : MessageBase
{
    public uint netID;
    public Vector3 position;
    public Quaternion rotation;
}

public class CustomNetworkTransformUpdate : NetworkBehaviour
{

    public float m_updateTransformTime = 0.25f;
    private float m_currentUpdateTransformTime = 0.0f;

    NetworkClient m_client;
    
    void Start () {
        m_client = isServer ? null : NetworkManager.singleton.client;
        
        if (!isLocalPlayer)
        {
            Debug.Log("Register Handler Netid: " + this.netId.Value);
            if (isServer)
            {
               // NetworkServer.RegisterHandler(CustomMsgType.Transform, OnTransformReceived);
            }
            else
            {
               // m_client.RegisterHandler(CustomMsgType.Transform, OnTransformReceived);
            }
        }
    }
	
	void Update () {
        m_currentUpdateTransformTime += Time.deltaTime;
        if (m_currentUpdateTransformTime > m_updateTransformTime)
        {
           
            TransformMessage Message = new TransformMessage();
            Message.position = this.transform.position;
            Message.rotation = this.transform.rotation;
            Message.netID = this.netId.Value;

            if (isServer)
            {
              //  NetworkServer.SendToAll(CustomMsgType.Transform, Message);
            }
            else
            {
               // m_client.Send(CustomMsgType.Transform, Message);
            }
            m_currentUpdateTransformTime = 0.0f;
        }
    }

    public void OnTransformReceived(NetworkMessage networkMessage)
    {
        TransformMessage message = networkMessage.ReadMessage<TransformMessage>();

        if (message.netID == this.netId.Value)
        {
            this.transform.position = message.position;
            this.transform.rotation = message.rotation;
        }
    }
}
