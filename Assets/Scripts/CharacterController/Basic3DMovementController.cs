using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Basic3DMovementController : MonoBehaviour
{
    public float moveSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position += new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")) * moveSpeed * Time.fixedDeltaTime;
    }
}
