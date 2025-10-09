using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System.Linq;
using Unity.VisualScripting;

public class ArduinoCommunicator : MonoBehaviour
{
    SerialPort inputStream;
    public string portName = "COM4";
    public int portVal = 9600;

    private string receivedStream;
    private bool isActive = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (SerialPort.GetPortNames().ToList().Contains(portName))
        {
            inputStream = new SerialPort(portName, portVal);
            inputStream.Open();
            isActive = true;
        }

    }

    private void OnApplicationQuit()
    {
        if(isActive)
        {
            isActive = false;
            inputStream.Close();
        }
    }

    // Update is called once per frame
    void Update()
    {   
        if (isActive)
        {
            receivedStream = inputStream.ReadLine();
            Debug.Log(receivedStream);

            if(receivedStream != string.Empty)
                InputManager.Instance.ReceiveNFCReader(receivedStream);
   
        } else
        {
            Debug.Log("Input stream closed");
        }
    }
}
