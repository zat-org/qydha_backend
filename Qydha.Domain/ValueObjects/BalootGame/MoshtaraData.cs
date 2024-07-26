namespace Qydha.Domain.ValueObjects;
public class MoshtaraData
{
    public BalootRecordingMode RecordingMode { get; init; }
    public int UsAbnat { get; init; }
    public int ThemAbnat { get; init; }
    public MoshtaraDetails? AdvancedDetails { get; init; }
    private MoshtaraData() { }
    public MoshtaraData(int usAbnat, int themAbnat)
    {
        RecordingMode = BalootRecordingMode.Regular;
        UsAbnat = usAbnat;
        ThemAbnat = themAbnat;
        AdvancedDetails = null;
    }
    public MoshtaraData(MoshtaraDetails advancedDetails)
    {
        RecordingMode = BalootRecordingMode.Advanced;
        AdvancedDetails = advancedDetails;
        (UsAbnat, ThemAbnat) = AdvancedDetails.CalculateAbnat();
    }
    public Result<MoshtaraData> AddMashare3(int usScore, int themScore)
    {
        if (RecordingMode != BalootRecordingMode.Regular)
            return Result.Fail(
                new InvalidBodyInputError(new Dictionary<string, List<string>>() {
                    {nameof(RecordingMode) ,["يجب ان يكون المشترى مسجل يدويا"] }
                })
            );
        return Result.Ok(new MoshtaraData(UsAbnat + usScore, ThemAbnat + themScore));
    }

    public BalootGameStatistics GetStatistics()
    {
        if (RecordingMode == BalootRecordingMode.Advanced && AdvancedDetails != null)
            return AdvancedDetails.GetStatistics();
        return BalootGameStatistics.Zero();
    }

}

public class MoshtaraDetails
{
    private MoshtaraDetails() { }

