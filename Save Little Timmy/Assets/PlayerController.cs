
using UnityEngine;


[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour
{

    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    private float looksensitivity = 3f;


    private PlayerMotor motor;

    private void Start()
    {
        motor = GetComponent <PlayerMotor> ();
    }

    private void Update()
    {
        float xMov = Input.GetAxisRaw("Horizontal");
        float zMov = Input.GetAxisRaw("Vertical");



        Vector3 movHorizontal = transform.right * xMov;
        Vector3 movVertical = transform.forward * zMov;



        Vector3 velocity = (movHorizontal + movVertical).normalized * speed;



        motor.Move(velocity);



        float yRot = Input.GetAxisRaw("Mouse X");

        Vector3 rotation = new Vector3(0f, yRot, 0f) * looksensitivity;

        motor.Rotate(rotation);


        float xRot = Input.GetAxisRaw("Mouse Y");

        Vector3 camerarotation = new Vector3(xRot, 0f, 0f) * looksensitivity;

        motor.RotateCamera(camerarotation);


    }
    }