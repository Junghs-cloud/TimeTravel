using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using TMPro;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public Image[] playerListImg;
    public Image[] orderList;
    public bool isReady;

    public GameObject readyText;
    public GameObject infoText;
    public GameObject timeText;
    public GameObject setOrderPanel;
    public GameObject gameStartText;

    public Button pickCardBtn;
    public Button leaveRoomBtn;

    public Sprite[] itemImg;
    public Image[] items;

    public List<int> itemList = new List<int>();

    public List<int> playerOrderList = new List<int>();

    public int readyCounts;
    public int localPlayerIndex; //�÷��̾ �� ��°�� ���Դ���
    public int playerIndexWithOrder; //���� ���� �� �÷��̾ ���° ��������

    public bool setTimer;

    public PhotonView PV;

    public TMP_Text test;
    // Start is called before the first frame update
    void Awake()
    {
        //readyCounts = 1;
        foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
        {
            Debug.Log(player.NickName);
        }
        Debug.Log(PhotonNetwork.CurrentRoom.MaxPlayers);
        for (int i = 0; i < PhotonNetwork.CurrentRoom.MaxPlayers; i++)
        {
            playerListImg[i].gameObject.SetActive(true);
        }

        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            Debug.Log("playerList" + i + PhotonNetwork.PlayerList[i].NickName);
            playerListImg[i].transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = PhotonNetwork.PlayerList[i].NickName;

        }

        setLocalPlayerIndex();
        //timer���
        Timer();
        if (PhotonNetwork.IsMasterClient) //master�� ����Ʈ ����
        {
            for (int i = 0; i < PhotonNetwork.CurrentRoom.MaxPlayers; i++)
            {
                playerOrderList.Add(i);
            }
        }
        

    }

    public void Timer()
    {
        setTimer = true;
        timeText.SetActive(true);
        infoText.SetActive(true);
    }


    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if(readyCounts == PhotonNetwork.CurrentRoom.MaxPlayers)
                StartGame();
        }
        //Debug.Log(newPlayer.NickName);

        for(int i = 0; i < PhotonNetwork.CurrentRoom.MaxPlayers; i++)
        {
            if(playerListImg[i].GetComponentInChildren<TMP_Text>().text == "")
            {
                playerListImg[i].GetComponentInChildren<TMP_Text>().text = newPlayer.NickName;
                break;
            }
        }

        if (playerListImg[localPlayerIndex].transform.GetChild(2).gameObject.activeSelf == true)
        {
            playerListImg[localPlayerIndex].GetComponent<playerPanel>().setNewPlayerToReadyMe();
        }
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        for (int i = 0; i < PhotonNetwork.CurrentRoom.MaxPlayers; i++)
        {
            if (playerListImg[i].GetComponentInChildren<TMP_Text>().text == otherPlayer.NickName)
            {
                playerListImg[i].GetComponentInChildren<TMP_Text>().text = "";
                break;
            }
        }
    }

    public void PickItems()
    {
        setTimer = false;
        pickCardBtn.interactable = false;
        int ran;
        //������ �̱�
        for(int i = 0; i < 3; i++)
        {
            ran = Random.Range(0, 3);
            items[i].sprite = itemImg[ran];
            itemList.Add(ran); //item �ε��� ���� -->���� ������ list�� �����ؾ��ϴµ�...  ---> ���� ��ŸƮ �� �� dontDesoryObjects ��ũ��Ʈ�� ����.
        }
        ran = Random.Range(3, 6);
        items[3].sprite = itemImg[ran];
        itemList.Add(ran);

        for (int i = 0; i < 4; i++)
        {
            items[i].gameObject.SetActive(true);
            Debug.Log(itemList[i]);
        }

        //������ �̱� �Ϸ��ϸ� �ڵ� ����ǰ�
        playerListImg[localPlayerIndex].transform.transform.Find("Ready Text").gameObject.SetActive(true);
        playerListImg[localPlayerIndex].GetComponent<playerPanel>().setReadyToOther(localPlayerIndex);
        //leaveButton.interactable = false;
        
    }


    public List<int> ShuffleOrder(List<int> list)
    {

        for (int i = 0; i < list.Count; i++)
        {
            int ran = Random.Range(0, list.Count);
            int temp = list[i];
            list[i] = list[ran];
            list[ran] = temp;
        }

        return list;
    }

    public void UpdateListToOthers()
    {
        PV.RPC("UpdateList", RpcTarget.All, playerOrderList.ToArray());
    }

    [PunRPC]
    void UpdateList(int[] updatedList)
    {
        List<int> updatedPlayerList = new List<int>(updatedList);
        setOrderPanel.SetActive(true);
        //leaveRoomBtn.interactable = false;

        for (int i = 0; i < updatedPlayerList.Count; i++)
        {
            orderList[i].transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = "" + (i + 1) + ". " + PhotonNetwork.PlayerList[updatedPlayerList[i]].NickName;
            orderList[i].gameObject.SetActive(true);
        }

        if (!PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < updatedList.Length; i++)
            {
                playerOrderList.Add(updatedList[i]);
            }
        }
        DontDestroyObjects.instance.setPlayerListWithOrder(updatedPlayerList);
    }

    public void StartGame()
    {
        StartCoroutine(StartGameRoutine());
    }

    public void UpdateItemListToOthers()
    {
        setPlayerIndexWithOrder();
        
        PV.RPC("UpdateItemList", RpcTarget.AllBuffered, playerIndexWithOrder, itemList.ToArray() );
    }

    [PunRPC]
    void UpdateItemList(int playerIndexWithOrder, int[] updatedList)
    {
        DontDestroyObjects.instance.fillItemList(playerIndexWithOrder, updatedList);
    }

    IEnumerator StartGameRoutine()
    {
        /*if (!PhotonNetwork.IsMasterClient)
            return;*/
        //���� ���ϱ�
        if (PhotonNetwork.IsMasterClient)
        {
            playerOrderList = ShuffleOrder(playerOrderList);
            UpdateListToOthers();
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }
        
        yield return new WaitForSeconds(2f);

        //������ ���� �� �ٸ� ����鿡�� ����.
        UpdateItemListToOthers();
        gameStartText.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        PhotonNetwork.LoadLevel("Loading");//�ϴ��� ��� Ŭ���̾�Ʈ���� �� �ε� �ϴ°ɷ� �ϴµ� �̰� ���߿� master������ �ε��ϰ� ����ȭ���� �����ؾ��ҵ�

    }

    public void LeaveRoom()
    {
        pickCardBtn.interactable = true;
        leaveRoomBtn.interactable = true;
        for (int i = 0; i < 4; i++)
        {
            items[i].gameObject.SetActive(false);
        }
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("Main");
    }

    void setLocalPlayerIndex()
    {
        for (int i = 0; i < PhotonNetwork.CurrentRoom.MaxPlayers; i++)
        {
            if (PhotonNetwork.PlayerList[i].NickName == PhotonNetwork.LocalPlayer.NickName)
            {
                localPlayerIndex = i;
                break;
            }
        }
    }

    void setPlayerIndexWithOrder()
    {
        List<Photon.Realtime.Player> playerListWithOrder = DontDestroyObjects.instance.playerListWithOrder;
        for (int i = 0; i <playerListWithOrder.Count ; i++)
        {
            if (playerListWithOrder[i].NickName == PhotonNetwork.LocalPlayer.NickName)
            {
                playerIndexWithOrder = i;
                break;
            }
        }
        
    }
}
