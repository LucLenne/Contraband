using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [Header("Images")]
    [SerializeField] private List<Image> _imagesList;
    [SerializeField] private List<RawImage> _rawImagesList;
    [SerializeField] private List<TMP_Text> _tmpList;
    [Header("Fade in")]
    [SerializeField] private float _fadeInDuration;

    private void OnEnable()
    {
        //Fade in
        foreach (RawImage raw in _rawImagesList)
        {
            Color defaultColor = raw.color;
            defaultColor.a = 0f;
            raw.color = defaultColor;

            raw.DOFade(1, _fadeInDuration);
        }
        foreach (Image image in _imagesList)
        {
            Color defaultColor = image.color;
            defaultColor.a = 0f;
            image.color = defaultColor;

            image.DOFade(1, _fadeInDuration);
        }
        foreach (TMP_Text text in _tmpList)
        {
            Color defaultColor = text.color;
            defaultColor.a = 0f;
            text.color = defaultColor;

            text.DOFade(1, _fadeInDuration);
        }
    }
}
