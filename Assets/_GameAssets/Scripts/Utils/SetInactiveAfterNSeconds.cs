using System.Collections;
using UnityEngine;

public class SetInactiveAfterNSeconds : MonoBehaviour
{
    [SerializeField] private float seconds;
    
    private void OnEnable()
    {
        StartCoroutine(SetInactiveAfterN());
    }

    private IEnumerator SetInactiveAfterN()
    {
        yield return new WaitForSeconds(seconds);
        gameObject.SetActive(false);
    }
}
