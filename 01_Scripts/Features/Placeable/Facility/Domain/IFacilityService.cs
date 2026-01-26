public interface IFacilityService
{
    Placeable Type { get; }
    void Initialize(Placeable placeable);
    void OnPlaced();
    void OnRemoved();
}
