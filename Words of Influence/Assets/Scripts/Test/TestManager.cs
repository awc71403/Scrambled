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
        Debug.Log("Matchmaking called");
        foreach (PlayerManager player in m_aliveList) {
            //While I have no opponnet
            if (player.OpponentID == PlayerManager.NoOpponent) {
                //And we're not doing round robin
                if (m_aliveList.Count > 4) {
                    int playerIndices;
                    int random;
                    //If there are even players or ghost is matched
                    //if (m_aliveList.Count % 2 == 0 || m_ghostMatched) {
                        //EVEN
                        playerIndices = m_aliveList.Count;
                    //}
                    //Else there are odd players and we need to match a ghost
                    //else {
                        //ODD
                        //playerIndices = m_aliveList.Count + 1;
                    //}
                    //Find an opponent that you haven't fought and isn't matched
                    //Need to test with int for loop instead of Random.Range
                    //while (true) {
                    bool found = false;
                    for (int i = 0; i < playerIndices; i ++) {
                        //random = Random.Range(0, playerIndices);
                        random = i;
                        //If you pick the ghost
                        //if (!m_ghostMatched && random == playerIndices && !player.GetOpponentTracker.Contains(PlayerManager.GhostID)) {
                            //GHOST
                            //int opponentID = Random.Range(0, playerIndices - 1);
                            //if (player.ID != opponentID) {
                                //If the ghost is not yourself
                                //player.SetOpponent(PlayerManager.GhostID);
                                //m_ghostMatched = true;
                                //Debug.Log($"Player {player.ID}'s opponent is the Ghost.");
                                //break;
                            //}
                        //}
                        if (m_playerList[m_aliveList[random].ID].OpponentID == PlayerManager.NoOpponent && !player.GetOpponentTracker.Contains(m_aliveList[random].ID) && player.ID != random) {
                            //If the person you chose does not have an opponent and you have not fought him in X turns and if your opponent is not yourself
                            player.SetOpponent(random);
                            m_aliveList[random].SetOpponent(player.ID);
                            Debug.Log($"Player {player.ID}'s opponent is Player {player.OpponentID}.");
                            Debug.Log($"Player {random}'s opponent is Player {m_aliveList[random].OpponentID}.");
                            found = true;
                            break;
                        }
                    }
                    if (!found) {
                        Debug.Log("No matching at all");
                    }
                }
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
