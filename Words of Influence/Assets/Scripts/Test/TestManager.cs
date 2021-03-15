using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestManager : MonoBehaviour
{
    List<PlayerManager> m_playerList;
    List<PlayerManager> m_aliveList;
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
        foreach (PlayerManager player in m_aliveList) {
            player.OpponentID = PlayerManager.NoOpponent;
        }
        int[] chosenEnemyID = new int[m_playerList.Count];
        for (int i = 0; i < chosenEnemyID.Length; i++) {
            chosenEnemyID[i] = PlayerManager.NoOpponent;
        }
        foreach (PlayerManager player in m_aliveList) {
            Debug.Log($"Player {player.ID}");
            //While I have no opponnet
            if (chosenEnemyID[player.ID] == PlayerManager.NoOpponent) {
                //And we're not doing round robin
                if (m_aliveList.Count > 4) {
                    List<PlayerManager> possibleOpponents = new List<PlayerManager>(m_aliveList);
                    int playerIndices = possibleOpponents.Count;
                    int random;
                    bool odd;
                    //If there are even players or ghost is matched
                    //if (m_aliveList.Count % 2 == 0 || m_ghostMatched) {
                    //EVEN
                    odd = false;
                    //}
                    //Else there are odd players and we need to match a ghost
                    //else {
                    //ODD
                    //Debug.Log("There are odd players");
                    //odd = true;
                    //}
                    //Find an opponent that you haven't fought and isn't matched
                    //Need to test with int for loop instead of Random.Range
                    bool found = false;
                    while (playerIndices != 0) {
                        //if (!odd) {
                        random = Random.Range(0, playerIndices);
                        //} else {
                        //random = Random.Range(0, playerIndices + 1);
                        //}
                        //If you pick the ghost
                        //if (random == playerIndices && !player.GetOpponentTracker.Contains(PlayerManager.GhostID)) {
                        //GHOST
                        //Add another while loop incase you picked yourself
                        //int opponentID = Random.Range(0, playerIndices - 1);
                        //if (player.ID == opponentID) {
                        //If the ghost is not yourself
                        //possibleOpponents.RemoveAt(opponentID);
                        //playerIndices--;
                        //opponentID = Random.Range(0, playerIndices - 1);
                        //}
                        //player.SetGhostOpponent(possibleOpponents[opponentID].ID);
                        //m_ghostMatched = true;
                        //found = true;
                        //Debug.Log($"Player {player.ID}'s opponent is the Ghost of Player {possibleOpponents[opponentID].ID}.");
                        //break;
                        //}
                        if (chosenEnemyID[possibleOpponents[random].ID] == PlayerManager.NoOpponent && !player.GetOpponentTracker.Contains(possibleOpponents[random].ID) && player.ID != possibleOpponents[random].ID) {
                            //If the person you chose does not have an opponent and you have not fought him in X turns and if your opponent is not yourself
                            chosenEnemyID[player.ID] = possibleOpponents[random].ID;
                            chosenEnemyID[possibleOpponents[random].ID] = player.ID;
                            Debug.Log($"Player {player.ID}'s opponent is Player {chosenEnemyID[player.ID]}.");
                            Debug.Log($"Player {possibleOpponents[random].ID}'s opponent is Player {chosenEnemyID[possibleOpponents[random].ID]}.");
                            found = true;
                            break;
                        } else {
                            possibleOpponents.RemoveAt(random);
                            playerIndices--;
                        }
                    }
                    if (!found) {
                        PlayerManager fix = player;
                        possibleOpponents = new List<PlayerManager>(m_aliveList);
                        playerIndices = possibleOpponents.Count;
                        Debug.LogError("No matching at all");
                        odd = false;
                        while (playerIndices != 0) {
                            random = Random.Range(0, playerIndices);

                            if (!fix.GetOpponentTracker.Contains(possibleOpponents[random].ID) && fix.ID != possibleOpponents[random].ID) {
                                chosenEnemyID[fix.ID] = possibleOpponents[random].ID;
                                int nextFix = chosenEnemyID[possibleOpponents[random].ID];
                                chosenEnemyID[possibleOpponents[random].ID] = fix.ID;

                                if (nextFix == PlayerManager.NoOpponent) {
                                    Debug.LogError("We have fixed it!");
                                    break;
                                }

                                possibleOpponents = new List<PlayerManager>(m_aliveList);
                                playerIndices = possibleOpponents.Count;
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
        foreach (PlayerManager player in m_aliveList) {
            player.OnPlayerDeath();
        }
        Debug.Log($"Player {random} has died");
    }
}
