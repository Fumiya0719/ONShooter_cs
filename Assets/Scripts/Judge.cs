using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Judge : MonoBehaviour
{
    [SerializeField] private GameObject[] MessageObj;
    [SerializeField] NotesManager notesManager;
    [SerializeField] LeverAim leverAim;

    [SerializeField] bool JudgeSlowAndFast;
    [SerializeField] bool DisplayCombo;
    [SerializeField] bool DisplayJudge;

    /*
    メモ：
    判定にロングノーツが含まれると流れ切ったノーツを逐一消去していると処理が複雑or不可能になる
    →リストから消去は行わず、オブジェクトをInactiveするのみの処理を行う
    全ノーツのうち、判定部分(判定ライン前後0.1sec)に入ったノーツのリストインデックスを別の配列等にキューとして保存し、キューのうち対象キーが押された(ロングノーツの場合長押し)していた場合判定を行う。
    キューのうち、判定ライン到達から0.1秒たったノーツのInactiveにする。
    そのノーツがロングノーツの場合、終点が0.1秒経った時にすべてInactiveにする。
    */

    [SerializeField] private AudioSource NoteSESource;
    [SerializeField] private AudioSource SideSESource;
    [SerializeField] private AudioSource FlickSESource;
    [SerializeField] private AudioClip NoteSE;
    [SerializeField] private AudioClip SideSE;
    [SerializeField] private AudioClip FlickSE;

    private int noteCount = 0;
    private float judgeTimeABS = 0.10f;
    private List<int> noteQueue = new List<int>();

    private float mousePos;
    private float objPos;
    private float preMousePosX;

    void Update()
    {
        if (EntireManager.instance.Start) {
            // 判定ライン-0.15秒に到達したノーツを判定キューに挿入
            // (Hit判定は±0.1秒だが処理によって若干の遅延が起きる可能性があるので少し時間を取る)
            if (Time.time >= EntireManager.instance.StartTime + notesManager.NotesTime[noteCount][0] - 0.10f) {
                noteQueue.Add(noteCount);
                noteCount++;
            }

            // 判定ライン±0.1秒内にノーツがある場合の処理
            if (noteQueue.Count != 0) {
                // ロングノーツの処理
                IsExistLongNotes();
                // フリックの処理
                mousePos = Input.mousePosition.x * leverAim.sens;
                if (mousePos < -3.56f) {
                    objPos = -3.56f;
                } else if (mousePos > 3.56f) {
                    objPos = 3.56f;
                } else {
                    objPos = mousePos;
                }
                JudgeFlick(preMousePosX, objPos);
                preMousePosX = objPos;

                // キー入力がされた際の処理
                if (Input.GetKeyDown(KeyCode.LeftShift)) JudgeNotes(0);
                if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.Equals)) JudgeNotes(1);
                if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.Semicolon)) JudgeNotes(2);
                if (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.RightBracket)) JudgeNotes(3);
                if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.Equals)) JudgeNotes(4);
                if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.Semicolon)) JudgeNotes(5);
                if (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.RightBracket)) JudgeNotes(6);
                if (Input.GetKeyDown(KeyCode.RightShift)) JudgeNotes(7);

                // 判定ライン+0.1秒に到達したノーツを非アクティブにし判定キューから消去
                // 判定ライン内にノーツがある場合全ノーツを検索(LN対応)
                for (int i = 0; i < noteQueue.Count; i++) {
                    if (notesManager.NotesTime[noteQueue[i]].Length == 1) {
                        if (Time.time > EntireManager.instance.StartTime + notesManager.NotesTime[noteQueue[i]][0] + judgeTimeABS) {
                            if (notesManager.NotesObj[noteQueue[i]][0].activeSelf) {
                                message(3, noteQueue[i]);
                                EntireManager.instance.combo = 0;
                                notesManager.NotesObj[noteQueue[i]][0].SetActive(false);
                            }
                            noteQueue.RemoveAt(i);
                        } 
                    } else if (notesManager.NotesTime[noteQueue[i]].Length == 2) {
                        if (notesManager.NotesObj[noteQueue[i]][0].activeSelf && Time.time > EntireManager.instance.StartTime + notesManager.NotesTime[noteQueue[i]][0] + judgeTimeABS) {
                            message(3, noteQueue[i]);
                            EntireManager.instance.combo = 0;
                            notesManager.NotesObj[noteQueue[i]][0].SetActive(false);
                        } 
                        if (Time.time > EntireManager.instance.StartTime + notesManager.NotesTime[noteQueue[i]][1] + judgeTimeABS) {
                            if (notesManager.NotesObj[noteQueue[i]][2].activeSelf) {
                                message(3, noteQueue[i]);
                                EntireManager.instance.combo = 0;
                                notesManager.NotesObj[noteQueue[i]][1].SetActive(false);
                                notesManager.NotesObj[noteQueue[i]][2].SetActive(false);
                            }
                            noteQueue.RemoveAt(i);
                        } 
                    }
                }
            }
        }     
    }

    void JudgeFlick(float prePos, float nowPos) {
        for (int i = 0; i < noteQueue.Count; i++) {
            if (!(notesManager.LaneNum[noteQueue[i]] == 8 || notesManager.LaneNum[noteQueue[i]] == 9)) continue;
            if (notesManager.LaneNum[noteQueue[i]] == 8 && nowPos - prePos < 0) {
                FlickSESource.PlayOneShot(FlickSE);
                Instantiate(MessageObj[0], new Vector3(-1f, 0.76f, 0.15f), Quaternion.Euler(45, 0, 0));
                EntireManager.instance.CBreak++;
                notesManager.NotesObj[noteQueue[i]][0].SetActive(false);
                noteQueue.RemoveAt(i);
            } else if (notesManager.LaneNum[noteQueue[i]] == 9 && nowPos - prePos > 0) {
                FlickSESource.PlayOneShot(FlickSE);
                Instantiate(MessageObj[0], new Vector3(1f, 0.76f, 0.15f), Quaternion.Euler(45, 0, 0));
                EntireManager.instance.CBreak++;
                notesManager.NotesObj[noteQueue[i]][0].SetActive(false);
                noteQueue.RemoveAt(i);
            }
        }
    }

    private float lnTime;
    private float enTime;

    void IsExistLongNotes() {
        for (int i = 0; i < noteQueue.Count; i++) {
            if (notesManager.NotesObj[noteQueue[i]].Length == 1) continue;

            lnTime = Time.time - (notesManager.NotesTime[noteQueue[i]][0] + EntireManager.instance.StartTime + (notesManager.longNoteInterval * notesManager.CountMidLN[noteQueue[i]]));
            enTime = Time.time - (notesManager.NotesTime[noteQueue[i]][1] + EntireManager.instance.StartTime);

            // 1/2リズム毎0.1f以内にキー入力があった場合、判定を出力
            if (enTime <= -0.10f) {
                if (lnTime > 0f && notesManager.JudgeFlag[noteQueue[i]] != 3) {
                    // Debug.Log(notesManager.JudgeFlag[0]);
                    message(notesManager.JudgeFlag[noteQueue[i]], noteQueue[i]);
                    notesManager.CountMidLN[noteQueue[i]]++;
                    notesManager.JudgeFlag[noteQueue[i]] = 3;
                }

                if (notesManager.JudgeFlag[noteQueue[i]] != 0 && lnTime > 0.33f) {
                    message(notesManager.JudgeFlag[noteQueue[i]], noteQueue[i]);
                    notesManager.CountMidLN[noteQueue[i]]++;
                    notesManager.JudgeFlag[noteQueue[i]] = 3;
                } else if (notesManager.JudgeFlag[noteQueue[i]] != 0 && notesManager.JudgeFlag[noteQueue[i]] != 1 && lnTime > 0.66f && lnTime <= 0.10f) {
                    message(notesManager.JudgeFlag[noteQueue[i]], noteQueue[i]);
                    notesManager.CountMidLN[noteQueue[i]]++;
                    notesManager.JudgeFlag[noteQueue[i]] = 3;
                }

                if (lnTime > 0.10f) {
                    if (notesManager.JudgeFlag[noteQueue[i]] == 3) {
                        message(3, noteQueue[i]);
                        notesManager.CountMidLN[noteQueue[i]]++;
                    } // else {
                    //     notesManager.JudgeFlag[noteQueue[i]] = 3;
                    // }
                }
            } else if (notesManager.NotesObj[noteQueue[i]][2].activeSelf) {
                if (lnTime > 0f && notesManager.JudgeFlag[noteQueue[i]] != 3) {
                    NoteSESource.PlayOneShot(NoteSE);
                    message(notesManager.JudgeFlag[noteQueue[i]], noteQueue[i]);
                    DeleteLNData(i);
                } else if (notesManager.JudgeFlag[noteQueue[i]] != 0 && lnTime > 0.33f) {
                    NoteSESource.PlayOneShot(NoteSE);
                    message(notesManager.JudgeFlag[noteQueue[i]], noteQueue[i]);
                    DeleteLNData(i);
                } else if (notesManager.JudgeFlag[noteQueue[i]] != 0 && notesManager.JudgeFlag[noteQueue[i]] != 1 && lnTime > 0.66f && lnTime <= 0.10f) {
                    NoteSESource.PlayOneShot(NoteSE);
                    message(notesManager.JudgeFlag[noteQueue[i]], noteQueue[i]);
                    DeleteLNData(i);
                }
            }

            if (Input.GetKey(KeyCode.LeftShift)) JudgeLongNotes(i, 0);
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.Equals)) JudgeLongNotes(i, 1);
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.Semicolon)) JudgeLongNotes(i, 2);
            if (Input.GetKey(KeyCode.F) || Input.GetKey(KeyCode.RightBracket)) JudgeLongNotes(i, 3);
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.Equals)) JudgeLongNotes(i, 4);
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.Semicolon)) JudgeLongNotes(i, 5);
            if (Input.GetKey(KeyCode.F) || Input.GetKey(KeyCode.RightBracket)) JudgeLongNotes(i, 6);
            if (Input.GetKey(KeyCode.RightShift)) JudgeLongNotes(i, 7);
        }
    }

    void JudgeLongNotes(int i, int key) {
        if (noteQueue.Count != 0) {
            // メモ
            // LNが2つ以上あるとき(？)LNの終点が消滅したタイミングでバグが起きる事がある。(index was out of range)
            // ↑更新：2つ以上ノーツがある際終点で1つだけ消えたときに発生する。
            if (notesManager.LaneNum[noteQueue[i]] == key) {
                notesManager.NotesObj[noteQueue[i]][1].layer = LayerMask.NameToLayer("LongNote");

                // LNの途中判定(1/2リズム毎)
                if (enTime <= -0.10f) {
                    if (lnTime > -0.10f) {
                        notesManager.JudgeFlag[noteQueue[i]] = 2;
                        if (lnTime > -0.66f) {
                            notesManager.JudgeFlag[noteQueue[i]] = 1;
                        } if (lnTime > -0.33f) {
                            notesManager.JudgeFlag[noteQueue[i]] = 0;
                        }
                    } else if (notesManager.JudgeFlag[noteQueue[i]] != 0 && lnTime > 0.33f) {
                        notesManager.JudgeFlag[noteQueue[i]] = 1;
                    } else if (notesManager.JudgeFlag[noteQueue[i]] != 0 && notesManager.JudgeFlag[noteQueue[i]] != 1 && lnTime > 0.66f && lnTime <= 0.10f) {
                        notesManager.JudgeFlag[noteQueue[i]] = 2;
                    }
                } else if (notesManager.NotesObj[noteQueue[i]][2].activeSelf) {
                    if (lnTime > -0.10f) {
                        notesManager.JudgeFlag[noteQueue[i]] = 2;
                        if (lnTime > -0.66f) {
                            notesManager.JudgeFlag[noteQueue[i]] = 1;
                        } if (lnTime > -0.33f) {
                            notesManager.JudgeFlag[noteQueue[i]] = 0;
                        } 
                    } else if (notesManager.JudgeFlag[noteQueue[i]] != 0 && lnTime > 0.33f) {
                        notesManager.JudgeFlag[noteQueue[i]] = 1;
                    } else if (notesManager.JudgeFlag[noteQueue[i]] != 0 && notesManager.JudgeFlag[noteQueue[i]] != 1 && lnTime > 0.66f && lnTime <= 0.10f) {
                        notesManager.JudgeFlag[noteQueue[i]] = 2;
                    }
                }     
            } //else {
            //     notesManager.NotesObj[noteQueue[i]][1].layer = LayerMask.NameToLayer("Default");
            // }
        }
    }

    // LNデータの削除
    void DeleteLNData(int i) {
        notesManager.NotesObj[noteQueue[i]][0].SetActive(false);
        notesManager.NotesObj[noteQueue[i]][1].SetActive(false);
        notesManager.NotesObj[noteQueue[i]][2].SetActive(false);
    }

    void JudgeNotes(int key) {
        for (int i = 0; i < noteQueue.Count; i++) {
            float timeLag = Time.time - (notesManager.NotesTime[noteQueue[i]][0] + EntireManager.instance.StartTime);
            float timeLagABS = Mathf.Abs(timeLag);
               
            if (timeLagABS <= 0.10 && (notesManager.LaneNum[noteQueue[i]] == key)) {
                if (JudgeSlowAndFast) {
                    if (timeLag < 0) {
                        Debug.Log("fast");
                    } else {
                        Debug.Log("slow");
                    }
                }

                // sideノーツの判定
                if (key == 0 || key == 7) {
                    SideSESource.PlayOneShot(SideSE);
                    if (timeLagABS <= 0.05) {
                        message(0, noteQueue[i]);
                    } else {
                        message(1, noteQueue[i]);
                    }
                // 通常ノーツの判定
                } else {
                    NoteSESource.PlayOneShot(NoteSE);
                    if (timeLagABS <= 0.033) {
                        message(0, noteQueue[i]);
                    } else if (timeLagABS <= 0.066) {
                        message(1, noteQueue[i]);
                    } else {
                        message(2, noteQueue[i]);
                    }
                }

                notesManager.NotesObj[noteQueue[i]][0].SetActive(false);
                if (DisplayCombo) Debug.Log(EntireManager.instance.combo);
                if (DisplayJudge) {
                    Debug.Log(EntireManager.instance.CBreak);
                    Debug.Log(EntireManager.instance.Break);
                    Debug.Log(EntireManager.instance.Hit);
                    Debug.Log(EntireManager.instance.Miss);
                }
                break;
            }
        }
    }

    void message(int judge, int i) {
        Instantiate(MessageObj[judge], new Vector3(notesManager.LaneNum[i] - 3.56f, 0.76f, 0.15f), Quaternion.Euler(45, 0, 0));
        switch (judge) {
            case 0:
                EntireManager.instance.CBreak++;
                EntireManager.instance.combo++;
                break;
            case 1:
                EntireManager.instance.Break++;
                EntireManager.instance.combo++;
                break;
            case 2:
                EntireManager.instance.Hit++;
                EntireManager.instance.combo++;
                break;
            case 3:
                EntireManager.instance.Miss++;
                EntireManager.instance.combo = 0;
                break;
            default:
                EntireManager.instance.Miss++;
                EntireManager.instance.combo = 0;
                break;
        }
    }
}
