using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Photon.Pun.UtilityScripts;
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int[] playerStartPoint;

    public int playerOrder;

    public Player[] player;
    public int newDiceSide;
    public bool timerOn;
    public bool isLadder;
    public bool isTransport;
    public bool finishRound;
    public string spaceCategory;
    public int correctCount;
    public bool secondRoll;//���� 5���϶� 
    public bool isOver;



    [Header("UI")]
    public GameObject canvas;
    public Canvas problemCanvas;
    public Dice dice;
    public Space spaceAction;
    public Text diceTimer;
    public Text spaceText;
    public TMP_Text startRoundText;
    public GameObject startRoundPanel;
    public GameObject space;
    public GameObject diceImg;
    public Image[] gaugeImg;
    public GameObject[] playerInformationUIs;
    public GameObject itemUsePanel;
    public GameObject itemUseResultPanel;
    public Photon.Realtime.Player controlPlayer; //���� Ǫ�� ���. ���� ������ �÷��̾�.
    public Sprite[] itemSmallSprites;
    public problem problemMaker;
    public GameObject endPanel;
    public GameObject endGameText;
    public string winner;
    public TMP_Text winnerName;
    public bool nextTurn;

    public TMP_Text testTMP;
    public int controlPlayerIndexWithOrder;
    public int localPlayerIndexWithOrder;
    public bool isThisTurnTimeSteal;
    public bool isUsedBind = false;
    public bool isMovableWithBind = false;
    public int bindPlayerIndex;
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        problemMaker.problemData= CSVReader.Read("����");
        problemMaker.answerData = CSVReader.Read("��");
        problemMaker.problemScript = problemMaker.gameObject.GetComponent<problemGraph>();
        playerStartPoint = new int[PhotonNetwork.CurrentRoom.PlayerCount];
        for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {
            playerStartPoint[i] = 0;
        }
        setPlayerInformationUIs();
        setPlayerPieces();
        setLocalPlayerIndexWithOrder();
        isThisTurnTimeSteal = false;
        isUsedBind = false;
        isMovableWithBind = false;
        controlPlayerIndexWithOrder = 0;
        controlPlayer = DontDestroyObjects.instance.playerListWithOrder[0];

    }
    // Start is called before the first frame update
    void Start()
    {
        Invoke("RoundStart", 1.5f);
    }

    // Update is called once per frame
    void Update()
    {
        //���� ����
        if (isOver)
            return;
        //when to stop moving
        if (player[controlPlayerIndexWithOrder].curIndex > playerStartPoint[controlPlayerIndexWithOrder] + newDiceSide)
        {
            player[controlPlayerIndexWithOrder].movingAllowed = false;
            playerStartPoint[controlPlayerIndexWithOrder] = player[controlPlayerIndexWithOrder].curIndex - 1;

            if (secondRoll)
            {
                secondRoll = false;
                finishRound = true;
                StartCoroutine(UISmallerRoutine(true));
            }
                
            //���� 5���̸� �ֻ��� �ѹ� �� ������
            if (player[GameManager.instance.controlPlayerIndexWithOrder].correctCount == 5)
            {
                player[GameManager.instance.controlPlayerIndexWithOrder].correctCount = 0;
                StartCoroutine(RollDiceAndGetItem());
            }
            if (isLadder && !player[controlPlayerIndexWithOrder].movingAllowed)
            {
                player[controlPlayerIndexWithOrder].moveLadder = true;
            }
            else if (isTransport && !player[controlPlayerIndexWithOrder].movingAllowed)
            {
                player[controlPlayerIndexWithOrder].Transport();
            }
            else
            {
                if (isMovableWithBind == true)
                {
                    updatePlayerInformationUI(controlPlayerIndexWithOrder);
                    RpcManager.instance.isMovableWithBind = true;
                    moveBindPlayer(RpcManager.instance.bindPlayerIndex);
                }
                else if(!secondRoll)
                {
                    finishRound = true;
                    StartCoroutine(UISmallerRoutine(true));
                }
            }

        }

        if (finishRound && nextTurn)
        {
            finishRound = false;
            isMovableWithBind = false;
            isUsedBind = false;
            if (controlPlayer == PhotonNetwork.LocalPlayer)
            {
                updatePlayerInformationUI(controlPlayerIndexWithOrder);
            }
            controlPlayerIndexWithOrder++;
            if (controlPlayerIndexWithOrder == PhotonNetwork.CurrentRoom.PlayerCount)
            {
                controlPlayerIndexWithOrder = 0;
            }
            controlPlayer = DontDestroyObjects.instance.playerListWithOrder[controlPlayerIndexWithOrder];
            Invoke("RoundStart", 1);
        }
    }

    IEnumerator UIBiggerRoutine(bool bigger)
    {
        float time = 0f;

        while (bigger)
        {
            playerInformationUIs[controlPlayerIndexWithOrder].transform.localScale = Vector3.one * (1 + time);
            time += Time.deltaTime;
            if (time > 0.3f)
            {
                bigger = false;
            }
            yield return null;
        }
    }

    IEnumerator UISmallerRoutine(bool smaller)
    {
        float time = 0f;
        while (smaller)
        {
            playerInformationUIs[controlPlayerIndexWithOrder].transform.localScale = Vector3.one * (1.3f - time);
            time += Time.deltaTime;
            if (time > 0.3f)
            {
                smaller = false;
            }
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        nextTurn = true;

    }

    public void setPlayerInformationUIs()
    {
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        for (int i = 0; i < playerCount; i++)
        {
            DontDestroyObjects.items[] arr = DontDestroyObjects.instance.playerItems[i].ToArray();
            for (int j = 0; j< 4; j++)
            {
                if (arr[j] == DontDestroyObjects.items.hint)
                {
                    playerInformationUIs[i].transform.GetChild(1).GetChild(j).GetComponent<Image>().sprite = itemSmallSprites[0];
                }
                else if (arr[j] == DontDestroyObjects.items.erase)
                {
                    playerInformationUIs[i].transform.GetChild(1).GetChild(j).GetComponent<Image>().sprite = itemSmallSprites[1];
                }
                else if (arr[j] == DontDestroyObjects.items.pass)
                {
                    playerInformationUIs[i].transform.GetChild(1).GetChild(j).GetComponent<Image>().sprite = itemSmallSprites[2];
                }
                else if (arr[j] == DontDestroyObjects.items.cardSteal)
                {
                    playerInformationUIs[i].transform.GetChild(1).GetChild(j).GetComponent<Image>().sprite = itemSmallSprites[3];
                }
                else if (arr[j] == DontDestroyObjects.items.timeSteal)
                {
                    playerInformationUIs[i].transform.GetChild(1).GetChild(j).GetComponent<Image>().sprite = itemSmallSprites[4];
                }
                else
                {
                    playerInformationUIs[i].transform.GetChild(1).GetChild(j).GetComponent<Image>().sprite = itemSmallSprites[5];
                }
            }
            playerInformationUIs[i].transform.GetChild(0).GetComponent<TMP_Text>().text = DontDestroyObjects.instance.playerListWithOrder[i].NickName;

           playerInformationUIs[i].SetActive(true);
        }
    }

    void setPlayerPieces()
    {
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        for (int i = 0; i < playerCount; i++)
        {
            player[i].gameObject.SetActive(true);
        }
    }

    public void RoundStart()
    {
        nextTurn = false;
        StartCoroutine(UIBiggerRoutine(true));
        //����..?
        if (correctCount != 5)
            secondRoll = false;
        player[controlPlayerIndexWithOrder].moveLadder = false;
        finishRound = false;
        StartCoroutine(RoundStartRoutine());
        /*RpcManager.instance.setDiceTrue();
        timerOn = true;*/
    }

    IEnumerator RoundStartRoutine()
    {
        startRoundText.text = controlPlayer.NickName + "�� �÷��̸� �����մϴ�."; //���� ����
        startRoundPanel.SetActive(true);

        yield return new WaitForSeconds(1.5f);

        startRoundPanel.SetActive(false);

        yield return new WaitForSeconds(1f);

        RpcManager.instance.setDiceTrue();
        timerOn = true;
    }



    public void MovePlayer()
    {
        player[controlPlayerIndexWithOrder].movingAllowed = true;
    }

    public void moveBindPlayer(int bindPlayerIndex)
    {
        player[bindPlayerIndex].movingAllowed = true;
    }
        
    //check
    public int getPlayerNextPosition()
    {
        return player[controlPlayerIndexWithOrder].curIndex + newDiceSide;
    }

    public void useItemCard(DontDestroyObjects.items itemName)
    {
        DontDestroyObjects.instance.playerItems[controlPlayerIndexWithOrder].Remove(itemName);
        RpcManager.instance.eraseItem(controlPlayerIndexWithOrder, itemName);
    }

    public void RpcCheck(string s)
    {
        testTMP.text = s;
    }

    public void CheckCurPoint(int diceNum)
    {
        switch (diceNum)
        {
            case 3:
            case 8:
            case 11:
            case 12:
            case 19:
            case 24:
            case 28:
            case 34:
            case 42:
            case 43:
            case 49:
            case 50:
            case 55:
            case 58:
            case 70:
            case 74:
            case 78:
            case 82:
            case 91:
                spaceCategory = "Nothing";
                spaceText.text = "���� ĭ�� �� ĭ�Դϴ�.\n �ٷ� �̵������մϴ�.";
                break;
            case 7:
            case 22:
            case 53:
            case 64:
            case 76:
                spaceCategory = "Ladder";
                spaceText.text = "���� ĭ�� ��ٸ� ĭ�Դϴ�.\n ��ٸ��� Ÿ�� �̵��մϴ�.";
                break;
            case 15:
            case 30:
            case 26:
            case 38:
            case 32:
            case 80:
            case 46:
            case 67:
            case 62:
            case 90:
                spaceCategory = "Portal";
                spaceText.text = "���� ĭ�� ���� ĭ�Դϴ�.\n ���� ������ ���з� �̵��մϴ�.";
                break;
            default:
                spaceCategory = "Problem";
                spaceText.text = "���� ĭ�� ���� ĭ�Դϴ�.\n ������ ���߸� �̵������մϴ�.";
                break;
        }
        space.SetActive(true);

        StartCoroutine(DoActionRoutine());
        
    }

    IEnumerator DoActionRoutine()
    {
        yield return new WaitForSeconds(1.5f);
        space.SetActive(false);

        if(spaceCategory == "Problem")
        {
            activeItemUsePanel();
            yield return new WaitForSeconds(6.0f);
            activeItemUseResultPanel();
            yield return new WaitForSeconds(3.5f);
        }
        else
            yield return new WaitForSeconds(1.5f);
        spaceAction.DoAction(spaceCategory);
    }

    public void UpdateGaugeImg()
    {
        for (int i = 0; i < player[GameManager.instance.controlPlayerIndexWithOrder].correctCount; i++)
        {
            playerInformationUIs[controlPlayerIndexWithOrder].transform.Find("Gauge Bar").GetChild(i).GetComponent<Image>().color = new Color(1, 1, 1, 1);

        }
    }

    void ResetGaugeImg()
    {
        for (int i = 0; i < 5; i++)
        {
            playerInformationUIs[controlPlayerIndexWithOrder].transform.Find("Gauge Bar").GetChild(i).GetComponent<Image>().color = new Color(1, 1, 1, 0.3f);
        }
    }



    IEnumerator RollDiceAndGetItem()
    {

        Debug.Log("������ ����" + DontDestroyObjects.instance.playerItems[controlPlayerIndexWithOrder].Count);
        if (DontDestroyObjects.instance.playerItems[controlPlayerIndexWithOrder].Count < 4) //�������ִ� ������ ������ 3�������϶�
        {
            spaceText.text = "BSet ������ �ϳ��� �߰��� ȹ���մϴ�!";
            space.SetActive(true);

            int ran = Random.Range(3, 6);

            if (ran == 3)
            {
                DontDestroyObjects.instance.playerItems[controlPlayerIndexWithOrder].Add(DontDestroyObjects.items.cardSteal);
                playerInformationUIs[controlPlayerIndexWithOrder].transform.GetChild(1).GetChild(3).GetComponent<Image>().sprite = itemSmallSprites[3];
            }
            else if (ran == 4)
            {
                DontDestroyObjects.instance.playerItems[controlPlayerIndexWithOrder].Add(DontDestroyObjects.items.bind);
                playerInformationUIs[controlPlayerIndexWithOrder].transform.GetChild(1).GetChild(3).GetComponent<Image>().sprite = itemSmallSprites[4];
            }
            else
            {
                DontDestroyObjects.instance.playerItems[controlPlayerIndexWithOrder].Add(DontDestroyObjects.items.timeSteal);
                playerInformationUIs[controlPlayerIndexWithOrder].transform.GetChild(1).GetChild(3).GetComponent<Image>().sprite = itemSmallSprites[5];
            }

            yield return new WaitForSeconds(1f);

        }
            

        secondRoll = true;
        spaceText.text = "..�����߰�...? �ֻ����� �ѹ� �� ���� �� �ֽ��ϴ�!";
        space.SetActive(true);

        yield return new WaitForSeconds(1.5f);
        space.SetActive(false);
        ResetGaugeImg();

        yield return new WaitForSeconds(2f);
        diceImg.SetActive(true);
        diceTimer.gameObject.SetActive(true);
        timerOn = true;
    }

    public void EndGame()
    {
        winner = PhotonNetwork.LocalPlayer.ToString();
        isOver = true;
        StartCoroutine(EndGameRoutine());
    }

    IEnumerator EndGameRoutine()
    {
        endGameText.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        endGameText.SetActive(false);
        RpcManager.instance.ShowEndPanel();

    }

    public void ReturnToLobby()
    {
        SceneManager.LoadScene("Main");
    }


    public void updatePlayerInformationUI(int controlPlayerIndex)
    {
        playerInformationUIs[controlPlayerIndex].GetComponent<playerInformationUI>().updatePlayerBoardNum(player[controlPlayerIndex].curIndex - 1);
    }


    void setLocalPlayerIndexWithOrder()
    {
        for (int i = 0; i < PhotonNetwork.CurrentRoom.MaxPlayers; i++)
        {
            if (DontDestroyObjects.instance.playerListWithOrder[i] == PhotonNetwork.LocalPlayer)
            {
                localPlayerIndexWithOrder = i;
                break;
            }
        }
    }

    public void activeItemUsePanel()
    {
        itemUsePanel.SetActive(true);
    }

    public void activeItemUseResultPanel()
    {
        itemUseResultPanel.SetActive(true);
    }

    
    public void eraseItemUI(int playerIndex, int cardIndex)
    {
        GameObject itemPanel = playerInformationUIs[controlPlayerIndexWithOrder].transform.GetChild(1).gameObject;
        GameObject stolenCard = itemPanel.transform.GetChild(cardIndex).gameObject;
        string itemName = stolenCard.GetComponent<Image>().sprite.name;
        GameObject itemUIPrefab = Resources.Load<GameObject>("Prefabs/itemImageUI");
        Destroy(stolenCard);
        if (itemName == "������ü UI")
        {
            DontDestroyObjects.instance.playerItems[controlPlayerIndexWithOrder].Remove(DontDestroyObjects.items.bind);
            DontDestroyObjects.instance.playerItems[playerIndex].Add(DontDestroyObjects.items.bind);
            GameObject createdItem = Instantiate(itemUIPrefab);
            createdItem.transform.SetParent(playerInformationUIs[playerIndex].transform.GetChild(1), false);
            createdItem.GetComponent<Image>().sprite = itemSmallSprites[5];
        }
        else if (itemName == "ī�廩�ѱ�")
        {
            DontDestroyObjects.instance.playerItems[controlPlayerIndexWithOrder].Remove(DontDestroyObjects.items.cardSteal);
            DontDestroyObjects.instance.playerItems[playerIndex].Add(DontDestroyObjects.items.cardSteal);
            GameObject createdItem = Instantiate(itemUIPrefab);
            createdItem.transform.SetParent(playerInformationUIs[playerIndex].transform.GetChild(1), false);
            createdItem.GetComponent<Image>().sprite = itemSmallSprites[3];
        }
        else if (itemName == "�ð����ѱ� UI")
        {
            DontDestroyObjects.instance.playerItems[controlPlayerIndexWithOrder].Remove(DontDestroyObjects.items.timeSteal);
            DontDestroyObjects.instance.playerItems[playerIndex].Add(DontDestroyObjects.items.timeSteal);
            GameObject createdItem = Instantiate(itemUIPrefab);
            createdItem.transform.SetParent(playerInformationUIs[playerIndex].transform.GetChild(1), false);
            createdItem.GetComponent<Image>().sprite = itemSmallSprites[4];
        }
        else if (itemName == "��Ʈ UI")
        {
            DontDestroyObjects.instance.playerItems[controlPlayerIndexWithOrder].Remove(DontDestroyObjects.items.hint);
            DontDestroyObjects.instance.playerItems[playerIndex].Add(DontDestroyObjects.items.hint);
            GameObject createdItem = Instantiate(itemUIPrefab);
            createdItem.transform.SetParent(playerInformationUIs[playerIndex].transform.GetChild(1), false);
            createdItem.GetComponent<Image>().sprite = itemSmallSprites[0];
        }
        else if (itemName == "������ ����� UI")
        {
            DontDestroyObjects.instance.playerItems[controlPlayerIndexWithOrder].Remove(DontDestroyObjects.items.erase);
            DontDestroyObjects.instance.playerItems[playerIndex].Add(DontDestroyObjects.items.bind);
            GameObject createdItem = Instantiate(itemUIPrefab);
            createdItem.transform.SetParent(playerInformationUIs[playerIndex].transform.GetChild(1), false);
            createdItem.GetComponent<Image>().sprite = itemSmallSprites[1];
        }
        else
        {
            DontDestroyObjects.instance.playerItems[controlPlayerIndexWithOrder].Remove(DontDestroyObjects.items.pass);
            DontDestroyObjects.instance.playerItems[playerIndex].Add(DontDestroyObjects.items.pass);
            GameObject createdItem = Instantiate(itemUIPrefab);
            createdItem.transform.SetParent(playerInformationUIs[playerIndex].transform.GetChild(1), false);
            createdItem.GetComponent<Image>().sprite = itemSmallSprites[2];
        }

    }

    public void eraseItemUI(int index, string itemName)
    {
        GameObject itemPanel = playerInformationUIs[index].transform.GetChild(1).gameObject;
        string itemNameToSpirteName = "";
        if (itemName == "������ü")
        {
            itemNameToSpirteName = "������ü UI";
        }
        else if (itemName == "ī�廩�ѱ�")
        {
            itemNameToSpirteName = "ī�� ���ѱ�";
        }
        else if (itemName == "�ð����ѱ�")
        {
            itemNameToSpirteName = "�ð� ���ѱ� UI";
        }
        else if (itemName == "��Ʈ" || itemName=="hint" )
        {
            itemNameToSpirteName = "��Ʈ UI";
        }
        else if (itemName == "����������" || itemName == "erase" )
        {
            itemNameToSpirteName = "������ ����� UI";
        }
        else
        {
            itemNameToSpirteName = "���� ��ŵ UI";
        }
        foreach (Transform child in itemPanel.transform)
        {
            if (child.gameObject.GetComponent<Image>().sprite.name == itemNameToSpirteName)
            {
                Destroy(child.gameObject);
                break;
            }
        }
    }
}

