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
    private float judgeTimeABS = 0.10f;
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
                        if (Time.time > EntireManager.instance.StartTime + notesManager.NotesTime[noteQueue[i]][0] + judgeTimeABS) {
                            message(3, noteQueue[i]);
                            notesManager.NotesObj[noteQueue[i]][0].SetActive(false);
                            noteQueue.RemoveAt(i);
                        } 
                    } else if (notesManager.NotesTime[noteQueue[i]].Length == 2) {
                        if (Time.time > EntireManager.instance.StartTime + notesManager.NotesTime[noteQueue[i]][1] + judgeTimeABS) {
                            notesManager.NotesObj[noteQueue[i]][0].SetActive(false);
                            notesManager.NotesObj[noteQueue[i]][1].SetActive(false);
                            notesManager.NotesObj[noteQueue[i]][2].SetActive(false);
                            noteQueue.RemoveAt(i);
                        } 
                    }
                }

                // キー入力がされた際の処理
                if (Input.GetKeyDown(KeyCode.LeftShift)) JudgeNotes(0,0);
                if (Input.GetKeyDown(KeyCode.S)) JudgeNotes(1,4);
                if (Input.GetKeyDown(KeyCode.D)) JudgeNotes(2,5);
                if (Input.GetKeyDown(KeyCode.F)) JudgeNotes(3,6);
                if (Input.GetKeyDown(KeyCode.Equals)) JudgeNotes(4,1);
                if (Input.GetKeyDown(KeyCode.Semicolon)) JudgeNotes(5,2);
                if (Input.GetKeyDown(KeyCode.RightBracket)) JudgeNotes(6,3);
                if (Input.GetKeyDown(KeyCode.RightShift)) JudgeNotes(7,7);

                // ロングノーツの処理
                // if (Input.GetKey(KeyCode.LeftShift)) JudgeLongNotes(0,0);
                // if (Input.GetKey(KeyCode.S)) JudgeLongNotes(1,4);
                // if (Input.GetKey(KeyCode.D)) JudgeLongNotes(2,5);
                // if (Input.GetKey(KeyCode.F)) JudgeLongNotes(3,6);
                // if (Input.GetKey(KeyCode.Equals)) JudgeLongNotes(4,1);
                // if (Input.GetKey(KeyCode.Semicolon)) JudgeLongNotes(5,2);
                // if (Input.GetKey(KeyCode.RightBracket)) JudgeLongNotes(6,3);
                // if (Input.GetKey(KeyCode.RightShift)) JudgeLongNotes(7,7);
                // if (Input.GetKey(KeyCode.RightBracket)) {
                //     for (int i = 0; i < noteQueue.Count; i++) {
                //         if (notesManager.NotesObj[noteQueue[i]].Length == 1) continue;
                //         if (notesManager.LaneNum[noteQueue[i]] == 6) {
                //             notesManager.NotesObj[noteQueue[i]][1].layer = LayerMask.NameToLayer("LongNote");
                //         }
                //         Debug.Log("test");
                //     }
                // }
                JudgeLongNotes();
            }
        }     
    }

    void isPuttingKey(int i, bool KeyCode) {
        if (KeyCode) {
            notesManager.NotesObj[noteQueue[i]][1].layer = LayerMask.NameToLayer("LongNote");
            float enTime = Time.time - (notesManager.NotesTime[noteQueue[i]][1] + EntireManager.instance.StartTime);
            if (enTime < 0) {
                message(0, noteQueue[i]);
                EntireManager.instance.CBreak++;
                notesManager.NotesObj[noteQueue[i]][0].SetActive(false);
                notesManager.NotesObj[noteQueue[i]][1].SetActive(false);
                notesManager.NotesObj[noteQueue[i]][2].SetActive(false);
            }
        } else {
            notesManager.NotesObj[noteQueue[i]][1].layer = LayerMask.NameToLayer("Default");
        }
    }

    void JudgeLongNotes() {
        for (int i = 0; i < noteQueue.Count; i++) {
            if (notesManager.NotesObj[noteQueue[i]].Length == 1) continue;
            // Debug.Log(noteQueue[i]);
            if (notesManager.LaneNum[noteQueue[i]] == 0) {
                isPuttingKey(i, Input.GetKey(KeyCode.LeftShift));
            } else if (notesManager.LaneNum[noteQueue[i]] == 1 || notesManager.LaneNum[noteQueue[i]] == 4) {
                isPuttingKey(i, Input.GetKey(KeyCode.S));
                isPuttingKey(i, Input.GetKey(KeyCode.Equals));
            } else if (notesManager.LaneNum[noteQueue[i]] == 2 || notesManager.LaneNum[noteQueue[i]] == 5) {
                isPuttingKey(i, Input.GetKey(KeyCode.D));
                isPuttingKey(i, Input.GetKey(KeyCode.Semicolon));
            } else if (notesManager.LaneNum[noteQueue[i]] == 3 || notesManager.LaneNum[noteQueue[i]] == 6) {
                isPuttingKey(i, Input.GetKey(KeyCode.F));
                isPuttingKey(i, Input.GetKey(KeyCode.RightBracket));
            } else if (notesManager.LaneNum[noteQueue[i]] == 7) {
                isPuttingKey(i, Input.GetKey(KeyCode.RightShift));
            }
        }
    }

    void JudgeNotes(params int[] args) {
        for (int i = 0; i < noteQueue.Count; i++) {
            float timeLag = Time.time - (notesManager.NotesTime[noteQueue[i]][0] + EntireManager.instance.StartTime);
            float timeLagABS = Mathf.Abs(timeLag);
               
            if (timeLagABS <= 0.10 && 
                (notesManager.LaneNum[noteQueue[i]] == args[0] ||
                notesManager.LaneNum[noteQueue[i]] == args[1])) {
                if (JudgeSlowAndFast) {
                    if (timeLag < 0) {
                        Debug.Log("fast");
                    } else {
                        Debug.Log("slow");
                    }
                }

                if (timeLagABS <= 0.033) {
                    message(0, noteQueue[i]);
                    EntireManager.instance.CBreak++;
                } else if (timeLagABS <= 0.066) {
                    message(1, noteQueue[i]);
                    EntireManager.instance.Break++;
                } else {
                    message(2, noteQueue[i]);
                    EntireManager.instance.Hit++;
                }
                EntireManager.instance.combo++;
                notesManager.NotesObj[noteQueue[i]][0].SetActive(false);
                noteQueue.RemoveAt(i);
                break;
            }
        }
    }

    void message(int judge, int i) {
        Instantiate(MessageObj[judge], new Vector3(notesManager.LaneNum[i] - 3.56f, 0.76f, 0.15f), Quaternion.Euler(45, 0, 0));
    }
}
