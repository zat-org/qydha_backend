namespace Qydha.API.Models;

public class GetPopupDto
{
    public string ActionPath { get; set; } = null!;
    public PopupActionType ActionType { get; set; }
    public bool Show { get; set; }
    public string? ImageUrl { get; set; } 
}
