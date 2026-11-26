using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class LevelManager : MonoBehaviour
{
    public TextMeshProUGUI moveText;
    
    public int maxMoveCount;
    public int moveCount;

    public bool outOfMoves = false;
    public bool finished = false;

    [SerializeField] private Tilemap finishTilemap;

    private void Awake()
    {
        moveText.text = maxMoveCount.ToString();
    }


    public void OnPlayerMove()
    {
        moveCount++;

        if(moveCount >= maxMoveCount)
        {
            outOfMoves = true;
            StartCoroutine(RestartRoutine());
        }

        moveText.text = (maxMoveCount - moveCount).ToString();

    }

    public void CheckWin(Vector2Int playerPosition)
    {
        Vector3Int cellPosition = new Vector3Int(playerPosition.x,playerPosition.y,0);

        if (finishTilemap.HasTile(cellPosition))
        {
            finished = true;
            Debug.Log("Level Completed!");

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    IEnumerator RestartRoutine()
    {
        yield return new WaitForSeconds(0.4f);
        if (!finished)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
