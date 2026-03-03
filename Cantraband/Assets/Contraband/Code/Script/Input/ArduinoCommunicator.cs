using System.IO.Ports;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class ArduinoCommunicator : MonoBehaviour
{

    public static ArduinoCommunicator Instance;

    public bool DebugMode = true;
    public bool DebugLED = true;
    private string portNamePrefix = "COM";
    private SerialPort inputStream;
    public int baudRate = 9600;

    private string receivedStream;
    private bool isActive = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    // Setup
    private void OnEnable()
    {
        if(LevelManager.Instance != null)
        {
            LevelManager.Instance.OnGameReturned += CallLed;
            LevelManager.Instance.OnReceiveCartridge += Wait;
            LevelManager.Instance.OnFailedByCop += Failed;
            LevelManager.Instance.OutOfPatience += Failed;
        }
    }

    void OnDisable()
    {
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OnGameReturned -= CallLed;
            LevelManager.Instance.OnReceiveCartridge -= Wait;
            LevelManager.Instance.OnFailedByCop -= Failed;
            LevelManager.Instance.OutOfPatience -= Failed;
        }

        CloseSerialPort();
    }

    void Start()
    {
        if(isActive)
        {
            Debug.Log("[ArduinoCommunicator] Connection already existing, no need to search for a port");
            return;
        }

        foreach (string portName in SerialPort.GetPortNames())
        {
            try
            {

                inputStream = new SerialPort(portName, baudRate);
                inputStream.ReadTimeout = 1000;
                inputStream.Open();
                Debug.Log("[ArduinoCommunicator] Found communication port: " + inputStream.PortName);

                // Handshake pour verif que le port est occupé par arduino
                System.Threading.Thread.Sleep(100);
                inputStream.WriteLine("CTRL_BAND_67");
                Debug.Log("[ArduinoCommunicator] Try handshake..  " + inputStream.PortName);
                string response = inputStream.ReadLine();
                if (response.Contains("ARDUINO_READY"))
                {
                    Debug.Log("[ArduinoCommunicator] Handshake validated, communication started with " + inputStream.PortName);
                    isActive = true;
                    break;
                }
                else
                {
                    Debug.Log("[ArduinoCommunicator] Handshake failed. Port " + inputStream.PortName + " doesnt run the ctrl + band software.");
                    inputStream.Close();
                }

            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[ArduinoCommunicator] Error while opening port {portName} : {e.Message}");
                if (inputStream != null && inputStream.IsOpen)
                    inputStream.Close();
            }
        }

        if (!isActive)
        {
            Debug.LogWarning("[ArduinoCommunicator] No valid communication port found...");
        } else
        {
            Debug.Log("[ArduinoCommunicator] Selected communication port: " + inputStream.PortName);
        }
    }

    // RFID Reading
    void Update()
    {
        if(DebugLED)
            DebugInputs();
        if (isActive && inputStream != null && inputStream.IsOpen)
        {
            try
            {
                receivedStream = inputStream.ReadLine();
                if (!string.IsNullOrEmpty(receivedStream))
                {
                    InputManager.Instance.ReceiveNFCReader(receivedStream);
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
                isActive = false; // Ne plus logguer � chaque frame
            }
        }
    }

    private void OnDestroy()
    {
        if(isActive)
            CloseSerialPort();
    }


    // Quit app
    void OnApplicationQuit()
    {
        CloseSerialPort();
    }

    private void CloseSerialPort()
    {
        if (inputStream != null && inputStream.IsOpen)
        {
            Debug.Log("[ArduinoCommunicator] Closing active port");
            inputStream.WriteLine("CTRL_BAND_STOP");
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

    // Led stuff

    private void Wait() // yellow? (maybe blinking)
    {
        //Debug.Log("LED WAIT -------------4---------------");
        //inputStream.Write("4");

    }

    private void Failed() // red
    {
        Debug.Log("LED FAIL -------------3---------------");
        if(inputStream != null)
        {
            try
            {
                inputStream.Write("3");
            }
            catch
            {
                Debug.Log("Fail to write");
            }
        }
    }
    private void Mid() // yellow
    {
        Debug.Log("LED MID -------------2---------------");
        if (inputStream != null)
        {
            try
            {
                inputStream.Write("2");
            }
            catch
            {
                Debug.Log("Fail to write");
            }
        }
    }
    private void Correct() // green
    {
        Debug.Log("LED CORRECT -------------1---------------");
        if (inputStream != null)
        {
            try
            {
                inputStream.Write("1");
            }
            catch
            {
                Debug.Log("Fail to write");
            }
        }
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
                Failed();
                Debug.Log(3);
            }
            if ((Keyboard.current[Key.Digit2].IsPressed()))
            {
                Mid();
                Debug.Log(2);
            }
            if (Keyboard.current[Key.Digit1].IsPressed())
            {
                Correct();
                Debug.Log(1);
            }
            if ((Keyboard.current[Key.Digit0].IsPressed()))
            {
                Wait();
                Debug.Log(0);
            }
        }
    }
}
