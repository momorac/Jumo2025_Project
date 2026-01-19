
// Enums for different placement types
using System;

public enum PlaceableType
{
    Facility,
    Tile,
    Decoration
}

public enum FacilityType
{
    JumoHouse = 0,
    Hearth = 1,
    Table = 2
}

public enum TileType
{
    Grass = 0,
    Water = 1,
    Sand = 2
}

public enum DecorationType
{
    Plant = 0,
    Statue = 1,
    Fountain = 2
}


// Base class for all placement types
[Serializable]
public class Placeable
{
    public PlaceableType PlacementType;
}

[Serializable]
public class Facility : Placeable
{
    public FacilityType FacilityType;

    public Facility(FacilityType facilityType)
    {
        PlacementType = PlaceableType.Facility;
        FacilityType = facilityType;
    }
}

[Serializable]
public class Tile : Placeable
{
    public TileType TileType;

    public Tile(TileType tileType)
    {
        PlacementType = PlaceableType.Tile;
        TileType = tileType;
    }
}

[Serializable]
public class Decoration : Placeable
{
    public DecorationType DecorationType;

    public Decoration(DecorationType decorationType)
    {
        PlacementType = PlaceableType.Decoration;
        DecorationType = decorationType;
    }
}


