using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;

public class WordManager : MonoBehaviour
{
    private static Dictionary<string,string> m_wordDictionary;

    private static WordManager m_singleton;

    void Start()
    {
        if (m_singleton) {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        m_singleton = this;

        // Load in dictionary
        TextAsset textFile = Resources.Load("dictionary") as TextAsset;
        m_wordDictionary = textFile.text.Split("\n"[0]).ToDictionary(v => v);
    }

    public static bool IsWord(string word)
    {
        word = word.ToUpper();
        word += Convert.ToChar(13);
        if (m_wordDictionary.ContainsKey(word)) {
            Debug.Log($"'{word}' is a word!");
            return true;
        }
        return false;
    }
}