    public MoshtaraType Moshtara { get; init; }
    public BalootGameTeam MoshtaraOwner { get; init; }
    public bool IsMoshtaraSucceeded { get; init; }
    public MoshtaraTeamDetails UsData { get; init; } = null!;
    public MoshtaraTeamDetails ThemData { get; init; } = null!;
    private MoshtaraDetails(MoshtaraType moshtara, MoshtaraTeamDetails usData, MoshtaraTeamDetails themData, int usAbnat, int themAbnat, BalootGameTeam? selectedMoshtaraOwner)
    {
        Moshtara = moshtara;
        UsData = usData;
        ThemData = themData;
        (MoshtaraOwner, IsMoshtaraSucceeded) = CalculateMoshtaraOwnerAndResult(usAbnat, themAbnat, selectedMoshtaraOwner);
    }
    public static Result<MoshtaraDetails> CreateMoshtaraDetails(
        MoshtaraType moshtaraType,
        (SunMoshtaraScoresId Us, SunMoshtaraScoresId Them)? sunId,
        (HokmMoshtaraScoresId Us, HokmMoshtaraScoresId Them)? hokmId,
        (int Us, int Them) sra,
        (int Us, int Them) khamsen,
        (int Us, int Them) me2a,
        (int Us, int Them)? baloot,
        (int Us, int Them)? rob3ome2a,
        (int Us, int Them) ekak,
        (int Us, int Them) aklat,
        BalootGameTeam? selectedMoshtaraOwner)
    {
        MoshtaraTeamDetails usData;
        MoshtaraTeamDetails themData;
        switch (moshtaraType)
        {
            case MoshtaraType.Sun:
                if (sunId is null)
                    return Result.Fail(
                        new InvalidBodyInputError(new Dictionary<string, List<string>>() {
                            {nameof(sunId) ,[" في حالة مشترى الصن يجب ان تكون قيمة النتيجة الاساسية صحيحة "] }
                        })
                    );
                if (rob3ome2a is null)
                {
                    return Result.Fail(
                        new InvalidBodyInputError(new Dictionary<string, List<string>>() {
                            {nameof(rob3ome2a) ,["في حالة مشترى الصن يجب ان تكون الربعمائة بقيمة صحيحة"] }
                        }));
                }
                usData = new SunMoshtaraTeamDetails(
                    new Mashare3Sun(sra.Us, khamsen.Us, me2a.Us, rob3ome2a.Value.Us), ekak.Us, aklat.Us, sunId.Value.Us);
                themData = new SunMoshtaraTeamDetails(
                    new Mashare3Sun(sra.Them, khamsen.Them, me2a.Them, rob3ome2a.Value.Them), ekak.Them, aklat.Them, sunId.Value.Them);
                break;
            case MoshtaraType.Hokm:
                if (hokmId is null)
                    return Result.Fail(
                        new InvalidBodyInputError(new Dictionary<string, List<string>>() {
                            {nameof(hokmId) ,[" في حالة مشترى حكم يجب ان تكون قيمة النتيجة الاساسية صحيحة "] }
                        })
                    );
                if (baloot is null)
                    return Result.Fail(
                        new InvalidBodyInputError(new Dictionary<string, List<string>>() {
                            {nameof(baloot) ,[" في حالة مشترى حكم يجب ان يكون البلوت بقيمة صحيحة"] }
                        })
                    );

                usData = new HokmMoshtaraTeamDetails(
                    new Mashare3Hokm(baloot.Value.Us, sra.Us, khamsen.Us, me2a.Us), ekak.Us, aklat.Us, hokmId.Value.Us);
                themData = new HokmMoshtaraTeamDetails(
                    new Mashare3Hokm(baloot.Value.Them, sra.Them, khamsen.Them, me2a.Them), ekak.Them, aklat.Them, hokmId.Value.Them);
                break;
            default:
                throw new ArgumentException("invalid value for moshtara not (sun or hokm)", nameof(moshtaraType));
        }

        int usAbnat, themAbnat;
        (usAbnat, themAbnat) = CalculateAbnat(usData, themData);

        if ((usData.IsScoreDoubled() || usData.IsScoreDoubled() || usAbnat == themAbnat) && selectedMoshtaraOwner is null)
            return Result.Fail(
                new InvalidBodyInputError(new Dictionary<string, List<string>>() {
                    {nameof(selectedMoshtaraOwner) , ["يجب تحديد صاحب المشترى في حالة التعادل او اللعب الدبل"] }
                })
            );
        return Result.Ok(new MoshtaraDetails(moshtaraType, usData, themData, usAbnat, themAbnat, selectedMoshtaraOwner));
    }
    public (BalootGameTeam, bool) CalculateMoshtaraOwnerAndResult(int usAbnat, int themAbnat, BalootGameTeam? selectedMoshtaraOwner)
    {

        if ((UsData.IsScoreKhosaraOrKhosaretKaboot() && !ThemData.IsScoreDoubled()) ||
            (ThemData.IsScoreKhosaraOrKhosaretKaboot() && !UsData.IsScoreDoubled()))
        {
            return (UsData.IsScoreKhosaraOrKhosaretKaboot() ? BalootGameTeam.Us : BalootGameTeam.Them, false);
        }
        else if (ThemData.IsScoreDoubled() || UsData.IsScoreDoubled() || usAbnat == themAbnat)
        {
            if (selectedMoshtaraOwner == null) throw new ArgumentNullException(nameof(selectedMoshtaraOwner));

            if (usAbnat == themAbnat)
                return (selectedMoshtaraOwner.Value, true);
            else
            {
                if (selectedMoshtaraOwner == BalootGameTeam.Us && UsData.IsScoreDoubled())
                    return (BalootGameTeam.Us, true);
                else if (selectedMoshtaraOwner == BalootGameTeam.Them && ThemData.IsScoreDoubled())
                    return (BalootGameTeam.Them, true);
                else
                    return (selectedMoshtaraOwner.Value, false);
            }
        }
        else
        {
            return (usAbnat > themAbnat ? BalootGameTeam.Us : BalootGameTeam.Them, true);
        }
    }

