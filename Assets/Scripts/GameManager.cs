using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// �^�C���}�b�v���Ǘ��p
/// </summary>
[System.Serializable]
public class TileInfo {
    public Vector3Int axialPos;  // 2D�����W
    public HexTile hexTile;      // �L���[�u���W�͂��̃N���X���ɂ���
}


public class GameManager : MonoBehaviour {
    [SerializeField] private int stageSize;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private List<HexTile> hexTileList = new();         // ���������^�C�����Ǘ�����N���X�̃��X�g
    [SerializeField] private List<HexCorner> hexCornerList = new();     // �אڃ^�C���̃��X�g
    [SerializeField] private List<Settlement> settlementList = new();   // ���_���X�g
    [SerializeField] private HexTileDataSO hexTileDataSO;

    // �f�o�b�O�p
    [SerializeField] private List<TileInfo> tileInfoList = new();    // �z�u���ꂽ���W�Ƃ��̍��W�ɂ��� HexTile �̏��̃��X�g
    [SerializeField] private bool isCatanMapStructure;

    private int minDiceNum = 2;
    private int maxDiceNum = 13;

    // �S�^�C�����i�[����Dictionary�B������ Vector3Int �͔z�u�p��2D���W
    private Dictionary<Vector3Int, HexTile> tileMapDic = new();


    // �^�C����6�̗אڕ����p�̃L���[�u���W�␳�l
    Vector3Int[] directions = new Vector3Int[] {
        new (1, -1, 0),  // �E�� 2��
        new (1, 0, -1),  // �E�� 4��
        new (0, 1, -1),  // ��   6��
        new (-1, 1, 0),  // ���� 8��
        new (-1, 0, 1),  // ���� 10��
        new (0, -1, 1)   // ��   12��
    };

    // �J�^�����
    // �����_�� 24
    // �X���� 14 + 28 = 42�{


    void Start() {
        // �J�^���̌`��̃}�b�v�ɂ��邩����
        if (isCatanMapStructure) {
            CreateCatanMap();
        } else {
            // ���W���v�Z���A�^�C���}�b�v���̂��̈ʒu�Ƀ^�C����z�u
            CreateHexGrid(stageSize);
        }

        // �אڃ^�C���̊Ǘ��p List �쐬
        CreateHexCornerList();
    }

    /// <summary>
    /// �J�^���̌`��̃}�b�v����
    /// </summary>
    private void CreateCatanMap() {
        // �e�s�̃^�C�������`
        int[] rowLengths = { 3, 4, 5, 4, 3 };

        // �}�b�v�̐����J�n
        for (int row = 0; row < rowLengths.Length; row++) {
            int tilesInRow = rowLengths[row];

            for (int col = 0; col < tilesInRow; col++) {
                // �L���[�u���W (q, r, s) ���v�Z
                int q = col;// ������
                int r = row;// �c�����i��������̂���𔽉f�j

                if (row == 1 || row == 2 || row == 3) {
                    q -= 1;
                }

                // �L���[�u���W�Ɋ�Â��ă^�C����z�u
                Vector3Int hexPos = AxialToCube(new Vector3Int(q, r, 0));//new Vector3Int(q, r, s);
                CreateHexTile(hexPos, 0);
            }
        }
    }

    /// <summary>
    /// ���W���v�Z���A�^�C���}�b�v���̂��̈ʒu�Ƀ^�C����z�u
    /// </summary>
    /// <param name="radius"></param>
    private void CreateHexGrid(int radius) {
        // ���W�̕␳�l
        int count = 0;
        for (int q = -radius; q <= radius; q++) {
            for (int r = -radius; r <= radius; r++) {
                // �����W (q, r) ���L���[�u���W�ɕϊ�
                Vector3Int cubePos = AxialToCube(new Vector3Int(q, r, 0));

                // ���W�Ɋ�Â��^�C���𐶐����Ĕz�u
                CreateHexTile(cubePos, count);
            }
            count++;
        }
    }

