namespace Qydha.Domain.Enums;

[Flags]
public enum NotificationActionType
{
    NoAction = 1,
    GoToURL = 2,
    GoToScreen = 4,
    GoToTab = 8,
    PopUp = 256,
    PopUpWithNoAction = 257,
    PopUpWithGoToURL = 258,
    PopUpWithGoToScreen = 260,
    PopUpWithGoToTab = 264,
}

public enum NotificationVisibility
{
    Public = 1,
    Anonymous = 2,
    Private = 3,
}
public enum NotificationSendingMechanism
{
    Manual = 1,
    Automatic = 2
}