    public (int, int) CalculateAbnat() => CalculateAbnat(UsData, ThemData);
    public static (int, int) CalculateAbnat(MoshtaraTeamDetails usData, MoshtaraTeamDetails themData)
    {
        int team1TotalScore = usData.GetRoundScoreValue();
        int team2TotalScore = themData.GetRoundScoreValue();
        if (usData.IsScoreKhosara() || themData.IsScoreKhosara())
        {
            int mashare3Value;

            if (usData.IsScoreDoubled() || themData.IsScoreDoubled())
                mashare3Value = usData.Mashare3.CalcDoubledValue() + themData.Mashare3.CalcDoubledValue();
            else
                mashare3Value = usData.Mashare3.CalcValue() + themData.Mashare3.CalcValue();

            team1TotalScore += mashare3Value * (usData.IsScoreKhosara() ? 0 : 1);
            team2TotalScore += mashare3Value * (themData.IsScoreKhosara() ? 0 : 1);
            return (team1TotalScore, team2TotalScore);
        }
        else if (usData.IsScoreKaboot() || themData.IsScoreKaboot())
        {
            team1TotalScore += usData.Mashare3.CalcValue() * (usData.IsScoreKaboot() ? 1 : 0);
            team2TotalScore += themData.Mashare3.CalcValue() * (themData.IsScoreKaboot() ? 1 : 0);
            return (team1TotalScore, team2TotalScore);
        }
        else
            return (team1TotalScore + usData.Mashare3.CalcValue(), team2TotalScore + themData.Mashare3.CalcValue());
    }

    public BalootGameStatistics GetStatistics()
    {
        var usStatistics = UsData.GetStatistics(MoshtaraOwner == BalootGameTeam.Us, IsMoshtaraSucceeded);
        var themStatistics = ThemData.GetStatistics(MoshtaraOwner == BalootGameTeam.Them, IsMoshtaraSucceeded);
        return new BalootGameStatistics(usStatistics, themStatistics);
    }

}

public abstract class MoshtaraTeamDetails(Mashare3 mashare3, int ekak, int aklat)
{

    public Mashare3 Mashare3 { get; init; } = mashare3;
    public int Ekak { get; init; } = ekak;
    public int Aklat { get; init; } = aklat;
    public abstract MoshtaraScore GetRoundScore();
    public int GetRoundScoreValue() => GetRoundScore().Value;
    public abstract bool IsScoreKhosara();
    public abstract bool IsScoreDoubled();
    public abstract bool IsScoreKaboot();
    public abstract bool IsScoreKhosaraOrKhosaretKaboot();
    public BalootGameTeamStatistics GetStatistics(bool isMoshtaraOwner, bool isMoshtaraSucceeded)
        => new()
        {
            Ekak = Ekak,
            Aklat = Aklat,
            Sra = Mashare3.Sra,
            Khamsen = Mashare3.Khamsen,
            Me2a = Mashare3.Me2a,
            Rob3ome2a = Mashare3 is Mashare3Sun mashare3Sun ? mashare3Sun.Rob3ome2a : 0,
            Baloot = Mashare3 is Mashare3Hokm mashare3Hokm ? mashare3Hokm.Baloot : 0,
            SunKaboot = Mashare3 is Mashare3Sun && IsScoreKaboot() ? 1 : 0,
            HokmKaboot = Mashare3 is Mashare3Hokm && IsScoreKaboot() ? 1 : 0,
            MoshtaraSunCount = Mashare3 is Mashare3Sun && isMoshtaraOwner ? 1 : 0,
            MoshtaraHokmCount = Mashare3 is Mashare3Hokm && isMoshtaraOwner ? 1 : 0,
            WonMoshtaraCount = isMoshtaraOwner && isMoshtaraSucceeded ? 1 : 0,
            LostMoshtaraCount = isMoshtaraOwner && !isMoshtaraSucceeded ? 1 : 0
        };
}

