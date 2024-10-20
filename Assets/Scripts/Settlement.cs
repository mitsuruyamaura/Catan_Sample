using UnityEngine;

[System.Serializable]
public class Settlement {
    public Vector3Int cubeCoordinates;
    public TileType tileType;            // 例えば「村」や「都市」など
    public OwnerType ownerType;          // プレイヤーの情報など

    public Settlement(Vector3Int cubeCoordinates, TileType tileType, OwnerType ownerType) {
        this.cubeCoordinates = cubeCoordinates;
        this.tileType = tileType;
        this.ownerType = ownerType;
    }

    public void CollectResource(TileType tileType) {
        // オーナーに資源を付与

    }
}