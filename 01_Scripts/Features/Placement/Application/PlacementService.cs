public class PlacementService
{
    private PlacementMeta placementMeta;

    public PlacementService(PlacementMeta placementMeta)
    {
        this.placementMeta = placementMeta;
    }

    public PlacementMeta GetMeta() => placementMeta;

    public void UpdateMeta(PlacementMeta newMeta)
    {
        placementMeta = newMeta;
    }
}
