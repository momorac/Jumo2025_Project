
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
    JumoHouse = 1,
    Hearth = 2,
    Table = 3
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
    public string displayName;
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


