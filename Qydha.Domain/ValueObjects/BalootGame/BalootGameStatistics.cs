namespace Qydha.Domain.Entities;

public record BalootGameStatistics(BalootGameTeamStatistics UsStatistics, BalootGameTeamStatistics ThemStatistics)
{

    public static BalootGameStatistics operator +(BalootGameStatistics c1, BalootGameStatistics c2)
    {
        return new BalootGameStatistics(c1.UsStatistics + c2.UsStatistics, c1.ThemStatistics + c2.ThemStatistics);
    }
    public static BalootGameStatistics Zero()
    {
        return new BalootGameStatistics(new BalootGameTeamStatistics(), new BalootGameTeamStatistics());
    }
    public BalootGameStatistics AddSakkaResult(BalootGameTeam? winner)
    {
        if (winner == null) return this;
        var newUs = UsStatistics with
        {
            PlayedSakkas = UsStatistics.PlayedSakkas + 1,
            WinnedSakkas = winner == BalootGameTeam.Us ? UsStatistics.WinnedSakkas + 1 : UsStatistics.WinnedSakkas,
            LostSakka = winner != BalootGameTeam.Us ? UsStatistics.LostSakka + 1 : UsStatistics.LostSakka
        };
        var newThem = ThemStatistics with
        {
            PlayedSakkas = ThemStatistics.PlayedSakkas + 1,
            WinnedSakkas = winner == BalootGameTeam.Them ? ThemStatistics.WinnedSakkas + 1 : ThemStatistics.WinnedSakkas,
            LostSakka = winner != BalootGameTeam.Them ? ThemStatistics.LostSakka + 1 : ThemStatistics.LostSakka
        };
        return new(newUs, newThem);
    }
}


public record BalootGameTeamStatistics()
{
    public int PlayedSakkas { get; init; }
    public int WinnedSakkas { get; init; }
    public int LostSakka { get; init; }
    public int MoshtaraSunCount { get; init; }
    public int MoshtaraHokmCount { get; init; }
    public int WonMoshtaraCount { get; init; }
    public int LostMoshtaraCount { get; init; }
    public int Ekak { get; init; }
    public int Aklat { get; init; }
    public int Sra { get; init; }
    public int Khamsen { get; init; }
    public int Me2a { get; init; }
    public int Rob3ome2a { get; init; }
    public int Baloot { get; init; }
    public int SunKaboot { get; init; }
    public int HokmKaboot { get; init; }

    public static BalootGameTeamStatistics operator +(BalootGameTeamStatistics c1, BalootGameTeamStatistics c2)
    {
        return new BalootGameTeamStatistics
        {
            Ekak = c1.Ekak + c2.Ekak,
            Aklat = c1.Aklat + c2.Aklat,
            Sra = c1.Sra + c2.Sra,
            Khamsen = c1.Khamsen + c2.Khamsen,
            Me2a = c1.Me2a + c2.Me2a,
            Rob3ome2a = c1.Rob3ome2a + c2.Rob3ome2a,
            Baloot = c1.Baloot + c2.Baloot,
            SunKaboot = c1.SunKaboot + c2.SunKaboot,
            HokmKaboot = c1.HokmKaboot + c2.HokmKaboot,
            MoshtaraSunCount = c1.MoshtaraSunCount + c2.MoshtaraSunCount,
            MoshtaraHokmCount = c1.MoshtaraHokmCount + c2.MoshtaraHokmCount,
            WonMoshtaraCount = c1.WonMoshtaraCount + c2.WonMoshtaraCount,
            LostMoshtaraCount = c1.LostMoshtaraCount + c2.LostMoshtaraCount,
            PlayedSakkas = c1.PlayedSakkas + c2.PlayedSakkas,
            WinnedSakkas = c1.WinnedSakkas + c2.WinnedSakkas,
            LostSakka = c1.LostSakka + c2.LostSakka,
        };
    }

}