
// Enums for different placement types
using System;

public enum PlacementType
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
public class Placement
{
    public PlacementType PlacementType;
}

[Serializable]
public class Facility : Placement
{
    public FacilityType FacilityType;

    public Facility(FacilityType facilityType)
    {
        PlacementType = PlacementType.Facility;
        FacilityType = facilityType;
    }
}

[Serializable]
public class Tile : Placement
{
    public TileType TileType;

    public Tile(TileType tileType)
    {
        PlacementType = PlacementType.Tile;
        TileType = tileType;
    }
}

[Serializable]
public class Decoration : Placement
{
    public DecorationType DecorationType;

    public Decoration(DecorationType decorationType)
    {
        PlacementType = PlacementType.Decoration;
        DecorationType = decorationType;
    }
}


