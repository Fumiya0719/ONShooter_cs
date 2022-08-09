using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Notes : MonoBehaviour
{
    // ハイスピ設定
    [SerializeField] NotesManager notesManager;
    // Update is called once per frame

    float HighSpeed = 15;
    bool start;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            start = true;
        }

        if (start) {
            // ノーツの移動
            transform.position -= transform.forward * Time.deltaTime * HighSpeed;
        }
    }
}
