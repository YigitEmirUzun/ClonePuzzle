using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour
{
    // Input Sistemi
    private PlayerControls playerControls;
    
    // Diger Scriptler
    public LevelManager levelManager;

    // Karakterin grid pozisyonu
    private Vector2Int currentGridPos;

    // Karakterin hareket etme durumu
    private bool isMoving;

    // Karakterin hareket suresi
    [SerializeField] private float moveDuration = 0.2f;

    // Grid ve Tilemapler
    [SerializeField] private Grid grid;
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap wallTilemap;

    // Dokunmanýn baslangic ve bitis pozisyonu
    private Vector2 touchStartPos;
    private Vector2 touchEndPos;

    // Hareket olmasi icin gereken baslangic ve bitis dokunmalarinin arasindaki minimum pixel mesafesi
    public float minSwipeDistance = 50f;

    // Klonlarýn pozisyonunu tutan liste ve klon prefabi (kontrol icin)
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
        // Hedef frame rate i ayarlama
        Application.targetFrameRate = 60;

        // cellPosition icerisine karakterin konumunun tilemap de hangi kareye denk geldigini atiyorum
        Vector3Int cellPosition = grid.WorldToCell(transform.position);
        currentGridPos = new Vector2Int(cellPosition.x, cellPosition.y);
        // Karakteri gorsel olarak olmasi gereken tilemap karesine yerlestiriyorum
        transform.position = grid.GetCellCenterWorld(cellPosition);

    }

    void Update()
    {
        // Karakter zaten hareket halindeyse ya da hamle sayisi bittiyse Updatei calistirmiyorum
        if (isMoving || levelManager.outOfMoves)
        {
            return;
        }
            
        // Parmak basildiðinda
        if (playerControls.Player.TouchPress.WasPressedThisFrame())
        {
            // Parmagin basildigindaki pozisyonu
            touchStartPos = playerControls.Player.TouchPosition.ReadValue<Vector2>();
        }

        // Parmak kaldirildiginda
        if (playerControls.Player.TouchPress.WasReleasedThisFrame())
        {
            // Parmagin kaldirildigindaki pozisyonu
            touchEndPos = playerControls.Player.TouchPosition.ReadValue<Vector2>();

            // Baslangic ve bitis dokunmalarinin arasindaki mesafenin minumum degeri sagladiðinin kontrolu.
            if(Vector2.Distance(touchStartPos,touchEndPos) >= minSwipeDistance && !levelManager.outOfMoves)
            {
                Vector2Int direction = Vector2Int.RoundToInt(GetSwipeDirection());
                Vector2Int targetPos = GetFarthestPosition(direction);

                Vector3 targetWorldPos = grid.GetCellCenterWorld(new Vector3Int(targetPos.x, targetPos.y, 0));

                // Karakterimizin belirtilen yonde hareket edebilme durumuna gore hareketi yapiyoruz
                if (targetPos != currentGridPos)
                {
                    levelManager.OnPlayerMove();
                    CreateClone();
                    isMoving = true;
                    transform.DOMove(targetWorldPos, moveDuration).SetEase(Ease.Linear).OnComplete(() =>
                    {
                        isMoving = false;
                        currentGridPos = targetPos;
                        
                        // Her harekette oyuncunun bitise ulasip ulasmadiginin kontrolu
                        levelManager.CheckWin(currentGridPos);
                    });
                }
                else 
                {
                    // Karakterin hareket edebilecegi yon yoksa karakterin rotasyonunu salliyoruz feedback için
                    transform.DOShakeRotation(0.2f,0.6f); 
                }
            }
        }
    }

    // Duvar var mi kontrolu
    private bool IsWall(Vector2Int position)
    {
        Vector3Int cellPosition = new Vector3Int(position.x, position.y,0);
        return wallTilemap.HasTile(cellPosition);
    }

    // Target pozisyonuna hareket edilebilir mi bool kontrolu
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

    // Kaydirmanin yonu
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

    // Kaydirilan yone gidebilecegimiz en uzak kare (tile)
    private Vector2Int GetFarthestPosition(Vector2Int direction)
    {
        Vector2Int targetPos = currentGridPos;

        while (CanMoveTo(targetPos + direction))
        {
            targetPos += direction;
        }

        return targetPos;
    }

    // Klon yaratimi
    private void CreateClone()
    {
        Vector3 clonePos = grid.GetCellCenterWorld(new Vector3Int(currentGridPos.x, currentGridPos.y, 0));

        Instantiate(clonePrefab, clonePos, Quaternion.identity);

        clonePositions.Add(currentGridPos);
    }

    private void OnEnable()
    {
        // Input sistemi aktif etme
        playerControls.Enable();
    }

    private void OnDisable()
    {
        // Input sistemi devre disi birakma
        playerControls.Disable();
    }
}
