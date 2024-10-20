using UnityEngine;

[System.Serializable]
public class HexTile
{
    public TileType tileType;
    public int diceNumber; // 2-12
    public Vector3Int cubePosition; // キューブ座標系 (x, y, z) は x + y + z = 0 という制約がある

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="type"></param>
    /// <param name="num"></param>
    /// <param name="position"></param>
    public HexTile(TileType type, int num, Vector3Int position) {
        tileType = type;
        diceNumber = num;
        cubePosition = position;
    }
}