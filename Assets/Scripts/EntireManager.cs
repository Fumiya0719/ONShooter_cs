using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntireManager : MonoBehaviour
{
    public static EntireManager instance = null;

    public int songID;
    public float HighSpeed;

    public bool Start;
    public float StartTime;

    public int combo;
    public int score;

    public int CBreak;
    public int Break;
    public int Hit;
    public int Miss;

    void Awake()
    {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        } else {
            Destroy(this.gameObject);
        }
    }
}
