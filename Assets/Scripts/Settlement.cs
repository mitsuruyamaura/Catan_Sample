using UnityEngine;

[System.Serializable]
public class Settlement {
    public Vector3Int cubeCoordinates;
    public TileType tileType;            // �Ⴆ�΁u���v��u�s�s�v�Ȃ�
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