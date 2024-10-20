using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="HexTileDataSO", menuName = "Create HexTileDataSO")]
public class HexTileDataSO : ScriptableObject
{
    public List<HexTileData> hexTileDataList = new();
}