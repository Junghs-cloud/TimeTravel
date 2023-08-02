using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using TMPro;

public class Dice : MonoBehaviour
{
    public GameManager manager;
    public Image curSide;
    public Sprite[] diceSides;
    public PhotonView PV;

    void Start()
    {
        this.gameObject.AddComponent<PhotonView>();
        PV = PhotonView.Get(this);

        PhotonNetwork.AllocateViewID(PV);
    }

    void OnEnable()
    {
        curSide.sprite = diceSides[0];
    }

    public void RollDice()
    {
        StartCoroutine(RollDiceRoutine());
    }

    IEnumerator RollDiceRoutine()
    {
        int ranSide = 0;
        for (int i = 0; i <= 10; i++)
        {
            ranSide = Random.Range(0, 6);
            curSide.sprite = diceSides[ranSide];
            yield return new WaitForSeconds(0.05f);
        }
        //ranSide + 1--> test������ �ٲٱ� checkcurPoint��;
        //manager.newDiceSide = ranSide + 1;
        manager.newDiceSide = ranSide + 1;
        yield return new WaitForSeconds(1f);
        if (!manager.secondRoll)
        {
            manager.diceImg.SetActive(false);
            manager.CheckCurPoint(GameManager.playerStartPoint + ranSide + 1);
        }
        else
        {
            Debug.Log("second roll");
            manager.MovePlayer();
        }
        yield return null;
    }

    [PunRPC]
    void testRPC()
    {
        GameManager.instance.RpcCheck("rpc complete");
    }
}
