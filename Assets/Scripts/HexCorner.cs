using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HexCorner
{
    public Vector3Int cubeCoordinates;  // キューブ座標で角の位置を管理
    public List<HexTile> adjacentTileList = new();  // 接しているタイル
    public Settlement settlement;  // 拠点情報

    public HexCorner(Vector3Int cubeCoordinates, List<HexTile> adjacentTileList, Settlement settlement) {
        this.cubeCoordinates = cubeCoordinates;
        this.adjacentTileList = new(adjacentTileList);
        this.settlement = settlement;
    }

    // 資源判定メソッド
    public void CheckResources(int diceRoll) {
        foreach (var tile in adjacentTileList) {
            if (tile.diceNumber == diceRoll) {
                // 資源獲得処理
                settlement.CollectResource(tile.tileType);
            }
        }
    }
}