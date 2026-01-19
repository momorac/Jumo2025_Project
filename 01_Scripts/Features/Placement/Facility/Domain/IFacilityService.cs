public interface IFacilityService
{
    FacilityType Type { get; }
    void Initialize();
    void OnPlaced();
    void OnRemoved();
}
