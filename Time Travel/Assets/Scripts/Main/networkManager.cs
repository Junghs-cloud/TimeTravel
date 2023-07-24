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
    int maxPlayer;
    int playerNums;

    public TMP_Text connectionInfoText;    //���� ���� ǥ�� �ؽ�Ʈ
    public TMP_InputField nickNameInput; //�г��� �Է�
    public Button[] joinButton;   //�� ���� ��ư

    public Image roomMakePanel;
    public TMP_Dropdown selectPlayerNum;
    public TMP_InputField roomPasswordInput;
    string roomPassword;

    public Image roomEnterPanel;

    void Start()
    {
        Screen.SetResolution(1920, 1080, false);
        PhotonNetwork.ConnectUsingSettings(); //������ ���� ���� �õ�
        for(int i = 0; i < joinButton.Length;i++)
            joinButton[i].interactable = false; //���� �õ��� ��ư Ŭ�� ���ϰ�
        connectionInfoText.text = "������ ������ ���� ��...";
        PhotonNetwork.NickName = nickNameInput.GetComponent<TMP_InputField>().text; //�г��� �Է�
        maxPlayer = 4;
    }

    public void setMaxPlayer(int num)
    {
        maxPlayer = num;
    }

    public override void OnConnectedToMaster()
    {
        for (int i = 0; i < joinButton.Length; i++)
            joinButton[i].interactable = true; //�� ���� ��ư Ȱ��ȭ
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
        setMaxPlayer(selectPlayerNum.value); //��Ӵٿ����� ������ ������ maxplayer ����
        roomPassword = roomPasswordInput.GetComponent<TMP_InputField>().text;
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
        roomPassword = roomPasswordInput.GetComponent<TMP_InputField>().text;
    }

    //�� �����ϱ� ��ư
    public void EnterRoom()
    {
        PhotonNetwork.JoinRoom(roomPassword);
    }

    public override void OnJoinedRoom()
    {
        connectionInfoText.text = "�� ���� ����";
        SceneManager.LoadScene("Room"); 


    }


}
