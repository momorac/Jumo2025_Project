
// Enums for different placement types
using System;

public enum PlaceableType
{
    None = 0,
    Facility = 1,
    Tile = 2,
    Decoration = 3
}

public enum FacilityType
{
    None = 0,

    // === 기존 시설 ===
    JumoHouse = 1,
    Table = 2,

    // === 조리 시설 (100번대) ===
    Pot = 100,           // 솥 - 밥, 국
    Cauldron = 101,      // 가마솥 - 반찬, 국밥/찌개
    Brazier = 102,       // 화로 - 전, 요리
    JangdokJar = 103,    // 장독대 - 김치

    // === 자원 시설 (200번대) ===
    Well = 200,          // 우물 - 물 제공
    Stump = 201,         // 그루터기 - 장작 제공
}

public enum TileType
{
    None = 0,
    Grass = 1,
    Water = 2,
    Sand = 3
}

public enum DecorationType
{
    None = 0,
    Plant = 1,
    Statue = 2,
    Fountain = 3
}


// Base class for all placement types
[Serializable]
public class Placeable
{
    public PlaceableType PlaceableType;
}

[Serializable]
public class Facility : Placeable
{
    public FacilityType Type;

    public Facility(FacilityType facilityType)
    {
        PlaceableType = PlaceableType.Facility;
        Type = facilityType;
    }
}

[Serializable]
public class Tile : Placeable
{
    public TileType Type;

    public Tile(TileType tileType)
    {
        PlaceableType = PlaceableType.Tile;
        Type = tileType;
    }
}

[Serializable]
public class Decoration : Placeable
{
    public DecorationType Type;

    public Decoration(DecorationType decorationType)
    {
        PlaceableType = PlaceableType.Decoration;
        Type = decorationType;
    }
}


