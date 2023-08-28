using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class networkManager : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    public int maxPlayer;
    int playerNums;

    public TMP_Text connectionInfoText;    //���� ���� ǥ�� �ؽ�Ʈ
    public TMP_InputField nickNameInput; //�г��� �Է�
    public Button[] joinButton;   //�� ���� ��ư

    public Image roomMakePanel;
    public TMP_Dropdown selectPlayerNum;
    public TMP_InputField setRoomPasswordInput;
    public TMP_InputField enterRoomPasswordInput;
    string roomPassword;
    string enterRoomPassword;

    public GameObject changeNickName;
    public bool sameNickName;

    public Image roomEnterPanel;
    public Main mainScript;

    void Start()
    {
        Screen.SetResolution(1920, 1080, false);
        PhotonNetwork.ConnectUsingSettings(); //������ ���� ���� �õ�
        for(int i = 0; i < joinButton.Length;i++)
            joinButton[i].interactable = false; //���� �õ��� ��ư Ŭ�� ���ϰ�
        connectionInfoText.text = "������ ������ ���� ��...";
        maxPlayer = 2;
        selectPlayerNum.onValueChanged.AddListener(delegate { setMaxPlayer(selectPlayerNum.value + 2); });
    }

    void Update()
    {
        if(nickNameInput.text != "")
        {
            if (mainScript.isBanned == 1)
            {
                for (int i = 0; i < joinButton.Length; i++)
                    joinButton[i].interactable = false;
            }
            else
            {
                for (int i = 0; i < joinButton.Length; i++)
                    joinButton[i].interactable = true; //�� ���� ��ư Ȱ��ȭ
            }
        }

        else if (nickNameInput.text == "")
        {
            for (int i = 0; i < joinButton.Length; i++)
                joinButton[i].interactable = false; 
        }
    }

    public void setMaxPlayer(int num)
    {
        maxPlayer = num;
    }

    public void setNickName()
    {
        PhotonNetwork.LocalPlayer.NickName = nickNameInput.text; //�г��� �Է�
        Debug.Log("�г���" + nickNameInput.text);
    }

    public override void OnConnectedToMaster()
    {
        connectionInfoText.text = "�¶��� : ������ ������ �����";
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        for (int i = 0; i < joinButton.Length; i++)
            joinButton[i].interactable = false;
        connectionInfoText.text = "�������� : ������ ������ ������� ����\n ���� ��õ� ��...";
        PhotonNetwork.ConnectUsingSettings();
    }

    //Join Random Room
    public void Connect()
    {
        for (int i = 0; i < joinButton.Length; i++)
            joinButton[i].interactable = false;
        if (PhotonNetwork.IsConnected)
        {
            connectionInfoText.text = "�뿡 ����...";
            PhotonNetwork.JoinRandomRoom(null, (byte)maxPlayer);

        }
        else
        {
            connectionInfoText.text = "�������� : ������ ������ ������� ����\n���� ��õ� ��...";
            PhotonNetwork.ConnectUsingSettings(); //���� ��õ�
        }
    }


    //���� �� ���� ���н� ���ο� ���� �� ����
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        connectionInfoText.text = "�� ���� ����, ���ο� �� ����...";
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayer }); //2,3,4�� ���밡���� �� �����

    }


    //Make Room
    public void ClickCreateRoom()
    {
        roomMakePanel.gameObject.SetActive(true);
        
    }

    public void SetPassword()
    {
        roomPassword = setRoomPasswordInput.GetComponent<TMP_InputField>().text;
    }

    public void EnterPassword()
    {
        enterRoomPassword = enterRoomPasswordInput.GetComponent<TMP_InputField>().text;
    }


    //�� ����� ��ư
    public void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = maxPlayer };
        PhotonNetwork.CreateRoom(roomPassword, roomOptions);
    }

    //�� ����� �����ϸ�
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        connectionInfoText.text = "�� ����� ����, ��õ���...";
        CreateRoom();
    }

    //Enter Room
    public void ClickEnterRoom()
    {
        roomEnterPanel.gameObject.SetActive(true);
    }


    //�� �����ϱ� ��ư
    public void EnterRoom()
    {
        PhotonNetwork.JoinRoom(enterRoomPassword);
    }

    public override void OnJoinedRoom()
    {
        foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
        {
            if (player.NickName == PhotonNetwork.LocalPlayer.NickName && player != PhotonNetwork.LocalPlayer)
            {
                sameNickName = true;
                changeNickName.SetActive(true);
                PhotonNetwork.LeaveRoom();
                break;
            }
        }

        if (!sameNickName)
        {
            connectionInfoText.text = "�� ���� ����.";
            SceneManager.LoadScene("Room");
        }
        

    }

    public void ClickCancelBtn()
    {
        changeNickName.SetActive(false);
        sameNickName = false;
    }

}
