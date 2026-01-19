
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
    public FacilityType FacilityType;

    public Facility(FacilityType facilityType)
    {
        PlacementType = PlaceableType.Facility;
        FacilityType = facilityType;
    }

    public override string GetDisplayName()
    {
        return FacilityType.ToString();
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

    public override string GetDisplayName()
    {
        return TileType.ToString();
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

    public override string GetDisplayName()
    {
        return DecorationType.ToString();
    }
}


