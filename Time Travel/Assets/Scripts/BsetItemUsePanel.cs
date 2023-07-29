using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class BsetItemUsePanel : MonoBehaviour
{
    int time;
    public TMP_Text TimeText;
    public TMP_Text useOrNotText;
    public Button yesButton;
    public Button noButton;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEnable()
    {
        //��� B��Ʈ ī�尡 ���� ��, �ٷ� ���� Ǯ�̷� �Ѿ�� �ڵ� �߰��� ��.
        //ī�� ���ѾƼ� B��Ʈ 2�� �̻� ������ ���� ����� �ڵ� �߰��� ��.
        if (GameManager.instance.controlPlayer == PhotonNetwork.LocalPlayer)
        {
            useOrNotText.text = "������ Ǫ�� �÷��̾�� B��Ʈ ī�带 �� �� �����ϴ�.\n �ٸ� ����� ������ ��ٸ��ϴ�...";
        }
        else
        {
            List<GameManager.items> playerCards = GameManager.instance.player.itemCards;
            if (playerCards.Contains(GameManager.items.cardSteal))
            {
                useOrNotText.text = "ī�� ���ѱ� ī�带 ����Ͻðڽ��ϱ�?";
                yesButton.gameObject.SetActive(true);
                noButton.gameObject.SetActive(true);
            }
            else if (playerCards.Contains(GameManager.items.timeSteal))
            {
                useOrNotText.text = "�ð� ���ѱ� ī�带 ����Ͻðڽ��ϱ�?";
                yesButton.gameObject.SetActive(true);
                noButton.gameObject.SetActive(true);
            }
            else if (playerCards.Contains(GameManager.items.bind))
            {
                useOrNotText.text = "�������ü ī�带 ����Ͻðڽ��ϱ�?";
                yesButton.gameObject.SetActive(true);
                noButton.gameObject.SetActive(true);
            }
            else
            {
                useOrNotText.text = "�� �� �ִ� �������� �����ϴ�. \n �ٸ� ����� ������ ��ٸ��ϴ�...";
            }
        }
        time = 5;
        StartCoroutine("setTimer");
    }

    void OnDisable()
    {
        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);
    }

    IEnumerator setTimer()
    {
        while (time >= 0)
        {
            TimeText.text = "���� �������: " + time.ToString() + "�� ���ҽ��ϴ�.";
            time -= 1;
            yield return new WaitForSeconds(1.0f);
        }
        this.gameObject.SetActive(false);
    }
}
