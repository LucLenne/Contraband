using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class Baron : MonoBehaviour
{

    [Header("References"),SerializeField] private TMP_Text _textSpeech;
    private const string _tableName = "Baron"; 
    [SerializeField] private GameObject _baron;
    
    [Header("Data"),SerializeField] private OrderSpeech _speechBaron;

    private void Start()
    {
        SpeechBaron(0);
    }

    private void DisplaySpeech(Speech speech)
    {
        _textSpeech.text = LocalizationSettings.StringDatabase.GetLocalizedString(_tableName, speech.id);
    }

    public async void SpeechBaron( int p)
    {
        if (p > 3 || p < 0) Debug.LogError("Wrong part : " + p + ". Choose Between 0 and 3");
        if (_speechBaron.listSpeech[p].Count == 0) return;
        _baron.SetActive(true);
        foreach (Speech speech in _speechBaron.listSpeech[p]) 
        {
            DisplaySpeech(speech);
            await Tasks.WaitSeconds(speech.time);
        }
        UnloadText();
        _baron.SetActive(false);
    }

    private void UnloadText()
    {
        _textSpeech.text = "";
    }
}

[System.Serializable]
public class Speech
{
    public int time;
    public string id;
}

[System.Serializable]
public class OrderSpeech
{
    public List<Speech> firstPart;
    public List<Speech> secondPart;
    public List<Speech> thirdPart;
    public List<Speech> fourthPart;

    public List<List<Speech>> listSpeech;

    public OrderSpeech() 
    { 
        listSpeech = new() { firstPart, secondPart, thirdPart, fourthPart };
    }
}