    /// <summary>
    /// �^�C������
    /// </summary>
    /// <param name="cubePosition"></param>
    /// <param name="count"></param>
    private void CreateHexTile(Vector3Int cubePosition, int count) {
        // �L���[�u���W�������W�ɕϊ�
        Vector3 axialPos = CubeToAxial(cubePosition);
        Vector3Int axialIntPos = new ((int)axialPos.x, (int)axialPos.y, 0);

        // ��U�����_���Ń^�C���̎�ނ�ݒ肵�ă^�C���z�u
        int randomTileIndex = Random.Range(0, hexTileDataSO.hexTileDataList.Count); 
        tilemap.SetTile(axialIntPos, hexTileDataSO.hexTileDataList[randomTileIndex].tile);

        // �^�C���p�̏����쐬
        TileType tileType = hexTileDataSO.hexTileDataList[randomTileIndex].tileType;
        int diceNumber = Random.Range(minDiceNum, maxDiceNum);

        // �^�C���Z�b�g�����̂ŁA�␳�l���g���ăL���[�u���W���č쐬(for ���ŏ��X�ɒl�������̂ŁA�����␳����B�Z�b�g���͖��Ȃ�)
        Vector3Int cubePos = AxialToCube(new(axialIntPos.x, axialIntPos.y - count, 0));

        // �^�C�����p�� HexTile �쐬���� List �ɒǉ�
        HexTile hexTile = new(tileType, diceNumber, cubePos);
        hexTileList.Add(hexTile);

        // �L���[�u���W���L�[�Ƃ��� tileMapDic �ɓo�^
        tileMapDic.Add(cubePos, hexTile);

        // Dictionary �������Ȃ��̂ŁA�������̂� List �ō쐬���ăf�o�b�O�p�ɗ��p����
        TileInfo tileInfo = new() { axialPos = axialIntPos, hexTile = hexTile };
        tileInfoList.Add(tileInfo);
    }

    /// <summary>
    /// 2D�����W���L���[�u���W�ɕϊ�
    /// </summary>
    /// <param name="axialPos"></param>
    /// <returns></returns>
    private Vector3Int AxialToCube(Vector3Int axialPos) {
        int q = axialPos.x; // �����W q -4
        int r = axialPos.y; // �����W r -5
        int x = q;          // �L���[�u���W x �͎����W q �Ɉ�v
        int z = r;          // �L���[�u���W z �͎����W r �Ɉ�v
        int y = -x - z;     // �L���[�u���W�n�� y = -x - z �����藧��
        return new Vector3Int(x, y, z);
    }

    /// <summary>
    /// �L���[�u���W��2D�����W�ɕϊ�
    /// </summary>
    /// <param name="cubePos"></param>
    /// <returns></returns>
    private Vector3 CubeToAxial(Vector3Int cubePos) {
        int q = cubePos.x; // Axial��X��
        int r = cubePos.z; // Axial��Y�� (cubePos.y �ł͂Ȃ� cubePos.z ���g��)
        return new Vector3(q, r, 0); // ���ۂ�2D��Ԃł̍��W�ɕϊ�
    }

    /// <summary>
    /// �אڃ^�C���̊Ǘ��p List �쐬
    /// </summary>
    private void CreateHexCornerList() {
        // �S�^�C���̗אڃ^�C���𑖍����Đݒ�
        foreach (HexTile hexTile in hexTileList) {
            List<HexTile> adjacentTiles = new(FindAdjacentTiles(hexTile.cubePosition));

            // �אڃ^�C���Ǘ��p�N���X�̐����� List �ǉ�
            HexCorner hexCorner = new(hexTile.cubePosition, adjacentTiles, null);
            hexCornerList.Add(hexCorner);
        }
    }

    /// <summary>
    /// �אڃ^�C����T��
    /// �߂�l�� HexCorner �� List<HexTile> �ɓ����
    /// </summary>
    /// <param name="currentTilePos"></param>
    /// <returns></returns>
    public List<HexTile> FindAdjacentTiles(Vector3Int currentTilePos) {
        //Debug.Log(currentTilePos);
        List<HexTile> adjacentTiles = new ();

        foreach (var direction in directions) {
            Vector3Int neighborPos = currentTilePos + direction;
            Debug.Log(neighborPos);

            Debug.Log($"Current: {currentTilePos}, Neighbor: {neighborPos}");

            if (tileMapDic.TryGetValue(neighborPos, out HexTile neighborTile)) {
                // �אڃ^�C����������Βǉ�
                adjacentTiles.Add(neighborTile);
                Debug.Log($"�אڃ^�C��: {neighborTile.cubePosition}, �^�C�v: {neighborTile.tileType}, �_�C�X�ԍ�: {neighborTile.diceNumber}");
            }
        }
        return adjacentTiles;
    }


    /// <summary>
    /// ���_�ǉ�
    /// </summary>
    /// <param name="settlement"></param>
    public void AddSettlement(Settlement settlement) {
        settlementList.Add(settlement);
    }

    /// <summary>
    /// �L���[�u���W���� HexTile ���擾
    /// </summary>
    /// <param name="cubePosition"></param>
    /// <returns></returns>
    private HexTile GetTileAt(Vector3Int cubePosition) {
        return hexTileList.FirstOrDefault(tile => tile.cubePosition == cubePosition);
    }

    /// <summary>
    /// �L���[�u���W���狒�_���擾
    /// </summary>
    /// <param name="cubePosition"></param>
    /// <returns></returns>
    public Settlement GetSettlementAt(Vector3Int cubePosition) {
        return settlementList.Find(settlement => settlement.cubeCoordinates == cubePosition);
    }
}