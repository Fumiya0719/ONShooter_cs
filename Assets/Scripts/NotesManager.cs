using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Data
{
    public string name;
    public int maxBlock;
    public int BPM;
    public int offset;
    public Note[] notes;

}

[System.Serializable]
public class Note
{
    public int type;
    public int num;
    public int block;
    public int LPB;

    public LongNote[] notes;
}

[System.Serializable]
public class LongNote {
    public int type;
    public int num;
    public int block;
    public int LPB;
}

public class NotesManager : MonoBehaviour
{
    public int noteNum;
    // 曲名
    public string title;
    public List<int> LaneNum = new List<int>();
    public List<int> NoteType = new List<int>();
    public List<float> NotesTime = new List<float>();
    public List<GameObject> NotesObj = new List<GameObject>();
    public List<GameObject> LongNotesObj = new List<GameObject>();
    public List<GameObject> EndNotesObj = new List<GameObject>();  

    [SerializeField] public float NotesSpeed;
    [SerializeField] private GameObject[] noteObjs;
    [SerializeField] private GameObject[] endNoteObjs;

    private GameObject noteObj;
    private GameObject endNoteObj;

    private float noteScale = 1.0f;

    void OnEnable() {
        noteNum = 0;
        title = "Girl Meets Love";
        Load(title);
    }

    private void Load(string title) {
        string inputString = Resources.Load<TextAsset>(title).ToString();
        Data inputJson = JsonUtility.FromJson<Data>(inputString);

        noteNum = inputJson.notes.Length;

        for (int i = 0; i < inputJson.notes.Length; i++) {
            noteObj = noteObjs[inputJson.notes[i].block];

            float interval = 60 / (inputJson.BPM * (float)inputJson.notes[i].LPB);
            float beatSec = interval * (float)inputJson.notes[i].LPB;
            float time = (beatSec * inputJson.notes[i].num / (float)inputJson.notes[i].LPB) + inputJson.offset * 0.01f;

            NotesTime.Add(time);
            LaneNum.Add(inputJson.notes[i].block);
            NoteType.Add(inputJson.notes[i].type);

            float z = NotesTime[i] * NotesSpeed;

            NotesObj.Add(Instantiate(noteObj, new Vector3(inputJson.notes[i].block - 3.56f, 0.55f, z), Quaternion.identity));

            // ロングノーツの生成
            
            if (inputJson.notes[i].type == 2) {
                endNoteObj = endNoteObjs[inputJson.notes[i].notes[0].block];
                // 始点のZ座標
                float startZ = z;
                // 終点のZ座標
                float enInterval = 60 / (inputJson.BPM * (float)inputJson.notes[i].notes[0].LPB);
                float enBeatSec = enInterval * (float)inputJson.notes[i].notes[0].LPB;
                float enTime = (beatSec * inputJson.notes[i].notes[0].num / (float)inputJson.notes[i].notes[0].LPB) + inputJson.offset * 0.01f;

                float endZ = enTime * NotesSpeed;
                // ロングノーツ終点にオブジェクトを生成
                EndNotesObj.Add(Instantiate(endNoteObj, new Vector3(inputJson.notes[i].block - 3.56f, 0.55f, endZ), Quaternion.identity));

                // LN始点と終点の差分
                float localZ = startZ - endZ;

                // 始点のX座標
                Vector3 startPos = new Vector3(0, 0, 0);
                // 終点のX座標
                Vector3 endPos = new Vector3(0, 0, localZ);

                LineRenderer line = endNoteObj.GetComponent<LineRenderer>();

                var pos = new Vector3[] { startPos, endPos };
                line.SetPositions(pos);
            }
        }
    }

    public void GenerateLongNote(Vector3 startPos, Vector3 endPos, GameObject noteObj, int block) {
        // Line Rendererの生成
        Debug.Log(startPos);
        Debug.Log(endPos);
        Debug.Log(noteObj);
        Debug.Log(block);
        var Line = noteObj.AddComponent<LineRenderer>();
        Debug.Log(Line);

        // Debug.Log(Line);

        var pos = new Vector3[] {
            startPos,
            endPos
        };

        Line.SetPositions(pos);

        Line.startWidth = noteScale;
        Line.endWidth = noteScale;
    }
}
