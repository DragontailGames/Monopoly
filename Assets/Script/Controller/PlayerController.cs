using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    int position = 0;

    public PlayerControlerCanvas canvas;

    public BoardController boardController;

    public GameManager manager;

    public Material normal, transparent;

    public void Awake()
    {
        manager.players.Add(this);
    }

    public void Start()
    {
        //StartCoroutine(MovePlayer());
        canvas.ConfigureUI(null, "Player_" + Random.Range(1000, 9999), 3000000);
    }

    public void StartMovePlayer()
    {
        int dice1 = ThrowDice();
        int dice2 = ThrowDice();

        Debug.Log("Dice value 1: " + dice1 + " - 2: " + dice2);

        bool doubleDice = dice1 == dice2;

        int valueDice = dice1 + dice2;

        StartCoroutine(manager.OnMovePlayer(this, StartCoroutine(MovePlayer(valueDice)), doubleDice));
    }

    public int ThrowDice()
    {
        //return 6;
        return Random.Range(1,7);
    }

    public IEnumerator MovePlayer(int valueDice)
    {
        int dest = valueDice + position;
        for (int i = position + 1; i <= dest; i++)
        {
            TileController tile = boardController.tileControllers.Find(t => t.index == i);

            if(i+1 >= boardController.tileControllers.Count)
            {
                var tempDest = dest - i; 
                i = -1;
                dest = tempDest;
            }

            Vector3 targetPos = tile.transform.position;
            targetPos.y = this.transform.position.y;

            while (Vector3.Distance(transform.position, targetPos) > 0.01f)
            {
                float step = 3.5f * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, targetPos, step);
                yield return new WaitForSeconds(0.001f);
            }

            position = i;
            yield return tile.OnPlayerPass(this);
        }

    }

    public IEnumerator TurnCorner()
    {
        var newRot = this.transform.rotation;
        var startY = newRot.y;
        var desiredAngle = Quaternion.Euler(newRot.x, newRot.y - 90, newRot.y);

        yield return new WaitUntil(() =>
        {
            this.transform.rotation = Quaternion.Lerp(transform.rotation, desiredAngle, Time.deltaTime * 20.0f);
            if (Vector3.Distance(this.transform.rotation.eulerAngles, desiredAngle.eulerAngles) < 1f)
            {
                return true;
            }
            return false;
        });
    }

    public void ChangeMaterial(bool transparent)
    {
        if(transparent)
        {
            this.transform.GetComponent<MeshRenderer>().material = this.transparent;
        }
        else
        {
            this.transform.GetComponent<MeshRenderer>().material = normal;
        }
    }
}