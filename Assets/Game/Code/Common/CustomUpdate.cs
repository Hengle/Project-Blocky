using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomUpdate : MonoBehaviour {

    public static event EndOfFixedUpdate OnEndOfFixedUpdate;
    public delegate void EndOfFixedUpdate();
    
    private YieldInstruction yieldInstruction = new WaitForFixedUpdate();

    private void Start()
    {
        StartCoroutine(Coroutine_EndOfFixedUpdate());
    }

    private IEnumerator Coroutine_EndOfFixedUpdate()
    {
        while (true)
        {
            yield return yieldInstruction;
            OnEndOfFixedUpdate();
        }
    }

}
