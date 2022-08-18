using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Notes : MonoBehaviour
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
            Vector3 pos = this.gameObject.transform.position;
            this.gameObject.transform.position = new Vector3(pos.x, pos.y, pos.z - (Time.deltaTime * HighSpeed));
        }
    }
}
