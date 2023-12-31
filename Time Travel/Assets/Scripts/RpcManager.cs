using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RpcManager : MonoBehaviour
{
    public static RpcManager instance;
    public TMP_Text testTMP;
    public TMP_Text resultText;
    public PhotonView PV;

    public Dictionary<Photon.Realtime.Player, string> currentTurnItems;
    public string currentTurnUsedItemOfLocalPlayer;

    public GameObject diceImg;
    public Text diceTimer;
    public Dice dice;

    public problem problemScript;
    public Canvas problemCanvas;

    public bool isUpdatedPlayerUI;

    public bool isMovableWithBind;
    public List<int> bindPlayerIndexes;

    public bool isSomeoneUseCardSteal;
    public int bindPlayerDiceNum;
    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        isUpdatedPlayerUI = false;
        isMovableWithBind = false;
        isSomeoneUseCardSteal = false;
        bindPlayerIndexes = new List<int>();
    }

    void Start()
    {
        currentTurnItems = new Dictionary<Photon.Realtime.Player, string>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isMovableWithBind == true)
        {
            if (bindPlayerIndexes.Count != 0)
            {
                int bindPlayerIndex = bindPlayerIndexes[0];
                GameManager.instance.player[bindPlayerIndex].movingAllowed = true;
                GameManager.instance.nowMovingPlayerIndex = bindPlayerIndex;
                if (GameManager.instance.player[bindPlayerIndex].curIndex > GameManager.instance.playerStartPoint[bindPlayerIndex] + bindPlayerDiceNum)
                {
                    
                    GameManager.instance.player[bindPlayerIndex].movingAllowed = false;
                    GameManager.instance.CheckPlayersPosition(bindPlayerIndex);
                    GameManager.instance.playerStartPoint[bindPlayerIndex] = GameManager.instance.player[bindPlayerIndex].curIndex - 1;
                    GameManager.instance.updatePlayerInformationUI(bindPlayerIndex);
                    if (bindPlayerIndexes.Count == 1)
                    {
                        GameManager.instance.isMovableWithBind = false;
                        isMovableWithBind = false;
                        bindPlayerIndexes.RemoveAt(0);
                        GameManager.instance.finishRound = true;
                        GameManager.instance.UISmaller();
                        bindPlayerIndexes.Clear();
                    }
                    else
                    {
                        bindPlayerIndexes.RemoveAt(0);
                    }
                }
            }
        }
    }


    public void checkPositionAndMoveBindPlayer(int num)
    {
        isMovableWithBind = true;
        if (num == 7 || num == 22 || num == 53 || num == 64 || num == 76)
        {
            GameManager.instance.isLadder = true;
        }
        else if (num == 15 || num == 30 || num == 26 || num == 38 || num == 32 || num == 80 || num == 46 || num == 67 || num == 62 || num == 90)
        {
            GameManager.instance.isTransport = true;
        }
    }

    public void updatePlayerBoardNum(playerInformationUI obj, int playerPosition)
    {
        if (playerPosition == -1)
        {
            obj.playerPositionText.text = "0";
        }
        else
        {
            obj.playerPositionText.text = (playerPosition).ToString();
        }
    }

    public void useItemOfLocalPlayer()
    {
        PV.RPC("eraseItemToOthers", RpcTarget.AllViaServer, GameManager.instance.localPlayerIndexWithOrder, currentTurnUsedItemOfLocalPlayer);
    }

    public void eraseItem(int index, DontDestroyObjects.items itemName)
    {
        PV.RPC("eraseItemToOthers", RpcTarget.AllViaServer, index, itemName.ToString());
    }

    public void useAsetItemCard(DontDestroyObjects.items itemName)
    {
        PV.RPC("useAsetItemCardToOthers", RpcTarget.All, itemName.ToString());
    }

    public void useBsetItemCard(DontDestroyObjects.items itemName)
    {
        PV.RPC("useBsetItemCardToOthers", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer, itemName.ToString());
    }

    public void cardSteal(int playerIndex, int stolenCardIndex)
    {
        PV.RPC("cardStealToOthers", RpcTarget.All, playerIndex, stolenCardIndex);
    }

    public void makeIsUsedBindTrue(int localPlayerIndex)
    {
        PV.RPC("makeIsUsedBindTrueToOthers", RpcTarget.AllViaServer, localPlayerIndex);
    }

    public void setIsThisTurnTimeStealTrue()
    {
        PV.RPC("setIsThisTurnTimeStealTrueToOthers", RpcTarget.All);
    }

    public void setResultText()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (currentTurnItems.Count == 0)
            {
                PV.RPC("setResultTextToOthersWithNoOneUsed", RpcTarget.All);
            }
            else
            {

                List<string> itemUsePlayers = new List<string>();
                List<string> usedItems = new List<string>();
                foreach (KeyValuePair<Photon.Realtime.Player, string> entry in currentTurnItems)
                {
                    itemUsePlayers.Add(entry.Key.ToString());
                    usedItems.Add(entry.Value);
                }

                PV.RPC("setResultTextToOthers", RpcTarget.All, itemUsePlayers.ToArray(), usedItems.ToArray());
            }
        }
    }

    [PunRPC]
    void setResultTextToOthers(string[] players, string[] items)
    {
        resultText.text = "";
        for (int i = 0; i < players.Length; i++)
        {
            resultText.text += players[i].Substring(4) + "님이 ";
            if (items[i] == "cardSteal")
            {
                resultText.text+= "카드 빼앗기를 사용했습니다.\n";
            }
            else if (items[i] == "timeSteal")
            {
                resultText.text += "시간 빼앗기를 사용했습니다.\n";
            }
            else
            {
                resultText.text += "운명공동체를 사용했습니다.\n";
            }
        }
    }

    [PunRPC]
    void setResultTextToOthersWithNoOneUsed()
    {
        resultText.text = "사용된 아이템이 없습니다.\n";
    }

    public void removeCurrentTurnItems()
    {
        currentTurnItems.Clear();
    }

    [PunRPC]
    void eraseItemToOthers(int index, string itemName)
    {
        if (itemName == "운명공동체" || itemName == "bind")
        {
            DontDestroyObjects.instance.playerItems[index].Remove(DontDestroyObjects.items.bind);
        }
        else if (itemName == "카드빼앗기" || itemName=="cardSteal")
        {
            DontDestroyObjects.instance.playerItems[index].Remove(DontDestroyObjects.items.cardSteal);
        }
        else
        {
            DontDestroyObjects.instance.playerItems[index].Remove(DontDestroyObjects.items.timeSteal);
        }
        GameManager.instance.eraseItemUI(index, itemName);
    }

    [PunRPC]
    void useAsetItemCardToOthers(string itemName)
    {
        int playerIndex = GameManager.instance.controlPlayerIndexWithOrder;
        if (itemName == "erase")
        {
            DontDestroyObjects.instance.playerItems[playerIndex].Remove(DontDestroyObjects.items.erase);
        }
        else if (itemName == "hint")
        {
            DontDestroyObjects.instance.playerItems[playerIndex].Remove(DontDestroyObjects.items.hint);
        }
        else
        {
            DontDestroyObjects.instance.playerItems[playerIndex].Remove(DontDestroyObjects.items.pass);
        }
        GameManager.instance.eraseItemUI(playerIndex, itemName);
    }

    [PunRPC]
    void useBsetItemCardToOthers(Photon.Realtime.Player p, string itemName)
    {
        currentTurnItems.Add(p, itemName);
    }

    [PunRPC]
    void makeIsUsedBindTrueToOthers(int localPlayerIndex)
    {
        bindPlayerIndexes.Add(localPlayerIndex);
        GameManager.instance.isMovableWithBind = true;
    }

    [PunRPC]
    void cardStealToOthers(int playerIndex, int stolenCardIndex)
    {
        GameManager.instance.eraseItemUI(playerIndex, stolenCardIndex);
    }

    [PunRPC]
    void setIsThisTurnTimeStealTrueToOthers()
    {
       GameManager.instance.isThisTurnTimeSteal = true;
    }

    public void setDiceTrue()
    {
        if (GameManager.instance.controlPlayer == PhotonNetwork.LocalPlayer)
        {
            PV.RPC("setDiceTrueToOthers", RpcTarget.AllViaServer);
        }
    }

    [PunRPC]
    public void setDiceTrueToOthers()
    {
        diceImg.SetActive(true);
        diceTimer.gameObject.SetActive(true);
    }

    public void rollDice()
    {
        int[] diceSpriteIndex = new int[11];
        for (int i = 0; i <= 10; i++)
        {
            diceSpriteIndex[i] = Random.Range(0, 6);
        }
        PV.RPC("rollDiceRPC", RpcTarget.AllViaServer, diceSpriteIndex);
    }

    [PunRPC]
    public void rollDiceRPC(int[] diceSpriteIndex)
    {
        dice.rollDice(diceSpriteIndex);
    }

    public void showProblem()
    {
        PV.RPC("showProblemToOthers", RpcTarget.AllViaServer);
    }

    public void setProblemID(int playerPosition)
    {
        int problemID = 0;
        int dynastyNum = 0;
        string dynastyText;
        solvedProblem currentProblem;
        do
        {
            if (playerPosition >= 1 && playerPosition <= 8)
            {
                dynastyText = "고조선";
                dynastyNum = problemData.instance.dynasty1.Rows.Count;
                problemID = Random.Range(1, dynastyNum+1);
            }
            else if (playerPosition >= 9 && playerPosition <= 20)
            {
                dynastyText = "삼국시대";
                dynastyNum = problemData.instance.dynasty2.Rows.Count;
                problemID = Random.Range(1, dynastyNum + 1);
            }
            else if (playerPosition >= 21 && playerPosition <= 40)
            {
                dynastyText = "고려";
                dynastyNum = problemData.instance.dynasty3.Rows.Count;
                problemID = Random.Range(1, dynastyNum+1);
            }
            else if (playerPosition >= 41 && playerPosition <= 70)
            {
                dynastyText = "조선시대";
                dynastyNum = problemData.instance.dynasty4.Rows.Count;
                problemID = Random.Range(1, dynastyNum+1);
            }
            else
            {
                dynastyText = "근대이후";
                dynastyNum = problemData.instance.dynasty5.Rows.Count;
                problemID = Random.Range(1, dynastyNum+1);
            }
            currentProblem = new solvedProblem(dynastyText, problemID);
        } while (problemScript.solvedProblems.Contains(currentProblem) == true);
        PV.RPC("setProblemIDToOThers", RpcTarget.AllViaServer, problemID);
    }

    [PunRPC]
    void showProblemToOthers()
    {
        problemCanvas.gameObject.SetActive(true);
    }

    [PunRPC]
    public void setProblemIDToOThers( int problemID)
    {
        problemScript.problemID = problemID;
        problemScript.setProblemPanel(problemID);
    }

    //endgame
    public void ShowEndPanel()
    {
        PV.RPC("ShowEndPanelToOthers", RpcTarget.All, GameManager.instance.winner, GameManager.instance.isOver);
    }

    [PunRPC]
    public void ShowEndPanelToOthers(string winner, bool isOver)
    {
        GameManager.instance.endPanel.SetActive(true);
        GameManager.instance.isOver = isOver;
        GameManager.instance.winnerName.text = winner + " 님\n승리를 축하합니다!";
    }

    [PunRPC]
    void setBool()
    {
        if (isSomeoneUseCardSteal == true)
        {
            isSomeoneUseCardSteal = false;
        }
        else
        {
            isSomeoneUseCardSteal = true;
        }
    }

    public void GetAdditionalItem(int ran)
    {
        PV.RPC("ShowAddtionalItemToOthers", RpcTarget.All, ran, GameManager.instance.controlPlayerIndexWithOrder);
    }

    [PunRPC]
    void ShowAddtionalItemToOthers(int ran, int currentPlayerIndex)
    {
        //int itemCount = DontDestroyObjects.instance.playerItems[currentPlayerIndex].Count;
        GameObject itemUIPrefab = Resources.Load<GameObject>("Prefabs/itemImageUI");
        GameObject createdItem = Instantiate(itemUIPrefab);

        if (ran == 0)
        {
            DontDestroyObjects.instance.playerItems[currentPlayerIndex].Add(DontDestroyObjects.items.hint);
            createdItem.transform.SetParent(GameManager.instance.playerInformationUIs[currentPlayerIndex].transform.GetChild(1), false);
            createdItem.GetComponent<Image>().sprite = GameManager.instance.itemSmallSprites[0];

        }
        else if (ran == 1)
        {
            DontDestroyObjects.instance.playerItems[currentPlayerIndex].Add(DontDestroyObjects.items.erase);
            createdItem.transform.SetParent(GameManager.instance.playerInformationUIs[currentPlayerIndex].transform.GetChild(1), false);
            createdItem.GetComponent<Image>().sprite = GameManager.instance.itemSmallSprites[1];

        }
        else if (ran == 2)
        {
            DontDestroyObjects.instance.playerItems[currentPlayerIndex].Add(DontDestroyObjects.items.pass);
            createdItem.transform.SetParent(GameManager.instance.playerInformationUIs[currentPlayerIndex].transform.GetChild(1), false);
            createdItem.GetComponent<Image>().sprite = GameManager.instance.itemSmallSprites[2];
        }
        else if (ran == 3)
        {
            DontDestroyObjects.instance.playerItems[currentPlayerIndex].Add(DontDestroyObjects.items.cardSteal);
            createdItem.transform.SetParent(GameManager.instance.playerInformationUIs[currentPlayerIndex].transform.GetChild(1), false);
            createdItem.GetComponent<Image>().sprite = GameManager.instance.itemSmallSprites[3];
        }
        else if (ran == 4)
        {
            DontDestroyObjects.instance.playerItems[currentPlayerIndex].Add(DontDestroyObjects.items.timeSteal);
            createdItem.transform.SetParent(GameManager.instance.playerInformationUIs[currentPlayerIndex].transform.GetChild(1), false);
            createdItem.GetComponent<Image>().sprite = GameManager.instance.itemSmallSprites[4];
        }
        else
        {
            DontDestroyObjects.instance.playerItems[currentPlayerIndex].Add(DontDestroyObjects.items.bind);
            createdItem.transform.SetParent(GameManager.instance.playerInformationUIs[currentPlayerIndex].transform.GetChild(1), false);
            createdItem.GetComponent<Image>().sprite = GameManager.instance.itemSmallSprites[5];
        }

        for(int i = 0; i < DontDestroyObjects.instance.playerItems[currentPlayerIndex].Count; i++)
        {
            Debug.Log("아이템 보유 목록 :" + DontDestroyObjects.instance.playerItems[currentPlayerIndex][i].ToString());
        }
        
    }

    public void setCardStealBool()
    {
        PV.RPC("setBool", RpcTarget.AllViaServer);
    }
}
