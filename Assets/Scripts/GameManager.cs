using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// タイルマップ情報管理用
/// </summary>
[System.Serializable]
public class TileInfo {
    public Vector3Int axialPos;  // 2D軸座標
    public HexTile hexTile;      // キューブ座標はこのクラス内にある
}


public class GameManager : MonoBehaviour {
    [SerializeField] private int stageSize;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private List<HexTile> hexTileList = new();         // 生成したタイルを管理するクラスのリスト
    [SerializeField] private List<HexCorner> hexCornerList = new();     // 隣接タイルのリスト
    [SerializeField] private List<Settlement> settlementList = new();   // 拠点リスト
    [SerializeField] private HexTileDataSO hexTileDataSO;

    // デバッグ用
    [SerializeField] private List<TileInfo> tileInfoList = new();    // 配置された座標とその座標にある HexTile の情報のリスト
    [SerializeField] private bool isCatanMapStructure;

    private int minDiceNum = 2;
    private int maxDiceNum = 13;

    // 全タイルを格納するDictionary。ここの Vector3Int は配置用の2D座標
    private Dictionary<Vector3Int, HexTile> tileMapDic = new();


    // タイルの6つの隣接方向用のキューブ座標補正値
    Vector3Int[] directions = new Vector3Int[] {
        new (1, -1, 0),  // 右上 2時
        new (1, 0, -1),  // 右下 4時
        new (0, 1, -1),  // 下   6時
        new (-1, 1, 0),  // 左下 8時
        new (-1, 0, 1),  // 左上 10時
        new (0, -1, 1)   // 上   12時
    };

    // カタン情報
    // 交差点数 24
    // 街道数 14 + 28 = 42本


    void Start() {
        // カタンの形状のマップにするか判定
        if (isCatanMapStructure) {
            CreateCatanMap();
        } else {
            // 座標を計算し、タイルマップ内のその位置にタイルを配置
            CreateHexGrid(stageSize);
        }

        // 隣接タイルの管理用 List 作成
        CreateHexCornerList();
    }

    /// <summary>
    /// カタンの形状のマップ生成
    /// </summary>
    private void CreateCatanMap() {
        // 各行のタイル数を定義
        int[] rowLengths = { 3, 4, 5, 4, 3 };

        // マップの生成開始
        for (int row = 0; row < rowLengths.Length; row++) {
            int tilesInRow = rowLengths[row];

            for (int col = 0; col < tilesInRow; col++) {
                // キューブ座標 (q, r, s) を計算
                int q = col;// 横方向
                int r = row;// 縦方向（中央からのずれを反映）

                if (row == 1 || row == 2 || row == 3) {
                    q -= 1;
                }

                // キューブ座標に基づいてタイルを配置
                Vector3Int hexPos = AxialToCube(new Vector3Int(q, r, 0));//new Vector3Int(q, r, s);
                CreateHexTile(hexPos, 0);
            }
        }
    }

    /// <summary>
    /// 座標を計算し、タイルマップ内のその位置にタイルを配置
    /// </summary>
    /// <param name="radius"></param>
    private void CreateHexGrid(int radius) {
        // 座標の補正値
        int count = 0;
        for (int q = -radius; q <= radius; q++) {
            for (int r = -radius; r <= radius; r++) {
                // 軸座標 (q, r) をキューブ座標に変換
                Vector3Int cubePos = AxialToCube(new Vector3Int(q, r, 0));

                // 座標に基づきタイルを生成して配置
                CreateHexTile(cubePos, count);
            }
            count++;
        }
    }

