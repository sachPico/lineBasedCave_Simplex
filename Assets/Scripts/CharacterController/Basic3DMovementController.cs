using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Basic3DMovementController : MonoBehaviour
{
    public float moveSpeed;
    public float rotateSensitivity;

    float xAxisClamp;
    GameObject camObject;
    // Start is called before the first frame update

    void ClampXAxisRotation(float value)
    {
        Vector3 eulerRotation = transform.eulerAngles;
        eulerRotation.x = value;
        transform.eulerAngles = eulerRotation;
    }
    void Start()
    {
        camObject = transform.GetChild(0).gameObject;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 moveForward, moveSide;
        moveForward = transform.forward * Input.GetAxisRaw("Vertical");
        moveSide = transform.right * Input.GetAxisRaw("Horizontal");

        float mouseX = Input.GetAxis("Mouse X") * Time.fixedDeltaTime * rotateSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * Time.fixedDeltaTime * rotateSensitivity;

        xAxisClamp+=mouseY;

        if(xAxisClamp>90f)
        {
            xAxisClamp = 90f;
            mouseY = 0;
            ClampXAxisRotation(270f);
        }
        else if(xAxisClamp<-90f)
        {
            xAxisClamp = -90f;
            mouseY = 0;
            ClampXAxisRotation(90f);
        }

        transform.position += (moveForward + moveSide) * moveSpeed * Time.fixedDeltaTime;
        camObject.transform.Rotate(-transform.right * mouseY);
        camObject.transform.Rotate(Vector3.up * mouseX);
    }
}
