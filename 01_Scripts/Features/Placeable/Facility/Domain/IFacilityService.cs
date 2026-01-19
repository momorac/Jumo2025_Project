public interface IFacilityService
{
    Placeable Type { get; }
    void Initialize();
    void OnPlaced();
    void OnRemoved();
}
