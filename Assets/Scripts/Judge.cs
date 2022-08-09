using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Judge : MonoBehaviour
{
    [SerializeField] private GameObject[] MessageObj;
    [SerializeField] NotesManager notesManager;

    // Update is called once per frame
    void Update()
    {
        // 次に来るノーツに反応してる？
        if (
            (Input.GetKeyDown(KeyCode.LeftShift) && notesManager.LaneNum[0] == 0) ||
            (Input.GetKeyDown(KeyCode.S) && notesManager.LaneNum[0] == 1) ||
            (Input.GetKeyDown(KeyCode.D) && notesManager.LaneNum[0] == 2) ||
            (Input.GetKeyDown(KeyCode.F) && notesManager.LaneNum[0] == 3) ||
            (Input.GetKeyDown(KeyCode.Equals) && notesManager.LaneNum[0] == 4) ||
            (Input.GetKeyDown(KeyCode.Semicolon) && notesManager.LaneNum[0] == 5) ||
            (Input.GetKeyDown(KeyCode.RightBracket) && notesManager.LaneNum[0] == 6) ||
            (Input.GetKeyDown(KeyCode.RightShift) && notesManager.LaneNum[0] == 7) 
        ) {
            Judgement(Mathf.Abs(Time.time - (notesManager.NotesTime[0] + EntireManager.instance.StartTime)));
        }
        
        // ノーツを叩くタイミングから一定時間経過したらミス判定
        if (Time.time > notesManager.NotesTime[0] + 0.1 + EntireManager.instance.StartTime) {
            message(3);
            deleteData();
            EntireManager.instance.Miss++;
            EntireManager.instance.combo = 0;
            // Debug.Log("Miss");
        }
    }

    void Judgement(float timeLag) {
        // Debug.Log(timeLag);
        // 判定表示
        if (timeLag <= 0.033) {
            // Debug.Log("Critical Break");
            message(0);
            EntireManager.instance.CBreak++;
            EntireManager.instance.combo++;
            deleteData();
        } else if (timeLag <= 0.066) {
            // Debug.Log("Break");
            message(1);
            EntireManager.instance.Break++;
            EntireManager.instance.combo++;
            deleteData();
        } else if (timeLag <= 0.10) {
            // Debug.Log("Hit");
            message(2);
            EntireManager.instance.Hit++;
            EntireManager.instance.combo++;
            deleteData();
        }
    }

    float GetABS(float num) {
        if (num < 0) return -num;
        return num;
    }

    void deleteData() {
        notesManager.NotesTime.RemoveAt(0);
        notesManager.LaneNum.RemoveAt(0);
        notesManager.NoteType.RemoveAt(0);
        notesManager.NotesObj.RemoveAt(0);
    }

    void message(int judge) {
        Instantiate(MessageObj[judge], new Vector3(notesManager.LaneNum[0] - 3.56f, 0.76f, 0.15f), Quaternion.Euler(45, 0, 0));
    }
}
