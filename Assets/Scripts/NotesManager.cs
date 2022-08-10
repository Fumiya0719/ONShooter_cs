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

    [SerializeField] private float NotesSpeed;
    [SerializeField] private GameObject[] noteObjs;

    private GameObject noteObj;

    void OnEnable() {
        noteNum = 0;
        title = "Emargence Vibe2";
        Load(title);
    }

    private void Load(string title) {
        string inputString = Resources.Load<TextAsset>(title).ToString();
        Data inputJson = JsonUtility.FromJson<Data>(inputString);

        noteNum = inputJson.notes.Length;

        for (int i = 0; i < inputJson.notes.Length; i++)
        {
            noteObj = noteObjs[inputJson.notes[i].block];

            float interval = 60 / (inputJson.BPM * (float)inputJson.notes[i].LPB);
            float beatSec = interval * (float)inputJson.notes[i].LPB;
            float time = (beatSec * inputJson.notes[i].num / (float)inputJson.notes[i].LPB) + inputJson.offset * 0.01f;
            NotesTime.Add(time);
            LaneNum.Add(inputJson.notes[i].block);
            NoteType.Add(inputJson.notes[i].type);

            float z = NotesTime[i] * NotesSpeed;

            NotesObj.Add(Instantiate(noteObj, new Vector3(inputJson.notes[i].block - 3.56f, 0.55f, z), Quaternion.identity));
        }
        
        // for (int i = 0; i < 46; i++) {
        //     Debug.Log(NotesTime[i]);
        // }
    }
}