public class SunMoshtaraTeamDetails(Mashare3Sun mashare3, int ekak, int aklat, SunMoshtaraScoresId roundScoreId)
    : MoshtaraTeamDetails(mashare3, ekak, aklat)
{
    public SunMoshtaraScoresId RoundScoreId { get; init; } = roundScoreId;

    public override MoshtaraScore GetRoundScore() => BalootConstants.SunRoundScores[RoundScoreId];

    public override bool IsScoreDoubled() => RoundScoreId == SunMoshtaraScoresId.DoubleSun;

    public override bool IsScoreKaboot() => RoundScoreId == SunMoshtaraScoresId.Kaboot;

    public override bool IsScoreKhosara() => RoundScoreId == SunMoshtaraScoresId.khosara;

    public override bool IsScoreKhosaraOrKhosaretKaboot() =>
        RoundScoreId == SunMoshtaraScoresId.khosaretKaboot || RoundScoreId == SunMoshtaraScoresId.khosara;
}

public class HokmMoshtaraTeamDetails(Mashare3Hokm mashare3, int ekak, int aklat, HokmMoshtaraScoresId roundScoreId)
    : MoshtaraTeamDetails(mashare3, ekak, aklat)
{
    public HokmMoshtaraScoresId RoundScoreId { get; init; } = roundScoreId;

    public override MoshtaraScore GetRoundScore() => BalootConstants.HokmRoundScores[RoundScoreId];

    public override bool IsScoreDoubled() =>
        RoundScoreId == HokmMoshtaraScoresId.Double || RoundScoreId == HokmMoshtaraScoresId.Three ||
        RoundScoreId == HokmMoshtaraScoresId.Four || RoundScoreId == HokmMoshtaraScoresId.Kahwa;

    public override bool IsScoreKaboot() => RoundScoreId == HokmMoshtaraScoresId.Kaboot;

    public override bool IsScoreKhosara() => RoundScoreId == HokmMoshtaraScoresId.khosara;

    public override bool IsScoreKhosaraOrKhosaretKaboot() =>
        RoundScoreId == HokmMoshtaraScoresId.khosaretKaboot || RoundScoreId == HokmMoshtaraScoresId.khosara;

}

public abstract record Mashare3(int Sra, int Khamsen, int Me2a)
{
    public abstract int CalcValue();
    public abstract int CalcDoubledValue();
}

public record Mashare3Sun(int Sra, int Khamsen, int Me2a, int Rob3ome2a)
    : Mashare3(Sra, Khamsen, Me2a)
{
    public override int CalcDoubledValue() => CalcValue() * 2;

    public override int CalcValue() =>
        Sra * BalootConstants.Mashare3SunValues["Sra"] +
        Khamsen * BalootConstants.Mashare3SunValues["Khamsen"] +
        Me2a * BalootConstants.Mashare3SunValues["Me2a"] +
        Rob3ome2a * BalootConstants.Mashare3SunValues["Rob3ome2a"];
}

public record Mashare3Hokm(int Baloot, int Sra, int Khamsen, int Me2a)
    : Mashare3(Sra, Khamsen, Me2a)
{
    public override int CalcDoubledValue() =>
       (CalcValue() * 2) - (Baloot * BalootConstants.Mashare3HokmValues["Baloot"]);

    public override int CalcValue() =>
        Sra * BalootConstants.Mashare3HokmValues["Sra"] +
        Khamsen * BalootConstants.Mashare3HokmValues["Khamsen"] +
        Me2a * BalootConstants.Mashare3HokmValues["Me2a"] +
        Baloot * BalootConstants.Mashare3HokmValues["Baloot"];
}

public abstract record MoshtaraScore(string DisplayValue, string Name, int Value);

public record SunMoshtaraScore(string DisplayValue, string Name, int Value, SunMoshtaraScoresId[] GoesToIds)
    : MoshtaraScore(DisplayValue, Name, Value);

public record HokmMoshtaraScore(string DisplayValue, string Name, int Value, HokmMoshtaraScoresId[] GoesToIds)
    : MoshtaraScore(DisplayValue, Name, Value);