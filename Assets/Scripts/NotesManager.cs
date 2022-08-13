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
    public List<List> NotesObj = new List<List>();
    // public List<GameObject> LongNotesObj = new List<GameObject>();
    // public List<GameObject> EndNotesObj = new List<GameObject>();  

    [SerializeField] public float NotesSpeed;
    [SerializeField] private GameObject[] noteObjs;
    [SerializeField] private GameObject[] longNoteObjs;
    [SerializeField] private GameObject[] endNoteObjs;

    private GameObject noteObj;
    private GameObject longNoteObj;
    private GameObject endNoteObj;

    private MeshFilter meshFilter;
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
        // ノーツオブジェクトの2次元化(LNとENを含める為)
        NotesObj.Add(new List<GameObject>());

        for (int i = 0; i < inputJson.notes.Length; i++) {
            noteObj = noteObjs[inputJson.notes[i].block];

            float interval = 60 / (inputJson.BPM * (float)inputJson.notes[i].LPB);
            float beatSec = interval * (float)inputJson.notes[i].LPB;
            float time = (beatSec * inputJson.notes[i].num / (float)inputJson.notes[i].LPB) + inputJson.offset * 0.01f;

            NotesTime.Add(time);
            LaneNum.Add(inputJson.notes[i].block);
            NoteType.Add(inputJson.notes[i].type);

            float z = NotesTime[i] * NotesSpeed;

            NotesObj[i].Add(Instantiate(noteObj, new Vector3(inputJson.notes[i].block - 3.56f, 0.55f, z), Quaternion.identity));

            // ロングノーツ(LN)の生成
            if (inputJson.notes[i].type == 2) {
                longNoteObj = longNoteObjs[inputJson.notes[i].block];
                endNoteObj = endNoteObjs[inputJson.notes[i].notes[0].block];

                // 始点のZ座標
                float startZ = z;
                // 終点のZ座標
                float enInterval = 60 / (inputJson.BPM * (float)inputJson.notes[i].notes[0].LPB);
                float enBeatSec = enInterval * (float)inputJson.notes[i].notes[0].LPB;
                float enTime = (beatSec * inputJson.notes[i].notes[0].num / (float)inputJson.notes[i].notes[0].LPB) + inputJson.offset * 0.01f;

                float endZ = enTime * NotesSpeed;
                // LN終点ノーツを追加
                notesObj[i].Add(Instantiate(endNoteObj, new Vector3(inputJson.notes[i].block - 3.56f, 0.55f, endZ), Quaternion.identity));

                // LN始点のX座標
                Vector3 startPos = new Vector3(inputJson.notes[i].block - 3.56f, 0.55f, z);
                // LN終点のX座標
                Vector3 endPos = new Vector3(inputJson.notes[i].block - 3.56f, 0.55f, endZ);
               
                GenerateLongNote(startPos, endPos, longNoteObj, inputJson.notes[i].block);

                // LNを追加
                notesObj.Add(Instantiate(longNoteObj, startPos, Quaternion.identity));
            }
        }
    }

    public void GenerateLongNote(Vector3 startPos, Vector3 endPos, GameObject ln, int block) {        
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[4];
        int[] triangles = new int[6];

        Vector3 lnLength = (endPos - startPos) * 2.3f;

        vertices[0] = new Vector3(-noteScale / 2, 0, 0);
        vertices[1] = new Vector3(noteScale / 2, 0, 0);
        vertices[2] = lnLength + new Vector3(-noteScale / 2, 0, 0);
        vertices[3] = lnLength + new Vector3(noteScale / 2, 0, 0);
        Debug.Log(vertices[0]);
        Debug.Log(vertices[1]);
        Debug.Log(vertices[2]);
        Debug.Log(vertices[3]);

        triangles = new int[6] {0, 2, 1, 3, 1, 2};

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        ln.GetComponent<MeshFilter>().mesh = mesh;
    }
}
