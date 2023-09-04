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
