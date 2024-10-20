using UnityEngine;

[System.Serializable]
public class HexTile
{
    public TileType tileType;
    public int diceNumber; // 2-12
    public Vector3Int cubePosition; // �L���[�u���W�n (x, y, z) �� x + y + z = 0 �Ƃ������񂪂���

    /// <summary>
    /// �R���X�g���N�^
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