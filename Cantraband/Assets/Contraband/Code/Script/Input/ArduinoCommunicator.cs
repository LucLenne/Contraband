using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System.Linq;
using Unity.VisualScripting;

public class ArduinoCommunicator : MonoBehaviour
{
    private string portNamePrefix = "COM";

    SerialPort inputStream;
    public int portVal = 9600;

    private string receivedStream;
    private bool isActive = false;

    private Coroutine _COMDetectedCoroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 4; i < 10; i++)
        {
            string portName = portNamePrefix + i.ToString();
            if (SerialPort.GetPortNames().ToList().Contains(portName))
            {
                inputStream = new SerialPort(portName, portVal);
                inputStream.Open();
                isActive = true;
            }
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
