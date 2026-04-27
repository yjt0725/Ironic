using System.Collections.Generic;
using UnityEngine;

public class RoomData
{
    public RectInt area;
    public int type;
    public RoomData(RectInt rect, int type) { this.area = rect; this.type = type; }
}

public class DungeonGenerator
{
    public Dictionary<Vector2Int, int> floorTileData = new Dictionary<Vector2Int, int>();
    public Dictionary<Vector2Int, int> wallTileData = new Dictionary<Vector2Int, int>();
    public List<RoomData> rooms = new List<RoomData>();

    public DungeonGenerator(int roomCount, int minSize, int maxSize, int maxTypes)
    {
        float currentRange = 5f;
        int attempts = 0;

        // 1. 방 위치 결정 및 방 색상 부여 (0번 '회색' 제외)
        while (rooms.Count < roomCount && attempts < 10000)
        {
            attempts++;
            float angle = Random.Range(0, Mathf.PI * 2);
            float radius = Random.Range(0, currentRange);
            int x = Mathf.RoundToInt(Mathf.Cos(angle) * radius);
            int y = Mathf.RoundToInt(Mathf.Sin(angle) * radius);
            int w = Random.Range(minSize, maxSize + 1);
            int h = Random.Range(minSize, maxSize + 1);

            RectInt rect = new RectInt(x, y, w, h);
            bool overlap = false;
            foreach (var r in rooms) {
                if (rect.Overlaps(new RectInt(r.area.x - 3, r.area.y - 3, r.area.width + 6, r.area.height + 6))) {
                    overlap = true; break;
                }
            }

            if (!overlap) {
                // maxTypes가 2개 이상일 때만 1번부터 선택, 아니면 0번 사용
                int type = (maxTypes > 1) ? Random.Range(1, maxTypes) : 0;
                rooms.Add(new RoomData(rect, type));
            }
            currentRange += 0.2f;
        }

        // 2. 복도를 0번(회색)으로 먼저 깔기
        ConnectRoomsPrim();

        // 3. 방 타일을 복도 위에 덮어쓰기 (방 색상은 1번부터 시작)
        foreach (var room in rooms) {
            for (int rx = room.area.xMin; rx < room.area.xMax; rx++)
                for (int ry = room.area.yMin; ry < room.area.yMax; ry++)
                    floorTileData[new Vector2Int(rx, ry)] = room.type;
        }

        // 4. 벽 생성
        BuildWallsWithTypes();
    }

    void ConnectRoomsPrim()
    {
        if (rooms.Count < 2) return;
        List<RoomData> connected = new List<RoomData> { rooms[0] };
        List<RoomData> unconnected = new List<RoomData>(rooms);
        unconnected.RemoveAt(0);

        while (unconnected.Count > 0)
        {
            float minDistance = float.MaxValue;
            RoomData bestA = null, bestB = null;

            foreach (var a in connected) {
                foreach (var b in unconnected) {
                    float dist = Vector2.Distance(a.area.center, b.area.center);
                    if (dist < minDistance) { minDistance = dist; bestA = a; bestB = b; }
                }
            }
            // 복도는 무조건 0번(회색) 고정
            CreateCorridor(bestA.area, bestB.area, 0); 
            connected.Add(bestB);
            unconnected.Remove(bestB);
        }
    }

    void CreateCorridor(RectInt roomA, RectInt roomB, int type)
    {
        Vector2Int posA = Vector2Int.RoundToInt(roomA.center);
        Vector2Int posB = Vector2Int.RoundToInt(roomB.center);

        for (int x = Mathf.Min(posA.x, posB.x); x <= Mathf.Max(posA.x, posB.x); x++)
            for (int i = -1; i <= 1; i++) floorTileData[new Vector2Int(x, posA.y + i)] = type;
            
        for (int y = Mathf.Min(posA.y, posB.y); y <= Mathf.Max(posA.y, posB.y); y++)
            for (int i = -1; i <= 1; i++) floorTileData[new Vector2Int(posB.x + i, y)] = type;
    }

    void BuildWallsWithTypes()
    {
        foreach (var entry in floorTileData) {
            Vector2Int pos = entry.Key;
            int type = entry.Value;
            for (int x = -1; x <= 1; x++)
                for (int y = -1; y <= 1; y++) {
                    Vector2Int neighbor = new Vector2Int(pos.x + x, pos.y + y);
                    if (!floorTileData.ContainsKey(neighbor)) wallTileData[neighbor] = type;
                }
        }
    }
}