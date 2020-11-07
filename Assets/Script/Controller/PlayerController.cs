using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    int position = 0;

    public PlayerControlerCanvas canvas;

    public BoardController boardController;

    public GameManager manager;

    public void Awake()
    {
        manager.players.Add(this);
    }

    public void Start()
    {
        StartCoroutine(MovePlayer());
        canvas.ConfigureUI(null, "Player_" + Random.Range(1000, 9999), 3000000);
    }

    public int ThrowDice()
    {
        //return 6;
        return Random.Range(1,7);
    }

    public IEnumerator MovePlayer()
    {
        int dice1 = ThrowDice();
        int dice2 = ThrowDice();

        Debug.Log("Dice value 1: " + dice1 + " - 2: " + dice2);

        for (int i = position + 1; i <= (dice1 + dice2 + position); i++)
        {
            TileController tile = boardController.tileControllers.Find(t => t.index == i);

            Vector3 targetPos = tile.transform.position;
            targetPos.y = this.transform.position.y;

            while (Vector3.Distance(transform.position, targetPos) > 0.01f)
            {
                float step = 3.5f * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, targetPos, step);
                yield return new WaitForSeconds(0.001f);
            }

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
            this.transform.rotation = Quaternion.Lerp(transform.rotation, desiredAngle, Time.deltaTime * 15.0f);
            if (Vector3.Distance(this.transform.rotation.eulerAngles, desiredAngle.eulerAngles) < 1f)
            {
                return true;
            }
            return false;
        });
    }
}