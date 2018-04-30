using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CTFGameManager : NetworkBehaviour
{

    public int m_numPlayers = 2;
    [SyncVar]
    public float m_gameTime = 60.0f;

    [SyncVar]
    public float powerUpCount = 0.0f;

    [SyncVar]
    public bool gameOver = false;

    public GameObject m_flag = null;
    public GameObject powerUpPrefab = null;

    public Text timerText;
    public GameObject GameOver;
    public Text t_winner;
    public Text t_highscore;

    private string playerName = "";
    private int highscore = 0;

    public enum CurrentGameState
    {
        LobbyState,
        StartState,
        InGameState,
        GameOverState
    }

    [SyncVar]
    CurrentGameState currentState = CurrentGameState.LobbyState;

    public bool SpawnFlag()
    {
        GameObject flag = Instantiate(m_flag, new Vector3(0, 3, 8.34f), new Quaternion());
        NetworkServer.Spawn(flag);
        return true;
    }

    public bool MaxPlayersReached()
    {
        return NetworkManager.singleton.numPlayers == m_numPlayers;
    }

	void Update ()
    {
        timerText.text = m_gameTime.ToString();

	    if(isServer)
        {
            if (currentState == CurrentGameState.LobbyState && MaxPlayersReached())
            {
                currentState = CurrentGameState.StartState;
            }
        }

        GameOver.SetActive(gameOver);

        UpdateGameState();
        UpdateInGame();
        UpdateEndGame();
    }

    public void SpawnPowerUp()
    {
        if(isServer)
        {
            if (powerUpCount <= 1)
            {
                powerUpCount++;

                var spawnPosition = new Vector3(Random.Range(-18.0f, 18.0f), 0.5f, Random.Range(-6.0f, 18.0f));

                var spawnRotation = Quaternion.Euler(0.0f, 0.0f,0.0f);

                var powerUp = (GameObject)Instantiate(powerUpPrefab, spawnPosition, spawnRotation);
                NetworkServer.Spawn(powerUp);
            }
        }
    }

    public void UpdateGameState()
    {
        if (currentState == CurrentGameState.StartState)
        {
            if (isServer)
            {
                SpawnPowerUp();

                SpawnFlag();

                currentState = CurrentGameState.InGameState;
            }
        }
    }

    public void UpdateInGame()
    {
        if (currentState == CurrentGameState.InGameState)
        {
            if (isServer)
            {
                SpawnPowerUp();

                m_gameTime -= Time.deltaTime;

                if (m_gameTime <= 0)
                {
                    m_gameTime = 0;
                    currentState = CurrentGameState.GameOverState;
                }
            }
        }
    }

    public void UpdateEndGame()
    {
        if (currentState == CurrentGameState.GameOverState)
        {
            gameOver = true;

            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

            foreach (GameObject p in players)
            {
                if (p.GetComponent<Score>().m_score > highscore)
                {
                    playerName = "Winner: Player Number - " + p.GetComponent<PlayerController>().playerId;
                    highscore = p.GetComponent<Score>().m_score;
                }
                p.GetComponent<PlayerController>().stunned = true;
            }

            GameObject flag = GameObject.FindGameObjectWithTag("Flag");
            flag.GetComponent<Flag>().frozen = true;

            t_winner.text = playerName;
        }
    }

    public void PlayAgain()
    {
        if(isServer)
        {
            gameOver = false;

            NetworkServer.Destroy(GameObject.FindGameObjectWithTag("Flag"));

            m_gameTime = 60.0f;
            currentState = CurrentGameState.LobbyState;

            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

            foreach (GameObject p in players)
            {
                p.GetComponent<PlayerController>().stunned = false;
                p.GetComponent<Score>().m_score = 0;
            }
        }
    }
}
