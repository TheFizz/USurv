using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    // Start is called before the first frame update
    public Vector2 InputVector { get; set; }
    public Vector3 MousePosition { get; set; }

    // Update is called once per frame
    void Update()
    {
        var h = Input.GetAxis("Horizontal");
        var v = Input.GetAxis("Vertical");
        InputVector = new Vector2(h, v);
        InputVector = Vector2.ClampMagnitude(InputVector, 1f);
        MousePosition = Input.mousePosition;
    }
}
