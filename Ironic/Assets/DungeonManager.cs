using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonManager : MonoBehaviour
{
    public int roomCount = 30;
    public int minSize = 20;
    public int maxSize = 60;

    public Tilemap tilemap;

    [Header("타일 배열 설정")]
    [Tooltip("Element 0은 반드시 '회색(복도용)' 타일이어야 합니다.")]
    public TileBase[] floorTiles; 
    [Tooltip("Floor Tiles와 동일한 순서로 벽 타일을 넣어주세요.")]
    public TileBase[] wallTiles;  

    void Start() { Generate(); }

    public void Generate()
    {
        if (tilemap == null || floorTiles.Length == 0 || wallTiles.Length == 0) return;
        tilemap.ClearAllTiles();

        // 타일 개수를 전달하여 랜덤 범위를 제한
        DungeonGenerator gen = new DungeonGenerator(roomCount, minSize, maxSize, floorTiles.Length);

        foreach (var entry in gen.floorTileData) {
            if (entry.Value < floorTiles.Length)
                tilemap.SetTile((Vector3Int)entry.Key, floorTiles[entry.Value]);
        }

        foreach (var entry in gen.wallTileData) {
            if (entry.Value < wallTiles.Length)
                tilemap.SetTile((Vector3Int)entry.Key, wallTiles[entry.Value]);
        }
    }
}