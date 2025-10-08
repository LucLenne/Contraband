using UnityEngine;
using System.Collections;
using System.IO.Ports;

public class ArduinoCommunicator : MonoBehaviour
{
    SerialPort inputStream = new SerialPort("COM4", 9600);
    private string receivedStream;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inputStream.Open();
    }

    private void OnApplicationQuit()
    {
        inputStream.Close();
    }

    // Update is called once per frame
    void Update()
    {
        receivedStream = inputStream.ReadLine();
        string[] datas = receivedStream.Split(',');
        /*if (receivedStream != null)
        Debug.Log("In stream from Freenove: " + receivedStream);*/
        
        if (datas.Length >= 2)
        {
            if (datas[0]?.Length > 0)
            {

                Debug.Log("Canal nfc: " + datas[0]);
            }
            if (datas[1]?.Length > 0)
                Debug.Log("Canal coatValue: " + datas[1]);
        }
    }
}
