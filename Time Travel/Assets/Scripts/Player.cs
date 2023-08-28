using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

public class Player : MonoBehaviour
{
    public GameManager manager;
    public Transform[] points;

    public float speed = 15f;
    public bool movingAllowed;
    public bool moveLadder;
    public int curIndex;
    public int ran;
    public int correctCount;

    void Start()
    {
        transform.position = points[curIndex].transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (movingAllowed)
            MovePath();
        if (moveLadder)
            MoveLadder();
    }

    public void MovePath()
    {
        

        if (curIndex <= points.Length - 1)
        {
            transform.position = Vector2.MoveTowards(transform.position, points[curIndex].transform.position, speed * Time.deltaTime);
            if (transform.position == points[curIndex].transform.position)
            {
                curIndex++;
            }


            if(curIndex == 10)
            {
                GameManager.instance.ChangeClothes("�ﱹ�ô�");
            }
            else if (curIndex == 21)
            {
                GameManager.instance.Flip(true);
            }
            else if(curIndex == 22)
            {
                GameManager.instance.ChangeClothes("����ô�");
                
            }
            else if(curIndex == 33)
            {
                GameManager.instance.Flip(false);
            }
            else if (curIndex == 42)
            {
                GameManager.instance.ChangeClothes("�����ô�");
            }
            else if (curIndex == 45)
            {
                GameManager.instance.Flip(true);
            }
            else if (curIndex == 57)
            {
                GameManager.instance.Flip(false);
            }
            else if (curIndex == 69)
            {
                GameManager.instance.Flip(true);
            }
            else if (curIndex == 72)
            {
                GameManager.instance.ChangeClothes("������");
            }
            else if (curIndex == 81)
            {
                GameManager.instance.Flip(false);
            }

            //�������� ���޽� ���� ���� 
            if (curIndex == points.Length - 1)
            {
                movingAllowed = false;
                GameManager.instance.EndGame(PhotonNetwork.LocalPlayer.ToString());
            }


        }

    }

    //��ٸ�ĭ �̵�
    public void MoveLadder()
    {

        switch (curIndex - 1)
        {
            case 7:
                transform.position = Vector2.MoveTowards(transform.position, points[36].transform.position, speed * Time.deltaTime);
                GameManager.instance.playerStartPoint[GameManager.instance.controlPlayerIndexWithOrder] = 36;
                if(transform.position == points[36].transform.position)
                {
                    GameManager.instance.ChangeClothes("����ô�");
                    curIndex = 37;
                    manager.isLadder = false;
                    manager.finishRound = true;
                    GameManager.instance.UISmaller();
                } 
                break;
            case 22:
                transform.position = Vector2.MoveTowards(transform.position, points[48].transform.position, speed * Time.deltaTime);
                GameManager.instance.playerStartPoint[GameManager.instance.controlPlayerIndexWithOrder] = 48;

                if (transform.position == points[48].transform.position)
                {
                    GameManager.instance.ChangeClothes("�����ô�");
                    curIndex = 49;
                    manager.isLadder = false;
                    manager.finishRound = true;
                    GameManager.instance.UISmaller();
                }
                break;
            case 53:
                transform.position = Vector2.MoveTowards(transform.position, points[60].transform.position, speed * Time.deltaTime);
                GameManager.instance.playerStartPoint[GameManager.instance.controlPlayerIndexWithOrder] = 60;

                if (transform.position == points[60].transform.position)
                {
                    GameManager.instance.Flip(false);
                    curIndex = 61;
                    manager.isLadder = false;
                    manager.finishRound = true;
                    GameManager.instance.UISmaller();
                }
                break;
            case 64:
                transform.position = Vector2.MoveTowards(transform.position, points[73].transform.position, speed * Time.deltaTime);
                GameManager.instance.playerStartPoint[GameManager.instance.controlPlayerIndexWithOrder] = 73;

                if (transform.position == points[73].transform.position)
                {
                    GameManager.instance.ChangeClothes("������");
                    GameManager.instance.Flip(true);
                    curIndex = 74;
                    manager.isLadder = false;
                    manager.finishRound = true;
                    GameManager.instance.UISmaller();
                }
                break;
            case 76:
                transform.position = Vector2.MoveTowards(transform.position, points[84].transform.position, speed * Time.deltaTime);
                GameManager.instance.playerStartPoint[GameManager.instance.controlPlayerIndexWithOrder] = 84;

                if (transform.position == points[84].transform.position)
                {
                    GameManager.instance.Flip(false);
                    curIndex = 85;
                    manager.isLadder = false;
                    manager.finishRound = true;
                    GameManager.instance.UISmaller();
                }
                break;
        }

    }

