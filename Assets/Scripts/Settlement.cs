using UnityEngine;

/// <summary>
/// ���_�p�N���X
/// </summary>
[System.Serializable]
public class Settlement {
    public Vector3Int cubeCoordinates;
    public TileType tileType;
    public OwnerType ownerType;          // �v���C���[�̏��Ȃ�

    public Settlement(Vector3Int cubeCoordinates, TileType tileType, OwnerType ownerType) {
        this.cubeCoordinates = cubeCoordinates;
        this.tileType = tileType;
        this.ownerType = ownerType;
    }

    public void CollectResource(TileType tileType) {
        // �I�[�i�[�Ɏ�����t�^

    }
}