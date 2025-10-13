using System.IO.Ports;
using System.Linq;
using UnityEngine;

public class ArduinoCommunicator : MonoBehaviour
{
    private string portNamePrefix = "COM";
    private SerialPort inputStream;
    public int baudRate = 9600;

    private string receivedStream;
    private bool isActive = false;

    void Start()
    {
        for (int i = 4; i < 10; i++)
        {
            string portName = portNamePrefix + i.ToString();
            if (SerialPort.GetPortNames().Contains(portName))
            {
                try
                {
                    inputStream = new SerialPort(portName, baudRate);
                    inputStream.ReadTimeout = 100;
                    inputStream.Open();
                    isActive = true;
                    break;
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"[ArduinoCommunicator] Erreur lors de l'ouverture du port {portName} : {e.Message}");
                }
            }
        }

        if (!isActive)
        {
            Debug.LogWarning("[ArduinoCommunicator] Aucun port série valide trouvé.");
        }
    }

    void Update()
    {
        if (isActive && inputStream != null && inputStream.IsOpen)
        {
            try
            {
                if (inputStream.BytesToRead > 0)
                {
                    receivedStream = inputStream.ReadLine();

                    if (!string.IsNullOrEmpty(receivedStream))
                    {
                        InputManager.Instance.ReceiveNFCReader(receivedStream);
                    }
                }
            }
            catch (System.TimeoutException)
            {
                // Ignorer, lecture non bloquante
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[ArduinoCommunicator] Erreur de lecture série : {e.Message}");
            }
        }
        else
        {
            if (!isActive)
            {
                isActive = false; // Ne plus logguer à chaque frame
            }
        }
    }

    void OnDisable()
    {
        CloseSerialPort();
    }

    void OnApplicationQuit()
    {
        CloseSerialPort();
    }

    private void CloseSerialPort()
    {
        if (inputStream != null && inputStream.IsOpen)
        {
            try
            {
                inputStream.Close();
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[ArduinoCommunicator] Erreur lors de la fermeture du port : {e.Message}");
            }
        }
        isActive = false;
    }
}
