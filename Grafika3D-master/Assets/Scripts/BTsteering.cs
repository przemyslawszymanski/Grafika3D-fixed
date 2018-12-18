using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using UnityEngine.UI;
using System.Threading;
using System.Globalization;

public class BTsteering : MonoBehaviour
{

    Text text;
    // Use this for initialization
    SerialPort port;
    string message;
    Thread readThread;
    bool m_continue;

    void Start()
    {
        m_continue = true;
        text = GetComponent<Text>();
        
        port = new SerialPort("COM5", 9600);
        port.Open();
        readThread = new Thread(Read);
        readThread.Start();
    }

    // Update is called once per frame
    void Update()
    {
        if (message != "")
        {
            text.text = message;
        }
        else
        {
            text.text = "pusto";
        }
    
    }

    void Read()
    {
        while(m_continue)
        {
            
            message = port.ReadTo("q");
            var axes = message.Split(' ');
            string axis_x_string = axes[0];
            string axis_y_string = axes[1];
            string axis_z_string = axes[2];

            var axis_x = float.Parse(axis_x_string, CultureInfo.InvariantCulture.NumberFormat);
            var axis_y = float.Parse(axis_y_string, CultureInfo.InvariantCulture.NumberFormat);
            var axis_z = float.Parse(axis_z_string, CultureInfo.InvariantCulture.NumberFormat);

        }
    }

    void OnDestroy()
    {
        m_continue = false;
        port.Close();
        port.Dispose();
    }
}