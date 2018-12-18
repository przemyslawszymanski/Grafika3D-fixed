using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using UnityEngine.UI;
using System.Threading;
using System.Globalization;

public class MazeSteer : MonoBehaviour {


    Text text;
    // Use this for initialization
    SerialPort port;
    string message;
    Thread readThread;
    bool m_continue;

    public Vector3 currentRot;
    public bool KeyboardEnabled;
	public float rotateSpeedKeyboard = 1f;
	public float rotateSpeedMouse = 1f;
	public float fastRotateSpeed = 30f;
	public float MinimumX = -15F;
	public float MaximumX = 15F;
	public GameObject ControllText;

    private volatile float axis_x;
    private volatile float axis_y;
    private volatile float axis_z;
    private Quaternion targetRot;

	void Start () {
		ControllText.SetActive (false);

        m_continue = true;
        text = GetComponent<Text>();

        port = new SerialPort("COM5", 9600);
        port.Open();
        readThread = new Thread(Read);
        readThread.Start();
    }

	void Update () {
        
        if (KeyboardEnabled) {
			RotateWithKeyboard ();
		} else {
		    RotateWithPhone ();
		}
		if(Input.GetKeyDown(KeyCode.Space)){
			ToggleControlls ();
		}
	}

	void RotateWithMouse(){

		float xRot = Input.GetAxis ("Mouse X") * rotateSpeedMouse;
		float zRot = Input.GetAxis ("Mouse Y") * rotateSpeedMouse;
		Rotate (-xRot, -zRot);

	}

    void RotateWithPhone(){
        float xRotation = axis_x;
        float zRotation = axis_z;
        Rotate(xRotation, zRotation);
    }

    void RotateWithKeyboard()
    {

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            Rotate(rotateSpeedKeyboard, 0);
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            Rotate(-rotateSpeedKeyboard, 0);
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            Rotate(0, rotateSpeedKeyboard);
        }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            Rotate(0, -rotateSpeedKeyboard);
        }
    }

    void Rotate(float xRot, float zRot){


        axis_y = Mathf.Clamp(axis_y, MinimumX, MaximumX);
        axis_z = Mathf.Clamp(axis_z, MinimumX, MaximumX);
        transform.rotation = Quaternion.Euler(axis_y, 0, axis_z);


		//targetRot = transform.rotation * Quaternion.Euler (xRot, 0f, zRot);

		//float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan (targetRot.x);
		//angleX = Mathf.Clamp (angleX, MinimumX, MaximumX);
		//targetRot.x = Mathf.Tan (0.5f * Mathf.Deg2Rad * angleX);

		//float angleZ = 2.0f * Mathf.Rad2Deg * Mathf.Atan (targetRot.z);
		//angleZ = Mathf.Clamp (angleZ, MinimumX, MaximumX);
		//targetRot.z = Mathf.Tan (0.5f * Mathf.Deg2Rad * angleZ);

		//targetRot.y = 0f;

		//transform.rotation = targetRot;

	}

    void Read()
    {


        while (m_continue)
        {
            try
            {
                message = port.ReadTo("q");
                var axes = message.Split(' ');
                string axis_x_string = axes[0];
                string axis_y_string = axes[1];
                string axis_z_string = axes[2];

                axis_x = float.Parse(axis_x_string, CultureInfo.InvariantCulture.NumberFormat);
                axis_y = float.Parse(axis_y_string, CultureInfo.InvariantCulture.NumberFormat);
                axis_z = float.Parse(axis_z_string, CultureInfo.InvariantCulture.NumberFormat);
                Debug.Log(axes);
            }
            catch {
                Debug.Log("DUPA");
            }
           
        }
    }

    void ToggleControlls()
    {
        ControllText.SetActive(true);
        if (KeyboardEnabled)
        {
            KeyboardEnabled = false;
            ControllText.GetComponent<TextMesh>().text = "Mouse Controled";
        }
        else
        {
            KeyboardEnabled = true;
            ControllText.GetComponent<TextMesh>().text = "Keyboard Controled";
        }
        StartCoroutine(ToggleActivity());
    }

    IEnumerator ToggleActivity()
    {
        yield return new WaitForSeconds(1f);
        ControllText.SetActive(false);
    }

    void OnDestroy()
    {
        m_continue = false;
        port.Close();
        port.Dispose();
    }
}
