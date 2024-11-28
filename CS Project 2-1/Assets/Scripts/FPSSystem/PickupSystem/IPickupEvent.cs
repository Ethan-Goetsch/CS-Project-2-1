namespace FPSSystem.PickupSystem
{
    public interface IPickupEvent
    {
        public record OnPickupCollected(Pickup Pickup) : IPickupEvent;
        public record OnPickupDespawned(Pickup Pickup) : IPickupEvent;
    }
}
