using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HexCorner
{
    public Vector3Int cubeCoordinates;  // �L���[�u���W�Ŋp�̈ʒu���Ǘ�
    public List<HexTile> adjacentTileList = new();  // �ڂ��Ă���^�C��
    public Settlement settlement;  // ���_���

    public HexCorner(Vector3Int cubeCoordinates, List<HexTile> adjacentTileList, Settlement settlement) {
        this.cubeCoordinates = cubeCoordinates;
        this.adjacentTileList = new(adjacentTileList);
        this.settlement = settlement;
    }

    // �������胁�\�b�h
    public void CheckResources(int diceRoll) {
        foreach (var tile in adjacentTileList) {
            if (tile.diceNumber == diceRoll) {
                // �����l������
                settlement.CollectResource(tile.tileType);
            }
        }
    }
}