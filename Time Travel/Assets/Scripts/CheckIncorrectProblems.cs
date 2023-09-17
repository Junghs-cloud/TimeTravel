using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CheckIncorrectProblems : MonoBehaviour
{
    public List<GameObject> pages;
    public int currentPage = 0;
    public int totalPage;
    public Button leftBtn;
    public Button rightBtn;

    public GameObject noIncorrectText;

    public problem problemScript;
    public problemGraph problemGraphScript;

    public GameObject problemPrefab;
    public GameObject selection1;
    public GameObject selection2;
    public GameObject selection3;
    public GameObject selection4;

    public Sprite problemImg;
    public Canvas incorrectProblemPanel;

    string dynasty;
    string problemType;
    int problemID;
    int prevDynasty;


    void Awake()
    {
        problemScript = problem.instance;
        problemGraphScript = problem.instance.problemScript;

    }


    void Start()
    {
        
        leftBtn.onClick.AddListener(() => { PrevPage(); });
        rightBtn.onClick.AddListener(() => { NextPage(); });
    }

    public void PrevPage()
    {
        SoundManager.instance.SoundPlayer("FlipPage");

        if (currentPage == 0)
        {
            return;
        }

        pages[currentPage].SetActive(false);
        currentPage--;
        pages[currentPage].SetActive(true);
        if (currentPage == 0)
        {
            leftBtn.interactable = false;
            rightBtn.interactable = true;
        }
        else
        {
            rightBtn.interactable = true;
            leftBtn.interactable = true;
        }

    }

    public void NextPage()
    {
        SoundManager.instance.SoundPlayer("FlipPage");
        if (currentPage == totalPage - 1)
        {
            return;
        }
        pages[currentPage].SetActive(false);
        currentPage++;
        pages[currentPage].SetActive(true);
        if (currentPage == totalPage - 1)
        {
            rightBtn.interactable = false;
            leftBtn.interactable = true;
        }
        else
        {
            rightBtn.interactable = true;
            leftBtn.interactable = true;
        }

    }

    public void CloseIncorrectProblemPanel()
    {
        SoundManager.instance.SoundPlayer("Button");
        incorrectProblemPanel.gameObject.SetActive(false);
    }

    public void ShowIncorrectProblems(int playerIndex)
    {
        if (GameManager.instance.player[playerIndex].incorrectProblemNumbers.Count == 0)
        {
            noIncorrectText.SetActive(true);
            return;
        }
            

        incorrectProblemPanel.gameObject.SetActive(true);
        totalPage = GameManager.instance.player[playerIndex].incorrectProblemNumbers.Count;
        if (totalPage <= 1)
        {
            leftBtn.interactable = false;
            rightBtn.interactable = false;
        }
        for (int i = 0; i < GameManager.instance.player[playerIndex].incorrectProblemNumbers.Count; i++)
        {
            problemID = GameManager.instance.player[playerIndex].incorrectProblemNumbers[i];
            problemType = problemScript.problemData[problemID - 1]["����"].ToString();
            dynasty = problemScript.problemData[problemID - 1]["�ô�"].ToString();

            SetPrevDynasty();
            SetProblemImg(dynasty);

            GameObject problemPage = Instantiate(problemPrefab, incorrectProblemPanel.transform.GetChild(0).gameObject.transform);
            problemPage.name = "ProblemNumber" + problemID; 
            problemPage.transform.GetChild(0).GetComponent<TMP_Text>().text = dynasty;
            problemPage.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().sprite = problemImg;
            problemPage.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().SetNativeSize();
            if(i != 0)
                problemPage.SetActive(false);
            selection1 = problemPage.transform.GetChild(2).GetChild(0).gameObject;
            selection2 = problemPage.transform.GetChild(2).GetChild(1).gameObject;
            selection1.SetActive(true);
            selection2.SetActive(true);
            
            if (problemType == "ox")
            {
                TMP_Text selectionText = selection1.transform.GetChild(0).GetComponent<TMP_Text>();
                selectionText.text = "o";
                CompareWithCorrectAnswer(1, selection1);
                selectionText = selection2.transform.GetChild(0).GetComponent<TMP_Text>();
                selectionText.text = "x";
                CompareWithCorrectAnswer(2, selection2);
            }
            else
            {
                selection3 = problemPage.transform.GetChild(2).GetChild(2).gameObject;
                selection4 = problemPage.transform.GetChild(2).GetChild(3).gameObject;
                selection3.SetActive(true);
                selection4.SetActive(true);
                setSelectionText(problemPage.transform.GetChild(2).GetChild(0).gameObject, "������1");
                CompareWithCorrectAnswer(1, selection1);
                setSelectionText(problemPage.transform.GetChild(2).GetChild(1).gameObject, "������2");
                CompareWithCorrectAnswer(2, selection2);
                setSelectionText(problemPage.transform.GetChild(2).GetChild(2).gameObject, "������3");
                CompareWithCorrectAnswer(3, selection3);
                setSelectionText(problemPage.transform.GetChild(2).GetChild(3).gameObject, "������4");
                CompareWithCorrectAnswer(4, selection4);
            }
            pages.Add(problemPage);
            
        }
    }

    void CompareWithCorrectAnswer(int selectionNum, GameObject selection)
    {
        int correctAnswer = int.Parse(problemScript.answerData[problemID - 1]["��"].ToString());
        if (correctAnswer == selectionNum)
        {
            selection.transform.GetChild(0).GetComponent<TMP_Text>().color = Color.red;
            selection.transform.GetChild(1).gameObject.SetActive(true);
        }
        else
        {
            selection.transform.GetChild(0).GetComponent<TMP_Text>().color = Color.black;
            selection.transform.GetChild(1).gameObject.SetActive(false);
        }
            
    }

    void SetPrevDynasty()
    {
        if (problemID < 31)
            prevDynasty = 0;
        else if (problemID < 96)
            prevDynasty = 30;
        else if (problemID < 176)
            prevDynasty = 95;
        else if (problemID < 286)
            prevDynasty = 175;
        else
            prevDynasty = 285;
    }

    void SetProblemImg(string dynasty)
    {
        Sprite[] dynastyImageGraph = null;
        switch (dynasty)
        {
            case "������":
                dynastyImageGraph = problemGraphScript.dynasty1;
                break;
            case "�ﱹ�ô�":
                dynastyImageGraph = problemGraphScript.dynasty2;
                break;
            case "���":
                dynastyImageGraph = problemGraphScript.dynasty3;
                break;
            case "�����ô�":
                dynastyImageGraph = problemGraphScript.dynasty4;
                break;
            case "�ٴ�����":
                dynastyImageGraph = problemGraphScript.dynasty5;
                break;
            default: break;
        }
        problemImg = dynastyImageGraph[problemID - 1 - prevDynasty];
    }

    void setSelectionText(GameObject selection, string selectionName)
    {
        TMP_Text selectionText = selection.transform.GetChild(0).GetComponent<TMP_Text>();
        selectionText.text = problemScript.answerData[problemID - 1][selectionName].ToString();
    }
}
