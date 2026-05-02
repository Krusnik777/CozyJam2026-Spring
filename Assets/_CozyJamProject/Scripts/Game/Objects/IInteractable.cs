namespace CozySpringJam.Game.Objects
{
    public interface IInteractable
    {
        public bool IsAvailableForInteraction { get; }

        public void Interact();
    }
}
