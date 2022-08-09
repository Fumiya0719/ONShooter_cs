using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitLight : MonoBehaviour
{
    [SerializeField] float Speed = 3;
    [SerializeField] int num = 0;
    private Renderer rend;
    private float alpha = 0;

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!(rend.material.color.a <= 0)) {
            rend.material.color = new Color(rend.material.color.r, rend.material.color.g, rend.material.color.b, alpha);
        }

        switch (num) {
            case 0:
                if (Input.GetKeyDown(KeyCode.LeftShift)) colorChange();
                break;
            case 1:
                if (Input.GetKeyDown(KeyCode.S)) colorChange();
                break;
            case 2:
                if (Input.GetKeyDown(KeyCode.D)) colorChange();
                break;
            case 3:
                if (Input.GetKeyDown(KeyCode.F)) colorChange();
                break;
            case 4:
                if (Input.GetKeyDown(KeyCode.Equals)) colorChange();
                break;
            case 5:
                if (Input.GetKeyDown(KeyCode.Semicolon)) colorChange();
                break;
            case 6:
                if (Input.GetKeyDown(KeyCode.RightBracket)) colorChange();
                break;
            case 7:
                if (Input.GetKeyDown(KeyCode.RightShift)) colorChange();
                break;
        }

        alpha -= Speed * Time.deltaTime;
    }

    void colorChange() {
        alpha = 0.5f;
        rend.material.color = new Color(rend.material.color.r, rend.material.color.g, rend.material.color.b, alpha);
    }
}
