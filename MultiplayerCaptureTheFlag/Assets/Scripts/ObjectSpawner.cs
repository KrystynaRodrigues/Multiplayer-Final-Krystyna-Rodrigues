using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.Networking;

public class ObjectSpawner : NetworkBehaviour {

    public GameObject playerPrefab = null;
    public GameObject flagPrefab = null;
    public Text timerText = null;

    public float range = 10.0f;

    public static bool SpawnPoint(Vector3 center, float range, out Vector3 result)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector2 randomSpawn = Random.insideUnitCircle;
            Vector3 randomSpawnPoint = center + new Vector3(randomSpawn.x,randomSpawn.y,center.z) * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomSpawnPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }
        result = Vector3.zero;
        return false;
    }

    static public bool SpawnObject(GameObject objectToSpawn, float range)
    {
        Vector3 spawnPoint;
        if (SpawnPoint(new Vector3(0.0f, 0.0f, 0.0f), range, out spawnPoint))
        {
            Quaternion rotation = objectToSpawn.transform.rotation;
            GameObject clone = (GameObject)Instantiate(objectToSpawn, spawnPoint, rotation);
            return true;
        }
        
        Debug.Log("Could not find point to spawn");
        return false;
    }

}
