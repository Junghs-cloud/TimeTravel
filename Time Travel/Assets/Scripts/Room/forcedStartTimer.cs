using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using TMPro;

public class forcedStartTimer : MonoBehaviour
{
    public TMP_Text timeText;
    public TMP_Text infoText;

    public GameObject forcedOutPanel;
    public float time = 60f;

    bool check;
    // Start is called before the first frame update

    void Start()
    {
        
    }

    void OnEnable()
    {
        time = 30f;
        check = false;
        for (int i = 0; i < PhotonNetwork.CurrentRoom.MaxPlayers; i++)
        {
            if (RoomManager.instance.playerListImg[i].GetComponentInChildren<TMP_Text>().text == PhotonNetwork.LocalPlayer.NickName && RoomManager.instance.playerListImg[i].transform.GetChild(2).gameObject.activeSelf == false)
            {
                Color textColor = new Color(0f / 255f, 0f / 255f, 0f / 255f);
                textColor.a = 0.0f;
                this.gameObject.GetComponent<TMP_Text>().color = textColor;
            }
            else if (RoomManager.instance.playerListImg[i].GetComponentInChildren<TMP_Text>().text == PhotonNetwork.LocalPlayer.NickName && RoomManager.instance.playerListImg[i].transform.GetChild(2).gameObject.activeSelf == true)
            {
                Color textColor = new Color(0f / 255f, 0f / 255f, 0f / 255f);
                textColor.a = 1.0f;
                this.gameObject.GetComponent<TMP_Text>().color = textColor;
            }
            else
            {
                continue;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        time -= Time.deltaTime;
        timeText.text = string.Format("{0:F0}��", time);
        infoText.text = "�ð��� ������ �ڵ����� ���۵˴ϴ�.\n �ٸ� ����� ���� ���� �ϸ� �ð��� ���ŵ˴ϴ�.";
        if (time <= 0)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount==1)
            {
                forcedOutPanel.SetActive(true);
                timeText.text = "";
                if (check == false)
                {
                    Invoke("leaveRoom", 1.5f);
                    check = true;
                }
            }
            else if (RoomManager.instance.readyCounts != PhotonNetwork.CurrentRoom.MaxPlayers && RoomManager.instance.readyCounts == PhotonNetwork.CurrentRoom.PlayerCount)
            {
                RoomManager.instance.StartGame();
                timeText.gameObject.SetActive(false);
            }
            else
            {
                infoText.text = "���������� ���� ����� ī�� �̱⸦ ��ٸ��� ���Դϴ�.\n";
                timeText.text = "";
            }
        }
    }

    void leaveRoom()
    {
        RoomManager.instance.LeaveRoom();
    }
}
