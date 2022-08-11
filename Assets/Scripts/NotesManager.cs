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

    private float noteScale = 1.0f;

    void OnEnable() {
        noteNum = 0;
        title = "Emargence Vibe2";
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

            // ロングノーツの場合、専用ジェネレーターにかける
            if (inputJson.notes[i].type == 2) {
                // 始点のZ座標
                float startZ = z;
                // 終点のZ座標
                float enInterval = 60 / (inputJson.BPM * (float)inputJson.notes[i].notes);

                // 始点のX座標
                Vector3 startX = new Vector3(inputJson.notes[i].block - 3.56f, 0.55f, z);
                // 終点のX座標
                Vector3 endX = new Vector3(inputJson.notes[i].notes - 3.56f, 0.55f, z);

                GenerateLongNote(startX, endX, noteObj);
            }
            NotesObj.Add(Instantiate(noteObj, new Vector3(inputJson.notes[i].block - 3.56f, 0.55f, z), Quaternion.identity));
        }
    }

    private void GenerateLongNote(Vector3 startX, Vector3 endX, GameObject noteObj) {
        // メッシュの生成
        Mesh mesh = new Mesh();
        noteObj.GetComponent<MeshFilter>().mesh = mesh;

        Vector3[] Vertices = new Vector3[4];
        int[] triangles = new int[6];

        Vertices[0] = startX + new Vector3(-noteScale / 2, 0, 0);
        Vertices[1] = startX + new Vector3(noteScale / 2, 0, 0);
        Vertices[2] = endX + new Vector3(-noteScale / 2, 0, 0);
        Vertices[3] = endX + new Vector3(noteScale / 2, 0, 0);

        triangles = new int[6] {0,2,1,3,1,2};

        mesh.vertices = Vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
}
