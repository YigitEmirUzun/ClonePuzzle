using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour
{
    private PlayerControls playerControls;
    public LevelManager levelManager;

    // Karakterin grid pozisyonu
    private Vector2Int currentGridPos;

    // Karakterin hareket etme durumu
    private bool isMoving;

    // Karakterin hareket süresi
    [SerializeField] private float moveDuration = 0.2f;

    // Grid ve Tilemapler
    [SerializeField] private Grid grid;
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap wallTilemap;

    // Dokunmanýn baþlangýç ve bitiþ pozisyonu
    private Vector2 touchStartPos;
    private Vector2 touchEndPos;

    // Hareket olmasý için gereken baþlangýç ve bitiþ dokunmalarýnýn arasýndaki minimum pixel mesafesi
    public float minSwipeDistance = 50f;

    private List<Vector2Int> clonePositions = new List<Vector2Int>();
    [SerializeField] private GameObject clonePrefab;

    private void Awake()
    {
        // Input sistemini alma.
        playerControls = new PlayerControls();
        levelManager = FindAnyObjectByType<LevelManager>();
    }
    void Start()
    {
        Vector3Int cellPosition = grid.WorldToCell(transform.position);
        currentGridPos = new Vector2Int(cellPosition.x, cellPosition.y);
        transform.position = grid.GetCellCenterWorld(cellPosition);

        Debug.Log(IsWall(currentGridPos));

    }

    void Update()
    {
        if (isMoving || levelManager.outOfMoves)
        {
            return;
        }
            
        // Parmak basýldýðýnda
        if (playerControls.Player.TouchPress.WasPressedThisFrame())
        {
            // Parmaðýn basýldýðýndaki pozisyonu
            touchStartPos = playerControls.Player.TouchPosition.ReadValue<Vector2>();
        }

        // Parmak kaldýrýldýðýnda
        if (playerControls.Player.TouchPress.WasReleasedThisFrame())
        {
            // Parmaðýn kaldýrýldýðýndaki pozisyonu
            touchEndPos = playerControls.Player.TouchPosition.ReadValue<Vector2>();

            // Baþlangýç ve bitiþ dokunmalarýnýn arasýndaki mesafenin minumum deðeri saðladýðýnýn kontrolü.
            if(Vector2.Distance(touchStartPos,touchEndPos) >= minSwipeDistance && !levelManager.outOfMoves)
            {
                Vector2Int direction = Vector2Int.RoundToInt(GetSwipeDirection());
                Vector2Int targetPos = GetFarthestPosition(direction);

                Vector3 targetWorldPos = grid.GetCellCenterWorld(new Vector3Int(targetPos.x, targetPos.y, 0));

                if (targetPos != currentGridPos)
                {
                    levelManager.OnPlayerMove();
                    CreateClone();
                    isMoving = true;
                    transform.DOMove(targetWorldPos, moveDuration).SetEase(Ease.InOutBounce).OnComplete(() =>
                    {
                        isMoving = false;
                        currentGridPos = targetPos;
                        
                        levelManager.CheckWin(currentGridPos);
                    });
                }
                else { transform.DOShakeRotation(0.2f,0.6f); }

                    Debug.Log("Swipe Direction: " + direction);
            }
        }
    }

    private bool IsWall(Vector2Int position)
    {
        Vector3Int cellPosition = new Vector3Int(position.x, position.y,0);
        return wallTilemap.HasTile(cellPosition);
    }

    private bool CanMoveTo(Vector2Int position)
    {
        if (IsWall(position))
        {
            return false;
        }
        
        if (clonePositions.Contains(position))
        {
            return false;
        }
        
        return true;
    }

    private Vector2 GetSwipeDirection()
    {
        Vector2 swipe = touchEndPos - touchStartPos;

        if (Mathf.Abs(swipe.x) > Mathf.Abs(swipe.y))
        {
            if(swipe.x > 0)
            {
                return Vector2.right;
            }
            else { return Vector2.left; }
        }
        else
        {
            if(swipe.y > 0)
            {
                return Vector2.up;  
            }
            else { return Vector2.down; }
        }
    }

    private Vector2Int GetFarthestPosition(Vector2Int direction)
    {
        Vector2Int targetPos = currentGridPos;

        while (CanMoveTo(targetPos + direction))
        {
            targetPos += direction;
        }

        return targetPos;
    }

    private void CreateClone()
    {
        Vector3 clonePos = grid.GetCellCenterWorld(new Vector3Int(currentGridPos.x, currentGridPos.y, 0));

        Instantiate(clonePrefab, clonePos, Quaternion.identity);

        clonePositions.Add(currentGridPos);
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }
}
