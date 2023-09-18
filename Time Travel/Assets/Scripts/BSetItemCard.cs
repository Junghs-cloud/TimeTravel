using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class BSetItemCard : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    bool isUsed;
    bool canNotUse;
    public GameObject textPanel;
    public TMP_Text itemText;

    public BsetItemUsePanel panelScript;
    // Start is called before the first frame update
    void Start()
    {
        string spriteName = this.gameObject.GetComponent<Image>().sprite.name;
        textPanel = transform.parent.parent.GetChild(3).gameObject;
        itemText = textPanel.transform.GetChild(0).GetComponent<TMP_Text>();

        //setItemText();
        if (spriteName == "ī�廩�ѱ�" && canStealCard() == false)
        {
            changeColorBlack();
            canNotUse = true;
        }
        else
        {
            isUsed = false;
            canNotUse = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.currentTurnBSetItem > 0)
        {
            canNotUse = true;
            changeColorBlack();
        }
        if (RpcManager.instance.isSomeoneUseCardSteal == true)
        {
            canNotUse = true;
            changeColorBlack();
        }
    }

    void OnDisable()
    {
        Destroy(this.gameObject);
    }

    void setItemText()
    {
        string spriteName = this.gameObject.GetComponent<Image>().sprite.name;
        if (spriteName == "ī�廩�ѱ�")
        {
            itemText.text = "������ ī�� 1���� ���Ѿ� �����ɴϴ�.\n������ ī�尡 ���ų� �� ī�尡 4���̸� �� �� �����ϴ�.\n";
        }
        else if (spriteName == "������ü")
        {
            itemText.text = "������ ������ �����ٸ� ���� �ֻ��� ���� �ش��ϴ� ĭ��ŭ ���ư��ϴ�.\n";
        }
        else
        {
            itemText.text = "������ ���� Ǫ�� �ð��� ���� �� �ֽ��ϴ�. \n ox ������ 8��, 4������ ������ 15�ʷ� �پ��ϴ�.\n";
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        setItemText();
        textPanel.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        textPanel.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isUsed == true || canNotUse==true || GameManager.instance.currentTurnBSetItem > 0)
        {
            return;
        }
        string spriteName = this.gameObject.GetComponent<Image>().sprite.name;
        changeColorBlack();
        isUsed = true;
        RpcManager.instance.currentTurnUsedItemOfLocalPlayer = spriteName;
        if (spriteName == "������ü")
        {
            RpcManager.instance.useBsetItemCard(DontDestroyObjects.items.bind);
            RpcManager.instance.makeIsUsedBindTrue(GameManager.instance.localPlayerIndexWithOrder);
        }
        else if (spriteName == "ī�廩�ѱ�")
        {
            if (RpcManager.instance.isSomeoneUseCardSteal == false)
            {
                RpcManager.instance.useBsetItemCard(DontDestroyObjects.items.cardSteal);
            }
            else
            {
                return;
            }

        }
        else
        {
            RpcManager.instance.useBsetItemCard(DontDestroyObjects.items.timeSteal);
            RpcManager.instance.setIsThisTurnTimeStealTrue();
        }
        GameManager.instance.currentTurnBSetItem++;
    }

    bool canStealCard()
    {
        if (DontDestroyObjects.instance.playerItems[GameManager.instance.localPlayerIndexWithOrder].Count == 4 || DontDestroyObjects.instance.playerItems[GameManager.instance.controlPlayerIndexWithOrder].Count == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    void changeColorBlack()
    {
        Color usedColor = new Color(100 / 255f, 100 / 255f, 100 / 255f);
        this.gameObject.GetComponent<Image>().color = usedColor;
    }
}
