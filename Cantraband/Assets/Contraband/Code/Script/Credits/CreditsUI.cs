using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsUI : MonoBehaviour
{
    private const string NAME_MAINMENU_LEVEL = "MainMenu";
    [SerializeField] private List<ParticleSystem> _ps;

    private void Start()
    {
        StartCoroutine(StartParticleSystem());
    }

    IEnumerator StartParticleSystem()
    {
        for (int i = 0; i < _ps.Count; i++)
        {
            _ps[i].Play();
            while (!_ps[i].IsAlive())
            {
                yield return null;
            }
            while (!_ps[i].gameObject.transform.GetChild(0).GetComponent<ParticleSystem>().IsAlive())
            {
                yield return null;
            }
            yield return new WaitForSeconds(2f);
        }
        ReturnMainMenu();
    }

    private void ReturnMainMenu() => LoadingManager.Instance.LoadScene(NAME_MAINMENU_LEVEL, false);

}
