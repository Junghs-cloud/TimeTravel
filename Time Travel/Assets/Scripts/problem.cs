using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

public class problem : MonoBehaviour
{
    public static problem instance;

    public Button selection1;
    public Button selection2;
    public Button selection3;
    public Button selection4;
    public Button hintButton;
    public TMP_Text TimeText;
    public TMP_Text dynastyText;
    public GameObject problemImage;
    public GameObject resultPanel;
    public GameObject hintPanel;
    public GameObject problemPassButton;
    public GameObject selectionEraseButton;
    public List<Dictionary<string, object>> problemData;
    public List<Dictionary<string, object>> answerData;
    public TMP_Text playerNameText;

    public int problemID;
    public int prevDynasty;
    string dynasty;
    string problemType;
    string isHaveHint;
    string hintString;
    TMP_Text resultText;

    public problemGraph problemScript;

    int playerPosition;
    bool isPlayerCorrect;

    bool isOtherPlayerUseTimeSteal;

    bool usePassItem;

    public PhotonView PV;
    public List<int> solvedProblems;
    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
            instance = this;

        resultText = resultPanel.transform.GetChild(0).gameObject.GetComponent<TMP_Text>();
        solvedProblems = new List<int>();
        problemID = 1;
        prevDynasty = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }


    void OnEnable()
    {
        usePassItem = false;
        getPlayerNextPosition();
        resultText = resultPanel.transform.GetChild(0).gameObject.GetComponent<TMP_Text>();
        playerNameText.text = "���� ���� Ǫ�� ���: " + GameManager.instance.controlPlayer.NickName;
        if (GameManager.instance.controlPlayer == PhotonNetwork.LocalPlayer)
        {
            RpcManager.instance.setProblemID(playerPosition);
            selection1.interactable = true;
            selection2.interactable = true;
            selection3.interactable = true;
            selection4.interactable = true;
        }
        else
        {
            selection1.interactable = false;
            selection2.interactable = false;
            selection3.interactable = false;
            selection4.interactable = false;
        }
    }

    void OnDisable()
    {
        hintPanel.SetActive(false);
        GameManager.instance.isThisTurnTimeSteal = false;
        if (isPlayerCorrect == true)
        {
            if (GameManager.instance.isUsedBind == true)
            {
                GameManager.instance.isMovableWithBind = true;
            }
            GameManager.instance.MovePlayer();

            if (!usePassItem) //�н������� correctcount����x
            {
                GameManager.instance.player[GameManager.instance.controlPlayerIndexWithOrder].correctCount++;
                GameManager.instance.UpdateGaugeImg();
            }
            RpcManager.instance.bindPlayerDiceNum = GameManager.instance.newDiceSide;

        }
        else
        {
            GameManager.instance.finishRound = true;
            GameManager.instance.UISmaller();
        }

    }

    public void setProblemPanel(int problemID, int prevDynasty)
    {
        this.problemID = problemID;
        this.prevDynasty = prevDynasty;
        getInfoFromCSV();
        setImage();
        controlButtons();
        if (PhotonNetwork.LocalPlayer == GameManager.instance.controlPlayer)
        {
            solvedProblems.Add(problemID);
        }
        if (problemType == "ox")
        {
            if (GameManager.instance.isThisTurnTimeSteal == true)
            {
                StartCoroutine("setTimer", 8);
            }
            else
            {
                StartCoroutine("setTimer", 15);
            }
        }
        else
        {
            if (GameManager.instance.isThisTurnTimeSteal == true)
            {
                StartCoroutine("setTimer", 25);
            }
            else
            {
                StartCoroutine("setTimer", 40);
            }
        }
    }

    IEnumerator setTimer(int time)
    {
        while (time >= 0)
        {
            TimeText.text = "���� �ð�: " + time.ToString() + "��";
            time -= 1;
            yield return new WaitForSeconds(1.0f);
            if (time == 5)
                SoundManager.instance.SoundPlayer("5Timer");
        }
        resultText.text = "Ʋ�Ƚ��ϴ�...";
        isPlayerCorrect = false;
        resultPanel.SetActive(true);
    }

    void getInfoFromCSV()
    {
        dynasty = problemData[problemID - 1]["�ô�"].ToString();
        problemType = problemData[problemID - 1]["����"].ToString();
        isHaveHint = problemData[problemID - 1]["��Ʈ ����"].ToString();
        hintString = problemData[problemID - 1]["��Ʈ"].ToString();
        dynastyText.text = dynasty;
    }

    void setImage()
    {
        string graphName = dynasty + problemID;
        Sprite[] dynastyImageGraph = null;
        switch (dynasty)
        {
            case "������":
                dynastyImageGraph = problemScript.dynasty1;
                break;
            case "�ﱹ�ô�":
                dynastyImageGraph = problemScript.dynasty2;
                break;
            case "���":
                dynastyImageGraph = problemScript.dynasty3;
                break;
            case "�����ô�":
                dynastyImageGraph = problemScript.dynasty4;
                break;
            case "�ٴ�����":
                dynastyImageGraph = problemScript.dynasty5;
                break;
            default: break;
        }
        Sprite sprite = dynastyImageGraph[problemID - 1 - prevDynasty];
        problemImage.GetComponent<Image>().sprite = sprite;
        problemImage.GetComponent<Image>().SetNativeSize();
    }

    void controlButtons()
    {
        selection1.gameObject.SetActive(true);
        selection2.gameObject.SetActive(true);
        if (problemType == "ox")
        {
            selection3.gameObject.SetActive(false);
            selection4.gameObject.SetActive(false);
            TMP_Text selectionText = selection1.transform.GetChild(0).GetComponent<TMP_Text>();
            selectionText.text = "o";
            selectionText = selection2.transform.GetChild(0).GetComponent<TMP_Text>();
            selectionText.text = "x";
        }
        else
        {
            selection3.gameObject.SetActive(true);
            selection4.gameObject.SetActive(true);
            setSelectionText(selection1, "������1");
            setSelectionText(selection2, "������2");
            setSelectionText(selection3, "������3");
            setSelectionText(selection4, "������4");
        }
        if (isHaveHint == "o")
        {
            hintButton.gameObject.SetActive(true);
        }
        else
        {
            hintButton.gameObject.SetActive(false);
        }
        showPassButton();
        showSelectionEraseButton();
    }

    void setSelectionText(Button button, string selectionName)
    {
        TMP_Text selectionText = button.transform.GetChild(0).GetComponent<TMP_Text>();
        selectionText.text = answerData[problemID - 1][selectionName].ToString();
    }

    public void selectAnswer(int selectionNum)
    {
        if (GameManager.instance.controlPlayer != PhotonNetwork.LocalPlayer)
        {
            return;
        }
        PV.RPC("selectAnswerToOthers", RpcTarget.AllViaServer, selectionNum);
    }

    [PunRPC]
    public void selectAnswerToOthers(int selectionNum)
    {
        StopCoroutine("setTimer");
        SoundManager.instance.SoundPlayerStop();
        resultPanel.SetActive(true);
        int correctAnswer = int.Parse(answerData[problemID - 1]["��"].ToString());
        if (selectionNum == correctAnswer)
        {
            resultText.text = "�����Դϴ�!";
            isPlayerCorrect = true;
            SoundManager.instance.SoundPlayer("Correct");
        }
        else
        {
            //���� ����
            if (!GameManager.instance.player[GameManager.instance.controlPlayerIndexWithOrder].incorrectProblemNumbers.Contains(problemID))
                GameManager.instance.player[GameManager.instance.controlPlayerIndexWithOrder].incorrectProblemNumbers.Add(problemID);
            resultText.text = "Ʋ�Ƚ��ϴ�...";
            isPlayerCorrect = false;
            SoundManager.instance.SoundPlayer("Wrong");
        }
    }

    public void showPassButton()
    {
        List<DontDestroyObjects.items> playerCards = DontDestroyObjects.instance.playerItems[GameManager.instance.controlPlayerIndexWithOrder];
        if (playerCards.Contains(DontDestroyObjects.items.pass))
        {
            problemPassButton.SetActive(true);
        }
        else
        {
            problemPassButton.gameObject.SetActive(false);
        }
    }

    public void showSelectionEraseButton()
    {
        List<DontDestroyObjects.items> playerCards = DontDestroyObjects.instance.playerItems[GameManager.instance.controlPlayerIndexWithOrder];
        if (playerCards.Contains(DontDestroyObjects.items.erase) && problemType == "4������")
        {
            selectionEraseButton.SetActive(true);
        }
        else
        {
            selectionEraseButton.gameObject.SetActive(false);
        }
    }

    public void showHint()
    {
        if (GameManager.instance.controlPlayer != PhotonNetwork.LocalPlayer)
        {
            return;
        }
        if (GameManager.instance.currentTurnASetItem == 1)
        {
            return;
        }
        SoundManager.instance.SoundPlayer("Button");
        List<DontDestroyObjects.items> playerCards = DontDestroyObjects.instance.playerItems[GameManager.instance.controlPlayerIndexWithOrder];
        if (playerCards.Contains(DontDestroyObjects.items.hint))
        {
            SoundManager.instance.SoundPlayer("Button1");
            TMP_Text hintText = hintPanel.transform.GetChild(0).GetComponent<TMP_Text>();
            hintText.text = hintString;
            hintPanel.SetActive(true);
            hintButton.gameObject.SetActive(false);
            RpcManager.instance.useAsetItemCard(DontDestroyObjects.items.hint);
            GameManager.instance.currentTurnASetItem = 1;
        }
    }

    public void eraseWrongSelection()
    {
        if (GameManager.instance.controlPlayer != PhotonNetwork.LocalPlayer)
        {
            return;
        }
        if (GameManager.instance.currentTurnASetItem == 1)
        {
            return;
        }
        SoundManager.instance.SoundPlayer("Button1");
        RpcManager.instance.useAsetItemCard(DontDestroyObjects.items.erase);
        selectionEraseButton.gameObject.SetActive(false);
        GameManager.instance.currentTurnASetItem = 1;
        int correctAnswer = int.Parse(answerData[problemID - 1]["��"].ToString());
        int eraseSelection;
        while (true)
        {
            eraseSelection = Random.Range(1, 5);
            if (eraseSelection != correctAnswer)
            {
                break;
            }
        }
        if (eraseSelection == 1)
        {
            selection1.gameObject.SetActive(false);
        }
        else if (eraseSelection == 2)
        {
            selection2.gameObject.SetActive(false);
        }
        else if (eraseSelection == 3)
        {
            selection3.gameObject.SetActive(false);
        }
        else
        {
            selection4.gameObject.SetActive(false);
        }
    }

    public void passProblem()
    {
        if (GameManager.instance.controlPlayer != PhotonNetwork.LocalPlayer)
        {
            return;
        }
        if (GameManager.instance.currentTurnASetItem == 1)
        {
            return;
        }


        problemPassButton.gameObject.SetActive(false);
        RpcManager.instance.useAsetItemCard(DontDestroyObjects.items.pass);
        GameManager.instance.currentTurnASetItem = 1;
        PV.RPC("passProblemToOthers", RpcTarget.AllViaServer);
    }

    [PunRPC]
    void passProblemToOthers()
    {
        StopCoroutine("setTimer");
        SoundManager.instance.SoundPlayerStop();
        SoundManager.instance.SoundPlayer("Button1");
        resultPanel.SetActive(true);
        resultText.text = "������ �н��߽��ϴ�. \n";
        string correctAnswer = answerData[problemID - 1]["��"].ToString();
        if (problemType == "ox")
        {
            if (correctAnswer == "1")
            {
                resultText.text += "������ o �����ϴ�.";
            }
            else
            {
                resultText.text += "������ x �����ϴ�.";
            }
        }
        else
        {
            resultText.text += "������ " + correctAnswer + "���̾����ϴ�.";
        }
        isPlayerCorrect = true;
        usePassItem = true;
    }

    public void getPlayerNextPosition()
    {
        playerPosition = GameManager.instance.getPlayerNextPosition();
    }

}