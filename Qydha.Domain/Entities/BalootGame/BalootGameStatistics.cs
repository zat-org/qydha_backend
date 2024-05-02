namespace Qydha.Domain.Entities;

public class BalootGameStatistics(BalootGameTeamStatistics usStatistics, BalootGameTeamStatistics themStatistics)
{
    public BalootGameTeamStatistics UsStatistics { get; set; } = usStatistics;
    public BalootGameTeamStatistics ThemStatistics { get; set; } = themStatistics;
    public static BalootGameStatistics operator +(BalootGameStatistics c1, BalootGameStatistics c2)
    {
        return new BalootGameStatistics(c1.UsStatistics + c2.UsStatistics, c1.ThemStatistics + c2.ThemStatistics);
    }
    public static BalootGameStatistics Zero()
    {
        return new BalootGameStatistics(new BalootGameTeamStatistics(), new BalootGameTeamStatistics());
    }
}

public class BalootGameTeamStatistics
{
    // public int PlayedSakkas { get; set; }
    // public int WinnedSakkas { get; set; }
    // public int LostSakka { get; set; }
    public int MoshtaraSunCount { get; set; }
    public int MoshtaraHokmCount { get; set; }
    public int WonMoshtaraCount { get; set; }
    public int LostMoshtaraCount { get; set; }
    public int Ekak { get; set; }
    public int Aklat { get; set; }
    public int Sra { get; set; }
    public int Khamsen { get; set; }
    public int Me2a { get; set; }
    public int Rob3ome2a { get; set; }
    public int Baloot { get; set; }
    public int SunKaboot { get; set; }
    public int HokmKaboot { get; set; }

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

        };
    }

}