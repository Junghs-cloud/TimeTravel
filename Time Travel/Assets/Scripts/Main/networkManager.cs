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

    public TMP_Text connectionInfoText;    //접속 정보 표시 텍스트
    public TMP_InputField nickNameInput; //닉네임 입력
    public Button[] joinButton;   //룸 접속 버튼

    public Image roomMakePanel;
    public TMP_Dropdown selectPlayerNum;
    public TMP_InputField roomPasswordInput;
    string roomPassword;

    public Image roomEnterPanel;

    void Start()
    {
        Screen.SetResolution(1920, 1080, false);
        PhotonNetwork.ConnectUsingSettings(); //마스터 서버 접속 시도
        for(int i = 0; i < joinButton.Length;i++)
            joinButton[i].interactable = false; //접속 시도시 버튼 클릭 못하게
        connectionInfoText.text = "마스터 서버에 접속 중...";
        PhotonNetwork.LocalPlayer.NickName = nickNameInput.text; //닉네임 입력
        maxPlayer = 4;
    }

    void Update()
    {
        if(nickNameInput.text != "")
        {
            for (int i = 0; i < joinButton.Length; i++)
                joinButton[i].interactable = true; //룸 접속 버튼 활성화
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

    public override void OnConnectedToMaster()
    {
        connectionInfoText.text = "온라인 : 마스터 서버와 연결됨";
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        for (int i = 0; i < joinButton.Length; i++)
            joinButton[i].interactable = false;
        connectionInfoText.text = "오프라인 : 마스터 서버와 연결되지 않음\n 접속 재시도 중...";
        PhotonNetwork.ConnectUsingSettings();
    }

    //Join Random Room
    public void Connect()
    {
        for (int i = 0; i < joinButton.Length; i++)
            joinButton[i].interactable = false;
        if (PhotonNetwork.IsConnected)
        {
            connectionInfoText.text = "룸에 접속...";
            PhotonNetwork.JoinRandomRoom(null, (byte)maxPlayer);

        }
        else
        {
            connectionInfoText.text = "오프라인 : 마스터 서버와 연결되지 않음\n접속 재시도 중...";
            PhotonNetwork.ConnectUsingSettings(); //접속 재시도
        }
    }

    //랜덤 룸 입장 실패시 새로운 랜덤 룸 생성
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        connectionInfoText.text = "빈 방이 없음, 새로운 방 생성...";
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayer }); //2,3,4명 수용가능한 방 만들기

    }


    //Make Room
    public void ClickCreateRoom()
    {
        roomMakePanel.gameObject.SetActive(true);
        setMaxPlayer(selectPlayerNum.value); //드롭다운으로 선택한 값으로 maxplayer 설정
        roomPassword = roomPasswordInput.GetComponent<TMP_InputField>().text;
    }

    //방 만들기 버튼
    public void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = maxPlayer };
        PhotonNetwork.CreateRoom(roomPassword, roomOptions);
    }

    //방 만들기 실패하면
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        connectionInfoText.text = "방 만들기 실패, 재시도중...";
        CreateRoom();
    }

    //Enter Room
    public void ClickEnterRoom()
    {
        roomEnterPanel.gameObject.SetActive(true);
        roomPassword = roomPasswordInput.GetComponent<TMP_InputField>().text;
    }

    //방 입장하기 버튼
    public void EnterRoom()
    {
        PhotonNetwork.JoinRoom(roomPassword);
    }

    public override void OnJoinedRoom()
    {
        connectionInfoText.text = "방 참가 성공.";
        SceneManager.LoadScene("Room"); 

    }

    public void setPlayerNickName()
    {
        PhotonNetwork.LocalPlayer.NickName = nickNameInput.text;
    }
}