    public void Transport()
    {
        switch (curIndex - 1)
        {
            case 15:
                transform.position = points[30].transform.position;
                GameManager.instance.playerStartPoint[GameManager.instance.controlPlayerIndexWithOrder] = 30;
                if (transform.position == points[30].transform.position)
                {
                    GameManager.instance.ChangeClothes("����ô�");
                    GameManager.instance.Flip(true);
                    curIndex = 31;
                    manager.isTransport = false;
                    manager.finishRound = true;
                    GameManager.instance.UISmaller();
                }
                break;
            case 26:
                transform.position = points[38].transform.position;
                GameManager.instance.playerStartPoint[GameManager.instance.controlPlayerIndexWithOrder] = 38;
                if (transform.position == points[38].transform.position)
                {
                    GameManager.instance.Flip(false);
                    curIndex = 39;
                    manager.isTransport = false;
                    manager.finishRound = true;
                    GameManager.instance.UISmaller();
                }
                break;
            case 30:
                transform.position = points[15].transform.position;
                GameManager.instance.playerStartPoint[GameManager.instance.controlPlayerIndexWithOrder] = 15;
                if (transform.position == points[15].transform.position)
                {
                    GameManager.instance.ChangeClothes("�ﱹ�ô�");
                    GameManager.instance.Flip(false);
                    curIndex = 16;
                    manager.isTransport = false;
                    manager.finishRound = true;
                    GameManager.instance.UISmaller();
                }
                break;
            case 32:
                transform.position = points[80].transform.position;
                GameManager.instance.playerStartPoint[GameManager.instance.controlPlayerIndexWithOrder] = 80;
                if (transform.position == points[80].transform.position)
                {
                    GameManager.instance.ChangeClothes("������");
                    GameManager.instance.Flip(false);
                    curIndex = 81;
                    manager.isTransport = false;
                    manager.finishRound = true;
                    GameManager.instance.UISmaller();
                }
                break;
            case 38:
                transform.position = points[26].transform.position;
                GameManager.instance.playerStartPoint[GameManager.instance.controlPlayerIndexWithOrder] = 26;
                if (transform.position == points[26].transform.position)
                {
                    GameManager.instance.Flip(true);
                    curIndex = 27;
                    manager.isTransport = false;
                    manager.finishRound = true;
                    GameManager.instance.UISmaller();
                }
                break;
            case 46:
                transform.position = points[67].transform.position;
                GameManager.instance.playerStartPoint[GameManager.instance.controlPlayerIndexWithOrder] = 67;
                if (transform.position == points[67].transform.position)
                {
                    GameManager.instance.Flip(false);
                    curIndex = 48;
                    manager.isTransport = false;
                    manager.finishRound = true;
                    GameManager.instance.UISmaller();
                }
                break;
            case 62:
                transform.position = points[90].transform.position;
                GameManager.instance.playerStartPoint[GameManager.instance.controlPlayerIndexWithOrder] = 90;
                if (transform.position == points[90].transform.position)
                {
                    GameManager.instance.ChangeClothes("������");
                    curIndex = 91;
                    manager.isTransport = false;
                    manager.finishRound = true;
                    GameManager.instance.UISmaller();
                }
                break;
            case 67:
                transform.position = points[46].transform.position;
                GameManager.instance.playerStartPoint[GameManager.instance.controlPlayerIndexWithOrder] = 46;
                if (transform.position == points[46].transform.position)
                {
                    GameManager.instance.Flip(true);
                    curIndex = 47;
                    manager.isTransport = false;
                    manager.finishRound = true;
                    GameManager.instance.UISmaller();
                }
                break;
            case 80:
                transform.position = points[32].transform.position;
                GameManager.instance.playerStartPoint[GameManager.instance.controlPlayerIndexWithOrder] = 32;
                if (transform.position == points[32].transform.position)
                {
                    GameManager.instance.ChangeClothes("����ô�");
                    GameManager.instance.Flip(false);
                    curIndex = 33;
                    manager.isTransport = false;
                    manager.finishRound = true;
                    GameManager.instance.UISmaller();
                }
                break;
            case 90:
                transform.position = points[62].transform.position;
                GameManager.instance.playerStartPoint[GameManager.instance.controlPlayerIndexWithOrder] = 62;
                if (transform.position == points[62].transform.position)
                {
                    GameManager.instance.ChangeClothes("�����ô�");
                    curIndex = 63;
                    manager.isTransport = false;
                    manager.finishRound = true;
                    GameManager.instance.UISmaller();
                }
                break;
           
        }
    }
}
