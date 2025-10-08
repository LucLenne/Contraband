using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private List<Image> _imagesList;
    [SerializeField] private List<TMP_Text> _tmpList;
    [Header("Fade in")]
    [SerializeField] private float _fadeInDuration;

    private void OnEnable()
    {
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
