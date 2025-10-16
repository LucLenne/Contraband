using System.IO.Ports;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class ArduinoWriteTest : MonoBehaviour
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

    private void Awake()
    {
        LevelManager.Instance.OnGameReturned += CallLed;
        LevelManager.Instance.OnReceiveCartridge += Wait;
        LevelManager.Instance.OnFailedByCop += Failed;
        LevelManager.Instance.OutOfPatience += Failed;
    }
    private void OnDisable()
    {
        LevelManager.Instance.OnGameReturned -= CallLed;
        LevelManager.Instance.OnReceiveCartridge -= Wait;
        LevelManager.Instance.OnFailedByCop -= Failed;
        LevelManager.Instance.OutOfPatience -= Failed;
    }

    private void Wait() // yellow? (maybe blinking)
    {
        inputStream.Write("4");

    }

    private void Failed() // red
    {
        inputStream.Write("3");
    }
    private void Mid() // yellow
    {
        inputStream.Write("2");
    }
    private void Correct() // green
    {
        inputStream.Write("1");
    }
    
    private void CallLed(LevelManager.GameReturnedType type)
    {
        switch (type)
        {
            case LevelManager.GameReturnedType.Wrong:
                Failed();
                return;
            case LevelManager.GameReturnedType.Good:
                Mid();
                return;
            case LevelManager.GameReturnedType.Favorite:
                Correct();
                return;


        }
    }


    private void DebugInputs()
    {
        if (inputStream.IsOpen)
        {
            if ((Keyboard.current[Key.Digit3].IsPressed()))
            {
                inputStream.Write("3");
                Debug.Log(3);
            }
            if ((Keyboard.current[Key.Digit2].IsPressed()))
            {
                inputStream.Write("2");
                Debug.Log(2);
            }
            if (Keyboard.current[Key.Digit1].IsPressed())
            {
                inputStream.Write("1");
                Debug.Log(1);
            }
            if ((Keyboard.current[Key.Digit0].IsPressed()))
            {
                inputStream.Write("0");
                Debug.Log(0);
            }
        }
    }

    void Update()
    {
        DebugInputs();
    }
}
