public interface IFacilityService
{
    Placement Type { get; }
    void Initialize();
    void OnPlaced();
    void OnRemoved();
}
