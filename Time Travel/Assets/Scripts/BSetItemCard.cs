using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class BSetItemCard : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    bool isSelected;
    bool canNotUse;
    public GameObject textPanel;
    public TMP_Text itemText;
    public string spriteName;
    public BsetItemUsePanel panelScript;
    // Start is called before the first frame update
    void Start()
    {
        spriteName = this.gameObject.GetComponent<Image>().sprite.name;
        textPanel = transform.parent.parent.GetChild(3).gameObject;
        itemText = textPanel.transform.GetChild(0).GetComponent<TMP_Text>();
        textPanel.gameObject.SetActive(false);
        //setItemText();
        if (spriteName == "ī�廩�ѱ�" && canStealCard() == false)
        {
            changeColorBlack();
            canNotUse = true;
        }
        else
        {
            isSelected = false;
            canNotUse = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (spriteName == "ī�廩�ѱ�" && canStealCard() == false)
        {
            canNotUse = true;
            changeColorBlack();
        }
        else
        {
            if (isSelected == false && RpcManager.instance.currentTurnUsedItemOfLocalPlayer != "")
            {
                canNotUse = true;
                changeColorBlack();
            }
            if (isSelected == false && RpcManager.instance.currentTurnUsedItemOfLocalPlayer == "")
            {
                canNotUse = false;
                Color whiteColor = new Color(255 / 255f, 255 / 255f, 255 / 255f);
                this.gameObject.GetComponent<Image>().color = whiteColor;
            }
            if (RpcManager.instance.isSomeoneUseCardSteal == true)
            {
                canNotUse = true;
                changeColorBlack();
            }
        }
    }

    void OnDisable()
    {
        if (isSelected == true)
        {
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
        }
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
        if (isSelected== true)
        {
            Color whiteColor = new Color(255 / 255f, 255 / 255f, 255 / 255f);
            this.gameObject.GetComponent<Image>().color = whiteColor;
            isSelected= false;
            transform.GetChild(0).gameObject.SetActive(false);
            RpcManager.instance.currentTurnUsedItemOfLocalPlayer = "";
            return;
        }
        if (canNotUse == true || RpcManager.instance.currentTurnUsedItemOfLocalPlayer != "")
        {
            return;
        }
        Color usedColor = new Color(200 / 255f, 200 / 255f, 200 / 255f);
        this.gameObject.GetComponent<Image>().color = usedColor;
        isSelected= true;
        transform.GetChild(0).gameObject.SetActive(true);
        RpcManager.instance.currentTurnUsedItemOfLocalPlayer = spriteName;
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
