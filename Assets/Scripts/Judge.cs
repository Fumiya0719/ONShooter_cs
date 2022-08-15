using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Judge : MonoBehaviour
{
    [SerializeField] private GameObject[] MessageObj;
    [SerializeField] NotesManager notesManager;

    [SerializeField] bool JudgeSlowAndFast;

    /*
    メモ：
    判定にロングノーツが含まれると流れ切ったノーツを逐一消去していると処理が複雑or不可能になる
    →リストから消去は行わず、オブジェクトをInactiveするのみの処理を行う
    全ノーツのうち、判定部分(判定ライン前後0.1sec)に入ったノーツのリストインデックスを別の配列等にキューとして保存し、キューのうち対象キーが押された(ロングノーツの場合長押し)していた場合判定を行う。
    キューのうち、判定ライン到達から0.1秒たったノーツのInactiveにする。
    そのノーツがロングノーツの場合、終点が0.1秒経った時にすべてInactiveにする。
    */

    private int noteCount = 0;
    private float judgeTimeABS = 0.1f;
    private List<int> noteQueue = new List<int>();
    void Update()
    {
        if (EntireManager.instance.Start) {
            // 判定ライン-0.15秒に到達したノーツを判定キューに挿入
            // (Hit判定は±0.1秒だが処理によって若干の遅延が起きる可能性があるので少し時間を取る)
            if (Time.time >= EntireManager.instance.StartTime + notesManager.NotesTime[noteCount][0] - 0.15f) {
                noteQueue.Add(noteCount);
                noteCount++;
            }

            // 判定ライン±0.1秒内にノーツがある場合の処理
            if (noteQueue.Count != 0) {
                // 判定ライン+0.1秒に到達したノーツを非アクティブにし判定キューから消去
                // 判定ライン内にノーツがある場合全ノーツを検索(LN対応)
                for (int i = 0; i < noteQueue.Count; i++) {
                    if (notesManager.NotesTime[noteQueue[i]].Length == 1) {
                        notesManager.NotesObj[noteQueue[i]][0].SetActive(false);
                        if (Time.time > EntireManager.instance.StartTime + notesManager.NotesTime[noteQueue[i]][0] + judgeTimeABS) noteQueue.RemoveAt(i);
                    } else if (notesManager.NotesTime[noteQueue[i]].Length == 2) {
                        notesManager.NotesObj[noteQueue[i]][0].SetActive(false);
                        notesManager.NotesObj[noteQueue[i]][1].SetActive(false);
                        notesManager.NotesObj[noteQueue[i]][2].SetActive(false);
                        if (Time.time > EntireManager.instance.StartTime + notesManager.NotesTime[noteQueue[i]][1] + judgeTimeABS) noteQueue.RemoveAt(i);
                    }
                }

                // キー入力がされた際の処理
                if (Input.GetKeyDown(KeyCode.LeftShift)) JudgeNotes(0);
                if (Input.GetKeyDown(KeyCode.S)) JudgeNotes(1);
                if (Input.GetKeyDown(KeyCode.D)) JudgeNotes(2);
                if (Input.GetKeyDown(KeyCode.F)) JudgeNotes(3);
                if (Input.GetKeyDown(KeyCode.Equals)) JudgeNotes(4);
                if (Input.GetKeyDown(KeyCode.Semicolon)) JudgeNotes(5);
                if (Input.GetKeyDown(KeyCode.RightBracket)) JudgeNotes(6);
                if (Input.GetKeyDown(KeyCode.RightShift)) JudgeNotes(7);
            }
        }     
    }

    void JudgeNotes(int index) {
        for (int i = 0; i < ; i++) {
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
                break;
            }
        }
    }

    void message(int judge, int i) {
        Instantiate(MessageObj[judge], new Vector3(notesManager.LaneNum[i] - 3.56f, 0.76f, 0.15f), Quaternion.Euler(45, 0, 0));
    }
}
