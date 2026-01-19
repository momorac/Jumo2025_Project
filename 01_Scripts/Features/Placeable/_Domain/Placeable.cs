
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
public abstract class Placeable
{
    public PlaceableType PlacementType;
    public abstract string GetDisplayName();
}

[Serializable]
public class Facility : Placeable
{
    public FacilityType Type;

    public Facility(FacilityType facilityType)
    {
        PlacementType = PlaceableType.Facility;
        Type = facilityType;
    }

    public override string GetDisplayName()
    {
        return Type.ToString();
    }
}

[Serializable]
public class Tile : Placeable
{
    public TileType Type;

    public Tile(TileType tileType)
    {
        PlacementType = PlaceableType.Tile;
        Type = tileType;
    }

    public override string GetDisplayName()
    {
        return Type.ToString();
    }
}

[Serializable]
public class Decoration : Placeable
{
    public DecorationType Type;

    public Decoration(DecorationType decorationType)
    {
        PlacementType = PlaceableType.Decoration;
        Type = decorationType;
    }

    public override string GetDisplayName()
    {
        return Type.ToString();
    }
}


