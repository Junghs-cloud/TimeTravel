using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Data;
using MySql.Data;
using MySql.Data.MySqlClient;

public class problemAddModifyMaker : MonoBehaviour
{
    public static MySqlConnection SqlConn;

    static string ipAddress = "183.96.251.147";
    string db_id = "root";
    string db_pw = "sm1906";
    string db_name = "timetravel";

    public TMP_Dropdown dynastySelection;
    public TMP_InputField addedProblem;
    public TMP_InputField selection1;
    public TMP_InputField selection2;
    public TMP_InputField selection3;
    public TMP_InputField selection4;
    public TMP_Dropdown problemCategory;
    public TMP_Dropdown answer;
    public TMP_Dropdown haveHint;
    public TMP_InputField hint;

    public TMP_Dropdown problemNum;

    public Button addProblemButton;
    public Button modifyProblemButton;
    public Button addButton;

    string dynastyText;
    string problemNumText;
    string problemText;
    string selection1Text;
    string selection2Text;
    string selection3Text;
    string selection4Text;
    string haveHintText;
    string hintText;
    int currentDynastyProblemNumCount = 0;

    void Awake()
    {
        string strConn = string.Format("server={0};uid={1};pwd={2};database={3};charset=utf8 ;", ipAddress, db_id, db_pw, db_name);
        SqlConn = new MySqlConnection(strConn);
    }

    void Start()
    {
        dynastySelection.onValueChanged.AddListener(delegate { setProblemNumOption();  });
        problemCategory.onValueChanged.AddListener(delegate { makeSelectionChange(); });
        addButton.onClick.AddListener(addProblemToDatabase);

        setProblemNumOption();
        
        //���� �����ؾ� �� �κ�

        //���� ��ȣ dropDown ���� �ٲ���� ��, DB�� problem ���̺� problem Text�� ���� NULL�̸� ���� inputField setActive()��Ű�� �ش� ������ �̹��� ������
        //null�� �ƴϸ� inputField�� �ش� �� �о����.
        //������ inputField�� dropDown���� �� �о����.

        //�߰��ϱ� ��ư ������ �ش� ���� DB�� ����.
        //�����ϱ� ��ư ������ �ش� ���� DB�� ����.

    }

    void setProblemNumOption()
    {
        for (int i = 0; i < problemNum.options.Count; i++)
        {
            problemNum.options.RemoveAt(0);
        }
        TMP_Text selectedLabel = problemNum.gameObject.transform.GetChild(0).gameObject.GetComponent<TMP_Text>();
        string query = setQuery();
        DataTable dynasty = selectRequest(query);
        currentDynastyProblemNumCount = dynasty.Rows.Count;

        if (addProblemButton.interactable== false)
        {
            problemNum.interactable = false;
            string newProblemNum = (currentDynastyProblemNumCount + 1).ToString();
            addOptionAtProblemNum(newProblemNum);

            selectedLabel.text = newProblemNum;
        }
        else
        {
            problemNum.interactable = true;
            for (int i = 1; i <= currentDynastyProblemNumCount; i++)
            {
                addOptionAtProblemNum(i.ToString());
            }
        }
        problemNum.value = 0;
    }

    void addOptionAtProblemNum(string valueText)
    {
        TMP_Dropdown.OptionData newData = new TMP_Dropdown.OptionData();
        newData.text = valueText;
        problemNum.options.Add(newData);
    }

    string setQuery()
    {
        string query;
        if (dynastySelection.value == 0)
        {
            query = "select ID from problem where �ô� = '������'";
        }
        else if (dynastySelection.value == 1)
        {
            query = "select ID from problem where �ô� = '�ﱹ�ô�'";
        }
        else if (dynastySelection.value == 2)
        {
            query = "select ID from problem where �ô� = '���'";
        }
        else if (dynastySelection.value == 3)
        {
            query = "select ID from problem where �ô� = '�����ô�'";
        }
        else
        {
            query = "select ID from problem where �ô� = '�ٴ�����'";
        }
        return query;
    }

    void addProblemToDatabase()
    {
        if (inputFieldAllFilled() == true)
        {
            getTextFromDropDownsAndInputFields();
            string insertQuery = string.Format("insert into problem values({0}, {1}, {2}, {3}, {4}) ;", dynastyText, problemNumText, problemText, haveHintText, hintText);
            setProblemNumOption();
            Debug.Log("���� �߰� ����");
        }
        else
        {
            Debug.Log("ä������ ���� inputField�� �־� ������ �߰��� �� �����ϴ�.\n");
        }
    }

    bool inputFieldAllFilled()
    {
        if (problemCategory.value == 0)
        {
            if (addedProblem.text != "" && selection1.text != "" && selection2.text != "")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (addedProblem.text != "" && selection1.text != "" && selection2.text != "" && selection3.text != "" && selection4.text != "")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    void getTextFromDropDownsAndInputFields()
    {
        dynastyText = dynastySelection.options[dynastySelection.value].text;
        problemNumText = problemNum.options[problemNum.value].text;
        problemText = addedProblem.text;
        selection1Text = selection1.text;
        selection2Text = selection2.text;
        selection3Text = selection3.text;
        selection4Text = selection4.text;
        haveHintText = haveHint.options[haveHint.value].text;
        hintText = hint.text;
    }

    void makeSelectionChange()
    {
        if (problemCategory.value == 0)
        {
            selection1.text = "o";
            selection2.text = "x";
            selection1.interactable = false;
            selection2.interactable = false;
            selection3.interactable = false;
            selection4.interactable = false;
        }
        else
        {
            selection1.text = "";
            selection2.text = "";
            selection1.interactable = true;
            selection2.interactable = true;
            selection3.interactable = true;
            selection4.interactable = true;
        }
    }

    public static DataTable selectRequest(string query)
    {
        try
        {
            SqlConn.Open();

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = SqlConn;
            cmd.CommandText = query;


            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);

            SqlConn.Close();

            return dataTable;
        }
        catch (System.Exception e)
        {
            Debug.Log(e.ToString());
            return null;
        }
    }

    public static void insertOrUpdateRequest(string query)
    {
        try
        {
            MySqlCommand sqlCommand = new MySqlCommand(query);
            sqlCommand.Connection = SqlConn;
            sqlCommand.CommandText = query;
            SqlConn.Open();
            sqlCommand.ExecuteNonQuery();
            SqlConn.Close();
        }
        catch (System.Exception e)
        {
            Debug.Log(e.ToString());
        }
    }
}
