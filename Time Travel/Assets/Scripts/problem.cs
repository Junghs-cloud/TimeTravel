using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using System.Data;
using System;

public class solvedProblem
{
    string dynasty;
    int problemID;
    public solvedProblem(string dynastyText, int problemID)
    {
        dynasty=dynastyText;
        this.problemID=problemID;
    }
}

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
    public TMP_Text problemText;
    public GameObject problemImage;
    public GameObject resultPanel;
    public GameObject hintPanel;
    public GameObject problemPassButton;
    public GameObject selectionEraseButton;
    public List<Dictionary<string, object>> problemDataCSV;
    public List<Dictionary<string, object>> answerData;
    public TMP_Text playerNameText;

    public int problemID;
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
    public List<solvedProblem> solvedProblems;
    DataRow currentProblemRow;
    DataRow currentAnswerRow;
    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
            instance = this;

        resultText = resultPanel.transform.GetChild(0).gameObject.GetComponent<TMP_Text>();
        solvedProblems = new List<solvedProblem>();
        problemID = 1;
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

    public void setProblemPanel(int problemID)
    {
        this.problemID = problemID;
        getInfoFromCSV();
        controlButtons();
        if (PhotonNetwork.LocalPlayer == GameManager.instance.controlPlayer)
        {
            solvedProblem currentProblem = new solvedProblem(dynasty, problemID);
            solvedProblems.Add(currentProblem);
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
        getCurrentProblem();

        dynasty = currentProblemRow["�ô�"].ToString();
        problemType = currentProblemRow["����"].ToString();
        isHaveHint = currentProblemRow["��Ʈ ����"].ToString();
        hintString = currentProblemRow["��Ʈ"].ToString();
        dynastyText.text = dynasty;
        if (DBNull.Value.Equals(currentProblemRow["����"]) == true)
        {
            problemText.gameObject.SetActive(false);
            problemImage.SetActive(true);
            setImage();
        }
        else
        {
            problemText.gameObject.SetActive(true);
            problemImage.SetActive(false);
            problemText.text = currentProblemRow["����"].ToString();
        }
        /*
        dynasty = problemDataCSV[problemID - 1]["�ô�"].ToString();
        problemType = problemDataCSV[problemID - 1]["����"].ToString();
        isHaveHint = problemDataCSV[problemID - 1]["��Ʈ ����"].ToString();
        hintString = problemDataCSV[problemID - 1]["��Ʈ"].ToString();
        dynastyText.text = dynasty;
        */
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
        Debug.Log(dynasty + "  " + problemID);
        Sprite sprite = dynastyImageGraph[problemID - 1];
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
        selectionText.text =currentAnswerRow[selectionName].ToString();
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
        int correctAnswer = int.Parse(currentAnswerRow["��"].ToString());
        if (selectionNum == correctAnswer)
        {
            resultText.text = "�����Դϴ�!";
            isPlayerCorrect = true;
            SoundManager.instance.SoundPlayer("Correct");
        }
        else
        {
            //���� ����
            switch (dynasty)
            {
                case "������":
                    if (!GameManager.instance.player[GameManager.instance.controlPlayerIndexWithOrder].incorrectProblemsFromDynasty1.Contains(problemID))
                        GameManager.instance.player[GameManager.instance.controlPlayerIndexWithOrder].incorrectProblemsFromDynasty1.Add(problemID);
                    break;
                case "�ﱹ�ô�":
                    if (!GameManager.instance.player[GameManager.instance.controlPlayerIndexWithOrder].incorrectProblemsFromDynasty2.Contains(problemID))
                        GameManager.instance.player[GameManager.instance.controlPlayerIndexWithOrder].incorrectProblemsFromDynasty2.Add(problemID);
                    break;
                case "���":
                    if (!GameManager.instance.player[GameManager.instance.controlPlayerIndexWithOrder].incorrectProblemsFromDynasty3.Contains(problemID))
                        GameManager.instance.player[GameManager.instance.controlPlayerIndexWithOrder].incorrectProblemsFromDynasty3.Add(problemID);
                    break;
                case "�����ô�":
                    if (!GameManager.instance.player[GameManager.instance.controlPlayerIndexWithOrder].incorrectProblemsFromDynasty4.Contains(problemID))
                        GameManager.instance.player[GameManager.instance.controlPlayerIndexWithOrder].incorrectProblemsFromDynasty4.Add(problemID);
                    break;
                case "�ٴ�����":
                    if (!GameManager.instance.player[GameManager.instance.controlPlayerIndexWithOrder].incorrectProblemsFromDynasty5.Contains(problemID))
                        GameManager.instance.player[GameManager.instance.controlPlayerIndexWithOrder].incorrectProblemsFromDynasty5.Add(problemID);
                    break;

            }
            
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
        int correctAnswer = int.Parse(currentProblemRow["��"].ToString());
        int eraseSelection;
        while (true)
        {
            eraseSelection = UnityEngine.Random.Range(1, 5);
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
        string correctAnswer = currentAnswerRow["��"].ToString();
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

    void  getCurrentProblem()
    {
        if (playerPosition >= 1 && playerPosition <= 8)
        {
            currentProblemRow = problemData.instance.dynasty1.Rows[problemID - 1];
            currentAnswerRow = problemData.instance.answer1.Rows[problemID - 1];
        }
        else if (playerPosition >= 9 && playerPosition <= 20)
        {
            currentProblemRow = problemData.instance.dynasty2.Rows[problemID - 1];
            currentAnswerRow = problemData.instance.answer2.Rows[problemID - 1];
        }
        else if (playerPosition >= 21 && playerPosition <= 40)
        {
            currentProblemRow = problemData.instance.dynasty3.Rows[problemID - 1];
            currentAnswerRow = problemData.instance.answer3.Rows[problemID - 1];
        }
        else if (playerPosition >= 41 && playerPosition <= 70)
        {
            currentProblemRow = problemData.instance.dynasty4.Rows[problemID - 1];
            currentAnswerRow = problemData.instance.answer4.Rows[problemID - 1];
        }
        else
        {
            currentProblemRow = problemData.instance.dynasty5.Rows[problemID - 1];
            currentAnswerRow = problemData.instance.answer5.Rows[problemID - 1];
        }
    }
}