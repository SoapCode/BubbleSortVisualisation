using UnityEngine;
using DG.Tweening;
using System.Collections;

public class NewBehaviourScript : MonoBehaviour
{
    void Start()
    {/*
        DOTween.To(
            x => transform.DOMove(
                new Vector3(0f + Mathf.Sin(x) * 1f, 0, 0f + Mathf.Cos(x) * 1f + 6f),
                0),
            0, 2 * Mathf.PI, 1f);*/
     /* DOTween.To(
                      x => transform.DOMove(
                          new Vector3(0f + Mathf.Sin(Time.time) * 1f, 0, 0f + Mathf.Cos(Time.time) * 1f + 6f),
                          0),

 0, Mathf.PI, 1f).SetDelay(2f).SetLoops(-1).Delay();
 */

        StartCoroutine(Lolka());
    }

    IEnumerator Lolka()
    {
        int cnt = 0;
        while (cnt != 5)
        {
            transform.DOMoveX(3f, 1f);
            cnt++;
            yield return new WaitForSeconds(1f);
        }
    }
}
