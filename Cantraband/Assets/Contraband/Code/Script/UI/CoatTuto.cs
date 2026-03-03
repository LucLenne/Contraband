using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CoatTuto : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI _textTuto;
    [SerializeField] private GameObject _ledPrefab;
    [SerializeField] private int _nbLed;
    [Space]
    [SerializeField] private Color _colorInactive;
    [SerializeField] private Color _colorClose;
    [SerializeField] private Color _colorOpen;
    [Space]
    [SerializeField] private string _textOpenCoat;
    [SerializeField] private string _textCloseCoat;
    [SerializeField] private string _textEndTuto;
    [Space]
    [SerializeField] private int _timeBeforeLeavingTuto;


    private int _indexLed;
    private int _indexColor;
    private Transform _parentLedsTransf;
    private const string SCENE_TO_LOAD = "Game";

    private bool _once;

    private void Start()
    {
        _parentLedsTransf = transform.parent.GetChild(2).transform;
        for (int i = 0; i < _nbLed; i++)
        {
            GameObject led = Instantiate(_ledPrefab, _parentLedsTransf);
            led.GetComponent<Image>().color = _colorInactive;
        }
        _textTuto.text = _textCloseCoat;
    }

    private void Update()
    {
        if (_parentLedsTransf.transform.childCount == 0) 
            return;
        if (_indexLed != _parentLedsTransf.transform.childCount)
        {
            switch (_indexColor)
            {
                case 0:
                    if (!InputManager.Instance.IsVestOpened)
                    {
                        SetColorLed(_colorClose);
                        _indexColor += 1;
                        _textTuto.text = _textOpenCoat;
                    }
                    break;
                case 1:
                    if (InputManager.Instance.IsVestOpened)
                    {
                        _indexColor += 1;
                        SetColorLed(_colorOpen);
                        _indexLed += 1;
                        _indexColor = 0;
                        _textTuto.text = _textCloseCoat;
                    }
                    break;
            }
        }
        else
        {
            if(!_once)
                StartCoroutine(ValidCoatTuto());
        }
    }

    void SetColorLed(Color color)
    {
        _parentLedsTransf.GetChild(_indexLed).GetComponent<Image>().color = color;
    }

    IEnumerator ValidCoatTuto()
    {
        _once = true;
        _textTuto.text = _textEndTuto;
        yield return new WaitForSeconds(_timeBeforeLeavingTuto);
        LoadingManager.Instance.LoadScene(SCENE_TO_LOAD, true);
    }
}
