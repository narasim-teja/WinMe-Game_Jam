using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Amazon.Lambda.Model;

public class PopUpMessage : MonoBehaviour
{
    public Image PopUpPanelImage;
    public TextMeshProUGUI UIText;

    void Start()
    {
        StartCoroutine(deleteTimer());
    }

    public void UpdatePopUpMessage(string popUpMsg){
        UIText.text = popUpMsg;
    }

    IEnumerator deleteTimer(){
        yield return new WaitForSeconds(2.5f);
        float timeTillDestroy = 1f;
        float timeElapsed = 0f;

        float startAlpha = this.gameObject.GetComponent<CanvasGroup>().alpha;
        while (timeElapsed < timeTillDestroy)
        {
            timeElapsed += Time.deltaTime;
            this.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(startAlpha, 0f, timeElapsed / timeTillDestroy); 
            yield return null;
        }

        Destroy(this.gameObject);
    }
}