    /// <summary>
    /// タイル生成
    /// </summary>
    /// <param name="cubePosition"></param>
    /// <param name="count"></param>
    private void CreateHexTile(Vector3Int cubePosition, int count) {
        // キューブ座標を軸座標に変換
        Vector3 axialPos = CubeToAxial(cubePosition);
        Vector3Int axialIntPos = new ((int)axialPos.x, (int)axialPos.y, 0);

        // 一旦ランダムでタイルの種類を設定してタイル配置
        int randomTileIndex = Random.Range(0, hexTileDataSO.hexTileDataList.Count); 
        tilemap.SetTile(axialIntPos, hexTileDataSO.hexTileDataList[randomTileIndex].tile);

        // タイル用の情報を作成
        TileType tileType = hexTileDataSO.hexTileDataList[randomTileIndex].tileType;
        int diceNumber = Random.Range(minDiceNum, maxDiceNum);

        // タイルセットしたので、補正値を使ってキューブ座標を再作成(for 文で徐々に値がずれるので、それを補正する。セット時は問題なし)
        Vector3Int cubePos = AxialToCube(new(axialIntPos.x, axialIntPos.y - count, 0));

        // タイル情報用の HexTile 作成して List に追加
        HexTile hexTile = new(tileType, diceNumber, cubePos);
        hexTileList.Add(hexTile);

        // キューブ座標をキーとして tileMapDic に登録
        tileMapDic.Add(cubePos, hexTile);

        // Dictionary が見えないので、同じものを List で作成してデバッグ用に利用する
        TileInfo tileInfo = new() { axialPos = axialIntPos, hexTile = hexTile };
        tileInfoList.Add(tileInfo);
    }

    /// <summary>
    /// 2D軸座標をキューブ座標に変換
    /// </summary>
    /// <param name="axialPos"></param>
    /// <returns></returns>
    private Vector3Int AxialToCube(Vector3Int axialPos) {
        int q = axialPos.x; // 軸座標 q -4
        int r = axialPos.y; // 軸座標 r -5
        int x = q;          // キューブ座標 x は軸座標 q に一致
        int z = r;          // キューブ座標 z は軸座標 r に一致
        int y = -x - z;     // キューブ座標系で y = -x - z が成り立つ
        return new Vector3Int(x, y, z);
    }

    /// <summary>
    /// キューブ座標を2D軸座標に変換
    /// </summary>
    /// <param name="cubePos"></param>
    /// <returns></returns>
    private Vector3 CubeToAxial(Vector3Int cubePos) {
        int q = cubePos.x; // AxialのX軸
        int r = cubePos.z; // AxialのY軸 (cubePos.y ではなく cubePos.z を使う)
        return new Vector3(q, r, 0); // 実際の2D空間での座標に変換
    }

    /// <summary>
    /// 隣接タイルの管理用 List 作成
    /// </summary>
    private void CreateHexCornerList() {
        // 全タイルの隣接タイルを走査して設定
        foreach (HexTile hexTile in hexTileList) {
            List<HexTile> adjacentTiles = new(FindAdjacentTiles(hexTile.cubePosition));

            // 隣接タイル管理用クラスの生成と List 追加
            HexCorner hexCorner = new(hexTile.cubePosition, adjacentTiles, null);
            hexCornerList.Add(hexCorner);
        }
    }

    /// <summary>
    /// 隣接タイルを探す
    /// 戻り値を HexCorner の List<HexTile> に入れる
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
                // 隣接タイルが見つかれば追加
                adjacentTiles.Add(neighborTile);
                Debug.Log($"隣接タイル: {neighborTile.cubePosition}, タイプ: {neighborTile.tileType}, ダイス番号: {neighborTile.diceNumber}");
            }
        }
        return adjacentTiles;
    }


    /// <summary>
    /// 拠点追加
    /// </summary>
    /// <param name="settlement"></param>
    public void AddSettlement(Settlement settlement) {
        settlementList.Add(settlement);
    }

    /// <summary>
    /// キューブ座標から HexTile を取得
    /// </summary>
    /// <param name="cubePosition"></param>
    /// <returns></returns>
    private HexTile GetTileAt(Vector3Int cubePosition) {
        return hexTileList.FirstOrDefault(tile => tile.cubePosition == cubePosition);
    }

    /// <summary>
    /// キューブ座標から拠点を取得
    /// </summary>
    /// <param name="cubePosition"></param>
    /// <returns></returns>
    public Settlement GetSettlementAt(Vector3Int cubePosition) {
        return settlementList.Find(settlement => settlement.cubeCoordinates == cubePosition);
    }
}