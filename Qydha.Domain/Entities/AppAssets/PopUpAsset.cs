namespace Qydha.Domain.Entities;

public class PopUpAsset
{
    public FileData? Image { get; set; } = null;
    public string ActionPath { get; set; } = string.Empty;
    public PopupActionType ActionType { get; set; } = PopupActionType.NoAction;
    public bool Show { get; set; } = false;
}
