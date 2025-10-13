using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class Baron : MonoBehaviour
{

    [Header("References"), SerializeField] private TMP_Text _textSpeech;
    private const string _tableName = "Baron";
    [SerializeField] private GameObject _baron;

    [Header("Data"), SerializeField] private OrderSpeech _speechBaron;


    private void Awake()
    {
        InitListSpeech();
    }

    private void DisplaySpeech(Speech speech)
    {
        _textSpeech.text = LocalizationSettings.StringDatabase.GetLocalizedString(_tableName, speech.id);
    }

    void InitListSpeech()
    {
        _speechBaron.listSpeech = new() { _speechBaron.firstPart, _speechBaron.secondPart, _speechBaron.thirdPart, _speechBaron.fourthPart };
    }



    public IEnumerator SpeechBaronCoroutine(int p, float timeBeforeStartSpeech)
    {
        yield return new WaitForSeconds(timeBeforeStartSpeech);
        if (p > 3 || p < 0)
        {
            Debug.LogError("Wrong part : " + p + ". Choose between 0 and 3");
            yield break;
        }

        if (_speechBaron.listSpeech[p].Count == 0)
            yield break;

        _baron.SetActive(true);

        foreach (Speech speech in _speechBaron.listSpeech[p])
        {
            DisplaySpeech(speech);
            yield return new WaitForSeconds(speech.time);
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


    public List<List<Speech>> listSpeech;
}