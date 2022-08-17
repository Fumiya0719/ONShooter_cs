using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideNotes : MonoBehaviour
{
    // ハイスピ設定
    float HighSpeed = 15;
    bool start;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            start = true;
        }

        if (start) {
            // ノーツの移動
            // transform.position -= transform.forward * Time.deltaTime * HighSpeed;
            Vector3 pos = this.gameObject.transform.position;
            this.gameObject.transform.position = new Vector3(pos.x, pos.y, pos.z - (Time.deltaTime * HighSpeed));
        }
    }
}
