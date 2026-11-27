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


    // Her Harekette çalýþan fonksiyon
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

    // Karakterin bitiþ karesinde bulunup bulunmadýðýnýn kontrolü
    public void CheckWin(Vector2Int playerPosition)
    {
        Vector3Int cellPosition = new Vector3Int(playerPosition.x,playerPosition.y,0);

        if (finishTilemap.HasTile(cellPosition))
        {
            finished = true;
            Debug.Log("Level Completed!");

            if (SceneManager.GetActiveScene().buildIndex + 1 < SceneManager.sceneCountInBuildSettings) 
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
            else { SceneManager.LoadScene(0); }
        }
    }

    // Hamle bittiðinde bölümü baþtan baþlatma (anýnda olmasýn diye 0.4 saniye delay)
    IEnumerator RestartRoutine()
    {
        yield return new WaitForSeconds(0.4f);
        if (!finished)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
