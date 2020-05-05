using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Basic3DMovementController : MonoBehaviour
{
    public float moveSpeed, turnSpeed;
    public float rotateSensitivity;

    float xAxisClamp, mouseX=0, mouseY=0, turnTime, rotateYLocal=0;
    Vector3 targetRotation;
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

    void CameraHandler()
    {
        mouseX += Input.GetAxis("Mouse X") * Time.fixedDeltaTime * rotateSensitivity % 360;
        mouseY -= Input.GetAxis("Mouse Y") * Time.fixedDeltaTime * rotateSensitivity;
        mouseY = Mathf.Clamp(mouseY, -40, 85);
        targetRotation = new Vector3(mouseY, mouseX);

        //transform.position += (moveForward + moveSide) * moveSpeed * Time.fixedDeltaTime;
        camObject.transform.eulerAngles = targetRotation;
    }

    void PlayerMovementHandler()
    {
        float moveIn = Input.GetAxisRaw("Vertical");
        rotateYLocal += Input.GetAxisRaw("Horizontal") * turnSpeed * Time.fixedDeltaTime;
        rotateYLocal %= 360;

        transform.position += (moveSpeed * moveIn * transform.forward * Time.fixedDeltaTime);
        transform.eulerAngles = new Vector3(0, rotateYLocal, 0);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CameraHandler();
        PlayerMovementHandler();
    }
}
