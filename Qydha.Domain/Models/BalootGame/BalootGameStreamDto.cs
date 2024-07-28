namespace Qydha.Domain.Models;

public record BalootGameStreamDto(
    Guid Id,
    string State,
    string UsName,
    string ThemName,
    int UsGameScore,
    int ThemGameScore,
    int MaxSakkaPerGame,
    BalootGameTeam? Winner,
    List<BalootSakkaStreamDto> Sakkas);


public record BalootSakkaStreamDto(
    int Id,
    string State,
    bool IsMashdoda,
    BalootGameTeam? Winner,
    int UsSakkaScore,
    int ThemSakkaScore,
    List<BalootMoshtaraStreamDto> Moshtaras);

public record BalootMoshtaraStreamDto(
    int Id,
    int UsAbnat,
    int ThemAbnat,
    string State
);