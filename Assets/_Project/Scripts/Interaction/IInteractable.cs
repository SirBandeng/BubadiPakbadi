namespace HorrorSchool.Interaction
{
    // Interface mendefinisikan kontrak perilaku tanpa detail implementasi
    public interface IInteractable
    {
        string PromptText { get; }
        bool CanInteract { get; }
        void Interact(Player.PlayerController player);
        void ShowPrompt(bool isVisible);
    }
}