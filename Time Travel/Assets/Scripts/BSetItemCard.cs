using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BSetItemCard : MonoBehaviour, IPointerClickHandler
{
    bool isUsed;
    // Start is called before the first frame update
    void Start()
    {
        isUsed = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDisable()
    {
        if (isUsed == true)
        {
            Destroy(this.gameObject);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isUsed == true)
        {
            return;
        }
        string spriteName = this.gameObject.GetComponent<Image>().sprite.name;
        Color usedColor = new Color(100 / 255f, 100 / 255f, 100 / 255f);
        this.gameObject.GetComponent<Image>().color = usedColor;
        isUsed = true;
        RpcManager.instance.currentTurnUsedItemOfLocalPlayer = spriteName;
        if (spriteName == "������ü")
        {
            RpcManager.instance.useBsetItemCard(DontDestroyObjects.items.bind);
            //�ο� ����� �÷��̾� ���� �߰��Ǹ�  �ڵ� �߰�.
        }
        else if (spriteName == "ī�廩�ѱ�")  //Ŭ�����ڸ��� ������ �÷��̾� UI���� ������µ� resultPanel �߸� ��������� ���� �ʿ�. ���� �߰��� ������ UI ũ�� ���� �ʿ�.
        {
            int controlPlayerCardNum = DontDestroyObjects.instance.playerItems[GameManager.instance.controlPlayerIndexWithOrder].Count;
            int stealCardIndex = Random.Range(0, controlPlayerCardNum);
            RpcManager.instance.useBsetItemCard(DontDestroyObjects.items.cardSteal); 
            RpcManager.instance.cardSteal(GameManager.instance.localPlayerIndexWithOrder, stealCardIndex);
        }
        else
        {
            RpcManager.instance.useBsetItemCard(DontDestroyObjects.items.timeSteal);
            RpcManager.instance.setIsThisTurnTimeStealTrue();
        }
    }
}
