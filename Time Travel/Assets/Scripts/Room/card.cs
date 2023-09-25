using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class card : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject textPanel;
    public TMP_Text cardDescriptionText;

    // Start is called before the first frame update
    void Start()
    {
        textPanel.gameObject.SetActive(false);
        setItemText();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void setItemText()
    {
        Sprite itemSprite = GetComponent<Image>().sprite;
        if (itemSprite == RoomManager.instance.itemImg[0])
        {
            cardDescriptionText.text = "�𸣴� ������ ��Ʈ�� ���� �� �ֽ��ϴ�. \n";
        }
        else if (itemSprite == RoomManager.instance.itemImg[1])
        {
            cardDescriptionText.text = "�𸣴� ������ �н��� �� �ֽ��ϴ�.\n������ ���� ó���ǰ� ��� ������� ���� �����˴ϴ�.";
        }
        else if (itemSprite == RoomManager.instance.itemImg[2])
        {
            cardDescriptionText.text = "���� �� �ϳ��� �������� �����մϴ�.\n4�������� �������� ����� �����մϴ�.";
        }
        else if (itemSprite == RoomManager.instance.itemImg[3])
        {
            cardDescriptionText.text = "������ ���� Ǫ�� �ð��� ���� �� �ֽ��ϴ�.\n������ �� ����, ������ ���� �ð�����\n�� ī�带 �� �� �ֽ��ϴ�.";
        }
        else if (itemSprite == RoomManager.instance.itemImg[4])
        {
            cardDescriptionText.text = "������ ������ �����ٸ� �ڽŵ� \n�ֻ��� ���� �ش��ϴ� ĭ��ŭ ���ư��ϴ�.\n������ �� ����, ������ ���� �ð�����\n�� ī�带 �� �� �ֽ��ϴ�.";
        }
        else
        {
            cardDescriptionText.text = "������ ī�� 1���� ���Ѿ� �����ɴϴ�.\n�� ī�尡 4���̸� �� �� �����ϴ�.\n������ �� ����, ������ ���� �ð�����\n�� ī�带 �� �� �ֽ��ϴ�.";
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        textPanel.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        textPanel.SetActive(false);
    }
}
