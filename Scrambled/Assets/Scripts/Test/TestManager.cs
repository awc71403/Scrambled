using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestManager : MonoBehaviour
{
    List<PlayerManager> m_playerList;
    [SerializeField]
    List<PlayerManager> m_aliveList;

    List<int> m_roundRobin;
    List<int> m_currentRobin;
    bool m_ghostMatched;

    public void Awake() {
        m_playerList = new List<PlayerManager>();
        m_aliveList = new List<PlayerManager>();
        m_ghostMatched = false;

        for (int i = 0; i < GetComponentsInChildren<PlayerManager>().Length; i++) {
            GetComponentsInChildren<PlayerManager>()[i].ID = i;
            m_aliveList.Add(GetComponentsInChildren<PlayerManager>()[i]);
            m_playerList.Add(GetComponentsInChildren<PlayerManager>()[i]);
        }
    }

    public void Matchmake() {
        int[] chosenEnemyID = new int[m_playerList.Count];
        for (int i = 0; i < chosenEnemyID.Length; i++) {
            chosenEnemyID[i] = PlayerManager.NoOpponent;
        }
        int alive = m_aliveList.Count;
        //foreach (PlayerManager player in m_aliveList) {
        if (alive > 4) {
            //While I have no opponnet
            //if (chosenEnemyID[player.ID] == PlayerManager.NoOpponent) {
            foreach (PlayerManager player in m_aliveList) {
                //And we're not doing round robin
                //if (m_aliveList.Count > 4) {
                if (chosenEnemyID[player.ID] == PlayerManager.NoOpponent) {
                    List<PlayerManager> possibleOpponents = new List<PlayerManager>(m_aliveList);
                    int playerIndices = possibleOpponents.Count;
                    int playerAgainstGhost = PlayerManager.DefaultGhost;
                    int chosenGhost;
                    int random;
                    bool odd;
                    //If there are even players or ghost is matched
                    if (m_aliveList.Count % 2 == 0) {
                        //EVEN
                        odd = false;
                    }
                    //Else there are odd players and we need to match a ghost
                    else {
                        //ODD
                        odd = true;
                    }
                    //Find an opponent that you haven't fought and isn't matched
                    //Need to test with int for loop instead of Random.Range
                    bool found = false;
                    //playerIndices != 0
                    while (playerIndices != 0) {
                        if (!odd || m_ghostMatched) {
                            random = Random.Range(0, playerIndices);
                        }
                        else {
                            random = Random.Range(0, playerIndices + 1);
                        }
                        //If you pick the ghost
                        if (random == playerIndices && !player.GetOpponentTracker.Contains(PlayerManager.DefaultGhost)) {
                            //GHOST
                            //Add another while loop incase you picked yourself
                            int opponentID = Random.Range(0, playerIndices - 1);
                            if (player.ID == opponentID) {
                                //If the ghost is not yourself
                                possibleOpponents.RemoveAt(opponentID);
                                playerIndices--;
                                opponentID = Random.Range(0, playerIndices - 1);
                            }
                            playerAgainstGhost = player.ID;
                            chosenGhost = opponentID;
                            chosenEnemyID[player.ID] = PlayerManager.DefaultGhost;
                            m_ghostMatched = true;
                            found = true;
                            Debug.Log($"Player {player.ID} matched with the Ghost of Player {possibleOpponents[opponentID].ID}.");
                            break;
                        }
                        if (chosenEnemyID[possibleOpponents[random].ID] == PlayerManager.NoOpponent && !player.GetOpponentTracker.Contains(possibleOpponents[random].ID) && player.ID != possibleOpponents[random].ID) {
                            //If the person you chose does not have an opponent and you have not fought him in X turns and if your opponent is not yourself
                            chosenEnemyID[player.ID] = possibleOpponents[random].ID;
                            chosenEnemyID[possibleOpponents[random].ID] = player.ID;
                            Debug.Log($"Player {player.ID} matched with Player {chosenEnemyID[player.ID]}.");
                            Debug.Log($"Player {possibleOpponents[random].ID} matched with Player {chosenEnemyID[possibleOpponents[random].ID]}.");
                            found = true;
                            break;
                        }
                        else {
                            possibleOpponents.RemoveAt(random);
                            playerIndices--;
                        }
                    }
                    if (!found) {
                        PlayerManager fix = player;
                        possibleOpponents = new List<PlayerManager>(m_aliveList);
                        playerIndices = possibleOpponents.Count;
                        Debug.LogError("No matching at all");
                        //playerIndices != 0
                        while (playerIndices != 0) {
                            if (!odd) {
                                random = Random.Range(0, playerIndices);
                            }
                            else {
                                random = Random.Range(0, playerIndices + 1);
                            }
                            if (random == playerIndices && !fix.GetOpponentTracker.Contains(PlayerManager.DefaultGhost)) {
                                int opponentID = Random.Range(0, playerIndices - 1);
                                if (fix.ID == opponentID) {
                                    possibleOpponents.RemoveAt(opponentID);
                                    playerIndices--;
                                    opponentID = Random.Range(0, playerIndices - 1);
                                }
                                chosenEnemyID[fix.ID] = PlayerManager.DefaultGhost;
                                chosenGhost = opponentID;
                                int nextFix = playerAgainstGhost;
                                playerAgainstGhost = fix.ID;

                                if (nextFix == PlayerManager.DefaultGhost) {
                                    break;
                                }

                                possibleOpponents = new List<PlayerManager>(m_aliveList);
                                playerIndices = possibleOpponents.Count;
                                Debug.Log($"NextFix: {nextFix}");
                                fix = m_playerList[nextFix];
                                break;
                            }
                            else if (random == playerIndices) {
                                continue;
                            }

                            Debug.Log($"Random: {random} and PlayerIndices: {playerIndices}");
                            if (!fix.GetOpponentTracker.Contains(possibleOpponents[random].ID) && fix.ID != possibleOpponents[random].ID) {
                                chosenEnemyID[fix.ID] = possibleOpponents[random].ID;
                                int nextFix = chosenEnemyID[possibleOpponents[random].ID];
                                chosenEnemyID[possibleOpponents[random].ID] = fix.ID;

                                if (nextFix == PlayerManager.NoOpponent) {
                                    Debug.LogError("We have fixed it!");
                                    break;
                                }

                                if (nextFix == PlayerManager.DefaultGhost) {
                                    nextFix = playerAgainstGhost;

                                    chosenGhost = possibleOpponents[random].ID;
                                    playerAgainstGhost = fix.ID;
                                }

                                possibleOpponents = new List<PlayerManager>(m_aliveList);
                                playerIndices = possibleOpponents.Count;

                                Debug.Log($"NextFix: {nextFix}");
                                if (nextFix == PlayerManager.DefaultGhost) {
                                    break;
                                }

                                fix = m_playerList[nextFix];
                            }
                            else {
                                possibleOpponents.RemoveAt(random);
                                playerIndices--;
                            }
                        }
                    }
                }
            }
        }
        else if (alive == 4) {
            if (m_roundRobin == null) {
                UpdateMatchmakeSystem();
            }

            if (m_currentRobin.Count == 0) {
                m_currentRobin = new List<int>(m_roundRobin);
            }
            if (m_currentRobin[0] == m_aliveList[0].ID) {
                m_currentRobin.RemoveAt(0);
                if (m_currentRobin.Count == 0) {
                    m_currentRobin = new List<int>(m_roundRobin);
                }
            }
            chosenEnemyID[m_aliveList[0].ID] = m_currentRobin[0];
            chosenEnemyID[m_currentRobin[0]] = m_aliveList[0].ID;
            m_currentRobin.RemoveAt(0);

            for (int i = 1; i < alive; i++) {
                if (chosenEnemyID[m_aliveList[i].ID] == PlayerManager.NoOpponent) {
                    for (int j = i + 1; j < alive; j++) {
                        if (chosenEnemyID[m_aliveList[j].ID] == PlayerManager.NoOpponent) {
                            chosenEnemyID[m_aliveList[i].ID] = m_aliveList[j].ID;
                            chosenEnemyID[m_aliveList[j].ID] = m_aliveList[i].ID;
                        }
                    }
                }
            }

        }
        else if (alive == 3) {
            if (m_roundRobin == null) {
                UpdateMatchmakeSystem();
            }
            Debug.Log("Calling Alive = 3");
            List<int> players = new List<int>(m_roundRobin);
            int chosenGhost = PlayerManager.DefaultGhost;
            if (m_currentRobin.Count == 0) {
                m_currentRobin = new List<int>(m_roundRobin);
            }
            Debug.Log($"CurrentRobin: {m_currentRobin[0]} and AliveList: {m_aliveList[0].ID}");
            if (m_currentRobin[0] == m_aliveList[0].ID) {
                //If you are fighting yourself, fight the ghost
                Debug.Log("Ghost Time");
                int random = Random.Range(0, players.Count);
                if (players[random] == m_aliveList[0].ID) {
                    players.RemoveAt(random);
                    random = Random.Range(0, players.Count);
                }
                chosenGhost = random;
                chosenEnemyID[m_aliveList[0].ID] = PlayerManager.DefaultGhost;

                chosenEnemyID[m_aliveList[1].ID] = m_aliveList[2].ID;
                chosenEnemyID[m_aliveList[2].ID] = m_aliveList[1].ID;
            }
            else {
                Debug.Log("Not Ghost Time");
                chosenEnemyID[m_aliveList[0].ID] = m_currentRobin[0];
                chosenEnemyID[m_currentRobin[0]] = m_aliveList[0].ID;

                for (int i = 1; i < alive; i++) {
                    if (chosenEnemyID[m_aliveList[i].ID] == PlayerManager.NoOpponent) {
                        int random = Random.Range(0, players.Count);
                        if (players[random] == m_aliveList[i].ID) {
                            players.RemoveAt(random);
                            random = Random.Range(0, players.Count);
                        }
                        chosenGhost = random;
                        chosenEnemyID[m_aliveList[i].ID] = PlayerManager.DefaultGhost;
                    }
                }
            }
            m_currentRobin.RemoveAt(0);
        }
        else if (alive == 2) {
            Debug.Log("Calling Alive = 2");
            chosenEnemyID[0] = m_aliveList[1].ID;
            chosenEnemyID[1] = m_aliveList[0].ID;
        }
        for (int i = 0; i < chosenEnemyID.Length; i++) {
            if (chosenEnemyID[i] != PlayerManager.NoOpponent) {
                m_playerList[i].SetOpponent(chosenEnemyID[i]);
                Debug.Log($"Player {m_playerList[i].ID}'s TRUE opponent is Player {m_playerList[i].OpponentID}.");
            }
        }

        Debug.Log("--------------------------------------------------------------------------");
    }

    public void KillPlayer() {
        int random = Random.Range(0, m_aliveList.Count);
        m_aliveList[random].gameObject.SetActive(false);
        m_aliveList.RemoveAt(random);
        PlayerManager.OnPlayerDeath();
        foreach (PlayerManager player in m_aliveList) {
            player.TrackerUpdate();
        }
        if (m_aliveList.Count == 4) {
            UpdateMatchmakeSystem();
        }
        else if (m_aliveList.Count == 3) {
            UpdateMatchmakeSystem();
        } 
        Debug.Log($"Player {random} has died");
        Debug.Log("--------------------------------------------------------------------------");
    }

    private void UpdateMatchmakeSystem() {
        Debug.Log("Update Matchmaking");
        List<PlayerManager> players = new List<PlayerManager>(m_aliveList);
        List<int> randomizedPlayerIDs = new List<int>();

        for (int i = 0; i < m_aliveList.Count; i++) {
            int random = Random.Range(0, players.Count);
            randomizedPlayerIDs.Add(players[random].ID);
            players.RemoveAt(random);
        }

        if (m_aliveList.Count == 4) {
            PlayerManager firstPlayer = m_aliveList[0];
            int firstID = randomizedPlayerIDs[0];
            if (firstID == firstPlayer.ID || firstID == firstPlayer.OpponentID) {
                UpdateMatchmakeSystem();
                return;
            }
        }

        m_currentRobin = new List<int>();
        m_roundRobin = randomizedPlayerIDs;
        Debug.Log("The order is:");
        for (int i = 0; i < m_roundRobin.Count; i++) {
            Debug.Log(m_roundRobin[i]);
        }
    }
}
