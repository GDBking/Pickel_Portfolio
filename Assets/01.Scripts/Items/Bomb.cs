using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Bomb : Item
{
    [Header("Effect")]
    [SerializeField] GameObject bombParticlePrefab;

    [Header("Pattern")]
    [SerializeField] DamagePattern pattern;

    [Header("Delay")]
    [SerializeField] float delayPerStep = 0.1f;

    [Header("Sound")]
    [SerializeField] AudioClip explosionSound;

    public override void UseItem(Vector2 hitNormal)
    {
        base.UseItem(hitNormal);
        StartCoroutine(BombDelay(hitNormal));

        SaveManager.saveData.endingLog.bombCnt++;
    }

    IEnumerator BombDelay(Vector2 hitNormal)
    {
        if (pattern == null || pattern.cells == null)
            yield break;

        // 1) distance 기준으로 그룹화
        Dictionary<int, List<DamageCell>> waves = new();

        foreach (var cell in pattern.cells)
        {
            int distance = Mathf.Max(Mathf.Abs(cell.offset.x), Mathf.Abs(cell.offset.y));

            if (!waves.TryGetValue(distance, out var list))
            {
                list = new List<DamageCell>();
                waves[distance] = list;
            }

            list.Add(cell);
        }

        // 2) distance 순서대로 정렬
        List<int> sortedKeys = new(waves.Keys);
        sortedKeys.Sort();

        // 3) 거리 증가할 때마다 딜레이 누적
        foreach (int d in sortedKeys)
        {
            yield return new WaitForSeconds(delayPerStep);

            AudioManager.Instance.PlaySfx(explosionSound);

            foreach (var cell in waves[d])
            {
                Vector3Int offset = cell.offset;
                Vector2 direction = cell.direction;

                if (pattern.isDir)
                {
                    offset = Rotate(offset, hitNormal);
                    direction = RotateDir(direction, hitNormal);
                }

                Vector3Int targetPos = cellPos + offset;

                if (playerDig.TryDig(targetPos, direction, 9999, true))
                {
                    Instantiate(
                        bombParticlePrefab,
                        tilemap.GetCellCenterWorld(targetPos),
                        Quaternion.identity
                    );
                }
            }
        }

        Destroy(gameObject);
    }

    Vector3Int Rotate(Vector3Int v, Vector2 dir)
    {
        Vector3Int rotation = dir switch
        {
            var n when n.x < -0.5f => new Vector3Int(-v.y, v.x, 0),
            var n when n.x > 0.5f => new Vector3Int(v.y, -v.x, 0),
            var n when n.y > 0.5f => v,
            _ => new Vector3Int(-v.x, -v.y, 0)
        };

        return rotation;
    }

    Vector2 RotateDir(Vector2 v, Vector2 dir)
    {
        Vector2 rotation = dir switch
        {
            var n when n.x < -0.5f => new Vector2(-v.y, v.x),
            var n when n.x > 0.5f => new Vector2(v.y, -v.x),
            var n when n.y > 0.5f => v,
            _ => new Vector2(-v.x, -v.y)
        };

        return rotation;
    }
}