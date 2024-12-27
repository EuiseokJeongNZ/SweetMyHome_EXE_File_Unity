using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public RectTransform uiGroup;
    Player enterPlayer;

    public GameObject[] itemObj;
    public int[] itemPrice;
    public Transform[] itemPos;
    public string[] talkData;
    public TextMeshProUGUI talkText;

    public void Enter(Player player)
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        enterPlayer = player;
        uiGroup.anchoredPosition = Vector3.zero;

        enterPlayer.isShop = true;
    }

    // Update is called once per frame
    public void Exit()
    {
        uiGroup.anchoredPosition = Vector3.down * 1000;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public void Buy(int index)
    {
        int price = itemPrice[index];
        if(price > enterPlayer.coin)
        {
            StopCoroutine(Talk());
            StartCoroutine(Talk());
            return;
        }

        enterPlayer.coin -= price;
        Vector3 ranVec = Vector3.right * Random.Range(-3, 3)
                          + Vector3.forward * Random.Range(-3, 3);

        Instantiate(itemObj[index], itemPos[0].position, itemPos[0].rotation);
    }
    IEnumerator Talk()
    {
        talkText.text = talkData[1];
        yield return new WaitForSeconds(2f);
        talkText.text = talkData[0];
    }
}
