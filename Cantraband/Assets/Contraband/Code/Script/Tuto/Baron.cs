using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

public class Baron : MonoBehaviour
{

    [Header("References"),SerializeField] private TMP_Text _textSpeech;
    private const string _tableName = "Baron"; 
    [SerializeField] private GameObject _baron;
    
    [Header("Data"),SerializeField] private OrderSpeech _speechBaron;
    
    
    private void DisplaySpeech(Speech speech, int indexOrder)
    {
        _textSpeech.text = LocalizationSettings.StringDatabase.GetLocalizedString(_tableName, speech.id);
    }


    private async Task ShowSpeech()
    {

    }



}

[System.Serializable]
public class Speech
{
    public float time;
    public string id;
}

[System.Serializable]
public class OrderSpeech
{
    public List<Speech> firstPart;
    public List<Speech> secondPart;
    public List<Speech> thirdPart;
    public List<Speech> fourthPart;
}