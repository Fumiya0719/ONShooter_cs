using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverAim : MonoBehaviour
{
    [SerializeField] public float sens;
    private Vector3 mousePos;
    private float argPos;
    private float objPos;
    private bool start;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            start = true;
        }

        if (start || !start) {
            // ノーツの移動
            mousePos = Input.mousePosition;
            argPos = mousePos.x * sens;
            // Debug.Log(mousePos.x);
            if (argPos < -3.56f) {
                objPos = -3.56f;
            } else if (argPos > 3.56f) {
                objPos = 3.56f;
            } else {
                objPos = argPos;
            }
            
            transform.position = new Vector3(objPos, 0.50f, -0.80f);
        }
    }
}
