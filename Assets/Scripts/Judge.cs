using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Judge : MonoBehaviour
{
    [SerializeField] private GameObject[] MessageObj;
    [SerializeField] NotesManager notesManager;

    [SerializeField] bool JudgeSlowAndFast;

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

        // ロングノーツの判定
        if (Input.GetKey(KeyCode.LeftShift)) JudgeLongNotes(0);
        if (Input.GetKey(KeyCode.S)) JudgeLongNotes(1);
        if (Input.GetKey(KeyCode.D)) JudgeLongNotes(2);
        if (Input.GetKey(KeyCode.F)) JudgeLongNotes(3);
        if (Input.GetKey(KeyCode.Equals)) JudgeLongNotes(4);
        if (Input.GetKey(KeyCode.Semicolon)) JudgeLongNotes(5);
        if (Input.GetKey(KeyCode.RightBracket)) JudgeLongNotes(6);
        if (Input.GetKey(KeyCode.RightShift)) JudgeLongNotes(7);
       
        // ノーツを叩くタイミングから一定時間経過したらミス判定
        if (notesManager.NotesObj[0].Length == 1) {
            if (EntireManager.instance.Start && Time.time > notesManager.NotesTime[0][0] + 0.1 + EntireManager.instance.StartTime) {
                message(3, 0);
                deleteData(0);
                EntireManager.instance.Miss++;
                EntireManager.instance.combo = 0;
            }
        } 
        
        if (notesManager.NotesObj[0].Length == 3) {
            if (EntireManager.instance.Start && Time.time > notesManager.NotesTime[0][0] + 0.1 + EntireManager.instance.StartTime) {
                message(3, 0);
                EntireManager.instance.Miss++;
                EntireManager.instance.combo = 0;
                notesManager.NotesObj[0][0].SetActive(false);
            }

            if (EntireManager.instance.Start && Time.time > notesManager.NotesTime[0][1] + 0.1 + EntireManager.instance.StartTime) {
                Debug.Log("aaa");
                message(3, 0);
                EntireManager.instance.Miss++;
                EntireManager.instance.combo = 0;
                notesManager.NotesTime.RemoveAt(0);
                notesManager.LaneNum.RemoveAt(0);
                notesManager.NoteType.RemoveAt(0);
                notesManager.NotesObj[0][2].SetActive(false);
                notesManager.NotesObj.RemoveAt(0);
            }
        } 
    }

    void JudgeNotes(int index) {
        for (int i = 0; i < 6; i++) {
            float timeLag = Time.time - (notesManager.NotesTime[i][0] + EntireManager.instance.StartTime);
            float timeLagABS = Mathf.Abs(timeLag);
               
            if (timeLagABS <= 0.10 && notesManager.LaneNum[i] == index) {
                if (JudgeSlowAndFast) {
                    if (timeLag < 0) {
                        Debug.Log("fast");
                    } else {
                        Debug.Log("slow");
                    }
                }

                if (timeLagABS <= 0.033) {
                    message(0, i);
                    EntireManager.instance.CBreak++;
                } else if (timeLagABS <= 0.066) {
                    message(1, i);
                    EntireManager.instance.Break++;
                } else {
                    message(2, i);
                    EntireManager.instance.Hit++;
                }
                EntireManager.instance.combo++;
                deleteData(i);
                break;
            }
        }
    }

    void JudgeLongNotes(int index) {
        for (int i = 0; i < 6; i++) {
            if (notesManager.LaneNum[i] == index && 
                Time.time >= notesManager.NotesTime[i][0] + EntireManager.instance.StartTime && 
                Time.time <= notesManager.NotesTime[i][1] + EntireManager.instance.StartTime) {
                Debug.Log("changed Layer of note" + i);
                notesManager.NotesObj[i][1].layer = LayerMask.NameToLayer("LongNote");
                break;
            } 
        }
    }

    void deleteData(int i) {
        notesManager.NotesTime.RemoveAt(i);
        notesManager.LaneNum.RemoveAt(i);
        notesManager.NoteType.RemoveAt(i);
        notesManager.NotesObj[i][0].SetActive(false); 
        notesManager.NotesObj.RemoveAt(i);
    }

    void message(int judge, int i) {
        Instantiate(MessageObj[judge], new Vector3(notesManager.LaneNum[i] - 3.56f, 0.76f, 0.15f), Quaternion.Euler(45, 0, 0));
    }
}
