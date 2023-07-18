using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class problem : MonoBehaviour
{
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
    List<Dictionary<string, object>> problemData;
    List<Dictionary<string, object>> answerData;

    int problemID;
    int prevDynasty;
    string dynasty;
    string problemType;
    string isHaveHint;
    string hintString;
    TMP_Text resultText;

    problemGraph problemScript;

    int playerPosition;
    bool isPlayerCorrect;
    // Start is called before the first frame update
    void Awake()
    {
        problemData = CSVReader.Read("����");
        answerData = CSVReader.Read("��");
        resultText = resultPanel.transform.GetChild(0).gameObject.GetComponent<TMP_Text>();
        problemScript = this.gameObject.GetComponent<problemGraph>();
    }

    // Update is called once per frame
    void Update()
    {

    }


    void OnEnable()
    {
        getPlayerNextPosition();
        Debug.Log(playerPosition);
        setProblemID();
        Debug.Log("���� ��ȣ: " + problemID);
        getInfoFromCSV();
        setImage();
        controlButtons();
        if (problemType == "ox")
        {
            StartCoroutine("setTimer", 15);
        }
        else
        {
            StartCoroutine("setTimer", 30);
        }
    }

    void OnDisable()
    {
        hintPanel.SetActive(false);
        if (isPlayerCorrect == true)
        {
            GameManager.instance.MovePlayer();
        }
        else
            GameManager.instance.finishRound = true; //�߰��� �κ�
    }

    void setProblemID()
    {
        if (playerPosition >= 1 && playerPosition <= 8)
        {
            problemID = Random.Range(1, 30);
            prevDynasty = 0;
        }
        else if (playerPosition >= 9 && playerPosition <= 20)
        {
            problemID = Random.Range(1, 65) + 30;
            prevDynasty = 30;
        }
        else if (playerPosition >= 21 && playerPosition <= 40)
        {
            //problemID = Random.Range(1, ����ô� ���� ��) + 95;
            prevDynasty = 95;
        }
        else if (playerPosition >= 41 && playerPosition <= 70)
        {
            //problemID = Random.Range(1, �����ô� ���� ��) + 95+����ô빮�� ��;
            //prevDynasty = 95+����ô� ���� �� ;
        }
        else
        {
            //problemID = Random.Range(1, ������ ���� ��) + 95+����ô빮�� ��+�����ô� ���� ��;
            //prevDynasty = 95+����ô� ���� ��+�����ô� ���� ��;
        }
    }

    IEnumerator setTimer(int time)
    {
        while (time >= 0)
        {
            TimeText.text = "���� �ð�: " + time.ToString() + "��";
            time -= 1;
            yield return new WaitForSeconds(1.0f);
        }
        resultText.text = "Ʋ�Ƚ��ϴ�...";
        isPlayerCorrect = false;
        resultPanel.SetActive(true);
    }

    void getInfoFromCSV()
    {
        dynasty = problemData[problemID - 1 + prevDynasty]["�ô�"].ToString();
        problemType = problemData[problemID - 1 + prevDynasty]["����"].ToString();
        isHaveHint = problemData[problemID - 1 + prevDynasty]["��Ʈ ����"].ToString();
        hintString = problemData[problemID - 1 + prevDynasty]["��Ʈ"].ToString();
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
        Sprite sprite = dynastyImageGraph[problemID - 1];
        problemImage.GetComponent<Image>().sprite = sprite;
        problemImage.GetComponent<Image>().SetNativeSize();
    }

    void controlButtons()
    {
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
    }

    void setSelectionText(Button button, string selectionName)
    {
        TMP_Text selectionText = button.transform.GetChild(0).GetComponent<TMP_Text>();
        selectionText.text = answerData[problemID - 1][selectionName].ToString();
    }

    public void selectAnswer(int selectionNum)
    {
        StopCoroutine("setTimer");
        resultPanel.SetActive(true);
        int correctAnswer = int.Parse(answerData[problemID - 1]["��"].ToString());
        if (selectionNum == correctAnswer)
        {
            resultText.text = "�����Դϴ�!";
            isPlayerCorrect = true;
        }
        else
        {
            resultText.text = "Ʋ�Ƚ��ϴ�...";
            isPlayerCorrect = false;
        }
    }

    public void showPassButton()
    {
        List<GameManager.items> playerCards = GameManager.instance.player.itemCards;
        if (playerCards.Contains(GameManager.items.pass))
        {
            problemPassButton.SetActive(true);
        }
        else
        {
            problemPassButton.gameObject.SetActive(false);
        }
    }

    public void showHint()
    {
        List<GameManager.items> playerCards = GameManager.instance.player.itemCards;
        if (playerCards.Contains(GameManager.items.hint))
        {
            TMP_Text hintText = hintPanel.transform.GetChild(0).GetComponent<TMP_Text>();
            hintText.text = hintString;
            hintPanel.SetActive(true);
            GameManager.instance.useItemCard(GameManager.items.hint);
        }
    }

    public void getPlayerNextPosition()
    {
        playerPosition = GameManager.instance.getPlayerNextPosition();
    }
}