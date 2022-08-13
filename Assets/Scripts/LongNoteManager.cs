using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongNoteManager : MonoBehaviour
{
    [SerializeField] NotesManager notesManager;

    // Start is called before the first frame update
    void Start()
    {
        GameObject longNote = new GameObject();
        longNote.AddComponent<MeshFilter>();
        longNote.AddComponent<MeshRenderer>();
    }

    // public void GenerateLongNote(Vector3 startPos, Vector3 endPos, GameObject ln) {
        
    //     Mesh mesh = new Mesh();

    //     Vector3[] vertices = new Vector3[4];
    //     int[] triangles = new int[6];

    //     vertices[0] = startPos + new Vector3(-noteScale / 2, 0, 0);
    //     vertices[1] = startPos + new Vector3(noteScale / 2, 0, 0);
    //     vertices[2] = endPos + new Vector3(-noteScale / 2, 0, 0);
    //     vertices[3] = endPos + new Vector3(noteScale / 2, 0, 0);
    //     Debug.Log(vertices[0]);
    //     Debug.Log(vertices[1]);
    //     Debug.Log(vertices[2]);
    //     Debug.Log(vertices[3]);

    //     triangles = new int[6] {0, 2, 1, 3, 1, 2};

    //     mesh.vertices = vertices;
    //     mesh.triangles = triangles;
    //     mesh.RecalculateNormals();

    //     ln.GetComponent<MeshFilter>().mesh = mesh;

    //     LongNotesObj.Add(Instantiate(ln, Vector3.zero, Quaternion.identity));
    // }
}
