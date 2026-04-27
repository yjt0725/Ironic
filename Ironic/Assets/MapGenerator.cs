using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public class Block
    {
        public Rect rect;
        public Block(Rect r) { rect = r; }
    }

    [Header("맵 설정")]
    public float mapWidth = 1500f;
    public float mapHeight = 1500f;
    public int roomCount = 30;

    [Header("방 설정")]
    public float minSize = 60f;
    public float maxSize = 80f;
    // [중요] 이 수치를 높일수록 방 사이가 멀어집니다.
    public float padding = 15f; 

    [Header("프리팹")]
    public GameObject roomPrefab;

    private List<Block> blocks = new List<Block>();

    void Start() => GenerateMap();

    public void GenerateMap()
    {
        foreach (Transform child in transform) Destroy(child.gameObject);
        blocks.Clear();

        Vector2 mapCenter = new Vector2(mapWidth / 2f, mapHeight / 2f);

        // 1. 초기 생성 (중앙 밀집)
        for (int i = 0; i < roomCount; i++)
        {
            float w = Random.Range(minSize, maxSize);
            float h = Random.Range(minSize, maxSize);
            Vector2 randomPos = Random.insideUnitCircle * 30f;
            blocks.Add(new Block(new Rect(mapCenter.x + randomPos.x, mapCenter.y + randomPos.y, w, h)));
        }

        // 2. 밀어내기 실행
        RepositionBlocks(mapCenter);

        // 3. 소환
        foreach (Block b in blocks)
        {
            GameObject go = Instantiate(roomPrefab, new Vector3(b.rect.x, b.rect.y, 0), Quaternion.identity, transform);
            // 30x30 타일 프리팹 기준 스케일
            go.transform.localScale = new Vector3(b.rect.width / 30f, b.rect.height / 30f, 1f);
        }
    }

    private void RepositionBlocks(Vector2 center)
    {
        int safetyNet = 0;
        while (safetyNet < 3000) // 반복 횟수를 더 늘림
        {
            bool overlapFound = false;
            for (int i = 0; i < blocks.Count; i++)
            {
                for (int j = i + 1; j < blocks.Count; j++)
                {
                    // [핵심] 실제 크기보다 Padding만큼 더 크게 잡아서 겹침 검사
                    Rect r1_padded = new Rect(blocks[i].rect.x - padding, blocks[i].rect.y - padding, blocks[i].rect.width + padding * 2, blocks[i].rect.height + padding * 2);
                    
                    if (r1_padded.Overlaps(blocks[j].rect))
                    {
                        overlapFound = true;
                        ResolveOverlap(center, blocks[i], blocks[j]);
                    }
                }
            }
            if (!overlapFound) break;
            safetyNet++;
        }
    }

    private void ResolveOverlap(Vector2 center, Block b1, Block b2)
    {
        Rect r1 = b1.rect;
        Rect r2 = b2.rect;

        float dx = Mathf.Min(Mathf.Abs(r1.xMax - r2.xMin), Mathf.Abs(r2.xMax - r1.xMin));
        float dy = Mathf.Min(Mathf.Abs(r1.yMax - r2.yMin), Mathf.Abs(r2.yMax - r1.yMin));

        // 밀어내는 힘을 2.0f로 상향 (더 시원하게 밀림)
        float push = 2.0f;

        if (dx < dy)
        {
            if (r1.center.x < r2.center.x) { r2.x += push; r1.x -= push; }
            else { r1.x += push; r2.x -= push; }
        }
        else
        {
            if (r1.center.y < r2.center.y) { r2.y += push; r1.y -= push; }
            else { r1.y += push; r2.y -= push; }
        }

        b1.rect = r1;
        b2.rect = r2;
    }
}