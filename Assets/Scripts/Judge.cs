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
        if (Input.GetKeyDown(KeyCode.LeftShift)) JudgeNotes(0);
        if (Input.GetKeyDown(KeyCode.S)) JudgeNotes(1);
        if (Input.GetKeyDown(KeyCode.D)) JudgeNotes(2);
        if (Input.GetKeyDown(KeyCode.F)) JudgeNotes(3);
        if (Input.GetKeyDown(KeyCode.Equals)) JudgeNotes(4);
        if (Input.GetKeyDown(KeyCode.Semicolon)) JudgeNotes(5);
        if (Input.GetKeyDown(KeyCode.RightBracket)) JudgeNotes(6);
        if (Input.GetKeyDown(KeyCode.RightShift)) JudgeNotes(7);
       
        // ノーツを叩くタイミングから一定時間経過したらミス判定
        if (Time.time > notesManager.NotesTime[0] + 0.1 + EntireManager.instance.StartTime) {
            message(3, 0);
            deleteData(0);
            EntireManager.instance.Miss++;
            EntireManager.instance.combo = 0;
            // Debug.Log("Miss");
        }
    }

    void JudgeNotes(int index) {
        float timeLagg = 0.0f;
        for (int i = 0; i < 6; i++) {
            timeLagg = Time.time - (notesManager.NotesTime[i] + EntireManager.instance.StartTime);
            float timeLag = Mathf.Abs(timeLagg);
            
            if (timeLag <= 0.10 && notesManager.LaneNum[i] == index) {
                if (timeLag <= 0.033) {
                    // Debug.Log("Critical Break");
                    message(0, i);
                    EntireManager.instance.CBreak++;
                } else if (timeLag <= 0.066) {
                    // Debug.Log("Break");
                    message(1, i);
                    EntireManager.instance.Break++;
                } else {
                    // Debug.Log("Hit");
                    message(2, i);
                    EntireManager.instance.Hit++;
                }
                EntireManager.instance.combo++;
                deleteData(i);
                break;
            }
        }
        if (timeLagg < 0) {
            Debug.Log("fast");
        } else {
            Debug.Log("slow");
        }
    }

    void deleteData(int i) {
        notesManager.NotesTime.RemoveAt(i);
        notesManager.LaneNum.RemoveAt(i);
        notesManager.NoteType.RemoveAt(i);
        notesManager.NotesObj.RemoveAt(i);
    }

    void message(int judge, int i) {
        Instantiate(MessageObj[judge], new Vector3(notesManager.LaneNum[i] - 3.56f, 0.76f, 0.15f), Quaternion.Euler(45, 0, 0));
    }
}
