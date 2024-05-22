namespace Qydha.Domain.Entities;

public class MoshtaraData
{
    public BalootRecordingMode RecordingMode { get; set; }
    public int UsAbnat { get; set; }
    public int ThemAbnat { get; set; }
    public MoshtaraDetails? AdvancedDetails { get; set; }


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
        CalculateAbnat();

    }
    public void CalculateAbnat()
    {
        if (RecordingMode == BalootRecordingMode.Advanced && AdvancedDetails is not null)
            (UsAbnat, ThemAbnat) = AdvancedDetails.CalculateAbnat();
    }

    public Result AddMashare3(int usScore, int themScore)
    {
        if (RecordingMode != BalootRecordingMode.Regular)
        {
            var err = new InvalidBodyInputError();
            err.ValidationErrors.Add(nameof(RecordingMode), ["يجب ان يكون المشترى مسجل يدويا"]);
            return Result.Fail(err);
        }
        UsAbnat += usScore;
        ThemAbnat += themScore;
        return Result.Ok();
    }
    public Result AddMashare3(Mashare3 usMashare3, Mashare3 themMashare3, BalootGameTeam? selectedMoshtaraOwner)
    {
        return Result.Ok().OnSuccess(() =>
        {
            if (RecordingMode != BalootRecordingMode.Advanced || AdvancedDetails == null)
            {
                var err = new InvalidBodyInputError();
                err.ValidationErrors.Add(nameof(RecordingMode), ["يجب ان يكون المشترى مسجل ياستخدام التسجيل المتقدم"]);
                return Result.Fail(err);
            }
            return Result.Ok();
        })
        .OnSuccess(() => AdvancedDetails!.UsData.Mashare3.AddMashare3(usMashare3))
        .OnSuccess(() => AdvancedDetails!.ThemData.Mashare3.AddMashare3(themMashare3))
        .OnSuccess(() =>
        {
            CalculateAbnat();
            return AdvancedDetails!.CalculateMoshtaraOwnerAndResult(selectedMoshtaraOwner);
        })
        .OnSuccess((tuple) =>
        {
            (AdvancedDetails!.MoshtaraOwner, AdvancedDetails.IsMoshtaraSucceeded) = tuple;
            return Result.Ok();
        });
    }
}
public class MoshtaraDetails
{
    public MoshtaraType Moshtara { get; set; }
    public BalootGameTeam MoshtaraOwner { get; set; }
    public bool IsMoshtaraSucceeded { get; set; }
    public MoshtaraTeamDetails UsData { get; set; } = null!;
    public MoshtaraTeamDetails ThemData { get; set; } = null!;

    private MoshtaraDetails() { }

    public static Result<MoshtaraDetails> CreateMoshtaraDetails(MoshtaraType moshtaraType, (SunMoshtaraScoresId, SunMoshtaraScoresId)? sunId, (HokmMoshtaraScoresId, HokmMoshtaraScoresId)? hokmId, (int, int) sra, (int, int) khamsen, (int, int) me2a, (int, int)? baloot, (int, int)? rob3ome2a, (int, int) ekak, (int, int) aklat, BalootGameTeam? selectedMoshtaraOwner)
    {
        var moshtaraDetails = new MoshtaraDetails { Moshtara = moshtaraType };
        switch (moshtaraType)
        {
            case MoshtaraType.Sun:
                if (sunId is null)
                {
                    var err = new InvalidBodyInputError();
                    err.ValidationErrors.Add(nameof(sunId),
                    [" في حالة مشترى الصن يجب ان تكون قيمة النتيجة الاساسية صحيحة "]);
                    return Result.Fail(err);
                }
                if (rob3ome2a is null)
                {
                    var err = new InvalidBodyInputError();
                    err.ValidationErrors.Add(nameof(sunId),
                    [" في حالة مشترى الصن يجب ان تكون الربعمائة بقيمة صحيحة"]);
                    return Result.Fail(err);
                }
                moshtaraDetails.UsData = new SunMoshtaraTeamDetails(new Mashare3Sun(sra.Item1, khamsen.Item1, me2a.Item1, rob3ome2a.Value.Item1), ekak.Item1, aklat.Item1, sunId.Value.Item1);
                moshtaraDetails.ThemData = new SunMoshtaraTeamDetails(new Mashare3Sun(sra.Item2, khamsen.Item2, me2a.Item2, rob3ome2a.Value.Item2), ekak.Item2, aklat.Item2, sunId.Value.Item2);
                break;
            case MoshtaraType.Hokm:
                if (hokmId is null)
                {
                    var err = new InvalidBodyInputError();
                    err.ValidationErrors.Add(nameof(hokmId),
                    [" في حالة مشترى حكم يجب ان تكون قيمة النتيجة الاساسية صحيحة "]);
                    return Result.Fail(err);
                }
                if (baloot is null)
                {
                    var err = new InvalidBodyInputError();
                    err.ValidationErrors.Add(nameof(baloot),
                    [" في حالة مشترى حكم يجب ان يكون البلوت بقيمة صحيحة"]);
                    return Result.Fail(err);
                }

                moshtaraDetails.UsData = new HokmMoshtaraTeamDetails(new Mashare3Hokm(baloot.Value.Item1, sra.Item1, khamsen.Item1, me2a.Item1), ekak.Item1, aklat.Item1, hokmId.Value.Item1);
                moshtaraDetails.ThemData = new HokmMoshtaraTeamDetails(new Mashare3Hokm(baloot.Value.Item2, sra.Item2, khamsen.Item2, me2a.Item2), ekak.Item2, aklat.Item2, hokmId.Value.Item2);
                break;
        }

        return moshtaraDetails.CalculateMoshtaraOwnerAndResult(selectedMoshtaraOwner)
        .OnSuccess((tuple) =>
        {
            (moshtaraDetails.MoshtaraOwner, moshtaraDetails.IsMoshtaraSucceeded) = tuple;
            return Result.Ok(moshtaraDetails);
        });
    }
    public Result<(BalootGameTeam, bool)> CalculateMoshtaraOwnerAndResult(BalootGameTeam? selectedMoshtaraOwner)
    {
        int usAbnat, themAbnat;
        (usAbnat, themAbnat) = CalculateAbnat();
        if ((UsData.IsScoreKhosaraOrKhosaretKaboot() && !ThemData.IsScoreDoubled()) ||
            (ThemData.IsScoreKhosaraOrKhosaretKaboot() && !UsData.IsScoreDoubled()))
        {
            return Result.Ok((UsData.IsScoreKhosaraOrKhosaretKaboot() ? BalootGameTeam.Us : BalootGameTeam.Them, false));
        }
        else if (ThemData.IsScoreDoubled() || UsData.IsScoreDoubled() || usAbnat == themAbnat)
        {
            if (selectedMoshtaraOwner is null)
            {
                var err = new InvalidBodyInputError();
                err.ValidationErrors.Add(nameof(selectedMoshtaraOwner),
                    ["يجب تحديد صاحب المشترى في حالة التعادل او اللعب الدبل"]);
                return Result.Fail(err);
            }
            if (usAbnat == themAbnat)
                return Result.Ok((selectedMoshtaraOwner.Value, true));
            else
            {
                if (selectedMoshtaraOwner == BalootGameTeam.Us && UsData.IsScoreDoubled())
                    return Result.Ok((BalootGameTeam.Us, true));
                else if (selectedMoshtaraOwner == BalootGameTeam.Them && ThemData.IsScoreDoubled())
                    return Result.Ok((BalootGameTeam.Them, true));
                else
                    return Result.Ok((selectedMoshtaraOwner.Value, false));
            }
        }
        else
        {
            return (usAbnat > themAbnat ? BalootGameTeam.Us : BalootGameTeam.Them, true);
        }
    }
    public (int, int) CalculateAbnat()
    {
        int team1TotalScore = UsData.GetRoundScoreValue();
        int team2TotalScore = ThemData.GetRoundScoreValue();
        if (UsData.IsScoreKhosara() || ThemData.IsScoreKhosara())
        {
            int mashare3Value;

            if (UsData.IsScoreDoubled() || ThemData.IsScoreDoubled())
                mashare3Value = UsData.Mashare3.CalcDoubledValue() + ThemData.Mashare3.CalcDoubledValue();
            else
                mashare3Value = UsData.Mashare3.CalcValue() + ThemData.Mashare3.CalcValue();

            team1TotalScore += mashare3Value * (UsData.IsScoreKhosara() ? 0 : 1);
            team2TotalScore += mashare3Value * (ThemData.IsScoreKhosara() ? 0 : 1);
            return (team1TotalScore, team2TotalScore);
        }
        else if (UsData.IsScoreKaboot() || ThemData.IsScoreKaboot())
        {
            team1TotalScore += UsData.Mashare3.CalcValue() * (UsData.IsScoreKaboot() ? 1 : 0);
            team2TotalScore += ThemData.Mashare3.CalcValue() * (ThemData.IsScoreKaboot() ? 1 : 0);
            return (team1TotalScore, team2TotalScore);
        }
        else
            return (team1TotalScore + UsData.Mashare3.CalcValue(), team2TotalScore + ThemData.Mashare3.CalcValue());
    }
}
public abstract class MoshtaraTeamDetails
{
    protected MoshtaraTeamDetails() { }
    public MoshtaraTeamDetails(Mashare3 mashare3, int ekak, int aklat)
    {
        Mashare3 = mashare3;
        Ekak = ekak;
        Aklat = aklat;
    }

    public Mashare3 Mashare3 { get; set; } = null!;
    public int Ekak { get; set; }
    public int Aklat { get; set; }

    public abstract MoshtaraScore GetRoundScore();
    public int GetRoundScoreValue() => GetRoundScore().Value;
    public abstract bool IsScoreKhosara();
    public abstract bool IsScoreDoubled();
    public abstract bool IsScoreKaboot();
    public abstract bool IsScoreKhosaraOrKhosaretKaboot();

}
public class SunMoshtaraTeamDetails : MoshtaraTeamDetails
{
    private SunMoshtaraTeamDetails() : base() { }
    public SunMoshtaraTeamDetails(Mashare3Sun mashare3, int ekak, int aklat, SunMoshtaraScoresId scoreId) : base(mashare3, ekak, aklat)
    {
        RoundScoreId = scoreId;
    }
    public SunMoshtaraScoresId RoundScoreId { get; set; }
    public override MoshtaraScore GetRoundScore() => BalootConstants.SunRoundScores[RoundScoreId];

    public override bool IsScoreDoubled() => RoundScoreId == SunMoshtaraScoresId.DoubleSun;

    public override bool IsScoreKaboot() => RoundScoreId == SunMoshtaraScoresId.Kaboot;

    public override bool IsScoreKhosara() => RoundScoreId == SunMoshtaraScoresId.khosara;

    public override bool IsScoreKhosaraOrKhosaretKaboot() =>
        RoundScoreId == SunMoshtaraScoresId.khosaretKaboot || RoundScoreId == SunMoshtaraScoresId.khosara;


}
public class HokmMoshtaraTeamDetails :
        MoshtaraTeamDetails
{
    private HokmMoshtaraTeamDetails() : base() { }
    public HokmMoshtaraTeamDetails(Mashare3Hokm mashare3, int ekak, int aklat, HokmMoshtaraScoresId scoreId) : base(mashare3, ekak, aklat)
    {
        RoundScoreId = scoreId;
    }
    public HokmMoshtaraScoresId RoundScoreId { get; set; }
    public override MoshtaraScore GetRoundScore() => BalootConstants.HokmRoundScores[RoundScoreId];
    public override bool IsScoreDoubled() =>
        RoundScoreId == HokmMoshtaraScoresId.Double || RoundScoreId == HokmMoshtaraScoresId.Three ||
        RoundScoreId == HokmMoshtaraScoresId.Four || RoundScoreId == HokmMoshtaraScoresId.Kahwa;

    public override bool IsScoreKaboot() => RoundScoreId == HokmMoshtaraScoresId.Kaboot;

    public override bool IsScoreKhosara() => RoundScoreId == HokmMoshtaraScoresId.khosara;

    public override bool IsScoreKhosaraOrKhosaretKaboot() =>
        RoundScoreId == HokmMoshtaraScoresId.khosaretKaboot || RoundScoreId == HokmMoshtaraScoresId.khosara;

}

public abstract class Mashare3
{
    protected Mashare3() { }
    public Mashare3(int sra, int khamsen, int me2a)
    {
        Sra = sra;
        Khamsen = khamsen;
        Me2a = me2a;
    }
    public int Sra { get; set; }
    public int Khamsen { get; set; }
    public int Me2a { get; set; }
    public abstract int CalcValue();
    public abstract int CalcDoubledValue();
    public abstract Result AddMashare3(Mashare3 mashare3);
}
public class Mashare3Sun : Mashare3
{
    private Mashare3Sun() : base() { }
    public Mashare3Sun(int sra, int khamsen, int me2a, int rob3ome2a) : base(sra, khamsen, me2a)
    {
        Rob3ome2a = rob3ome2a;
    }

    public int Rob3ome2a { get; set; }

    public override int CalcDoubledValue() =>
       CalcValue() * 2;

    public override int CalcValue() =>
        Sra * BalootConstants.Mashare3SunValues["Sra"] +
        Khamsen * BalootConstants.Mashare3SunValues["Khamsen"] +
        Me2a * BalootConstants.Mashare3SunValues["Me2a"] +
        Rob3ome2a * BalootConstants.Mashare3SunValues["Rob3ome2a"];

    public override Result AddMashare3(Mashare3 mashare3)
    {
        if (mashare3 is Mashare3Sun mashare3Sun)
        {
            Sra += mashare3Sun.Sra;
            Khamsen += mashare3Sun.Khamsen;
            Me2a += mashare3Sun.Me2a;
            Rob3ome2a += mashare3Sun.Rob3ome2a;
            return Result.Ok();
        }
        else
        {
            var err = new InvalidBodyInputError();
            err.ValidationErrors.Add(nameof(mashare3), ["يجب ان تكون المشاريع من نوع صن"]);
            return Result.Fail(err);
        }
    }
}
public class Mashare3Hokm : Mashare3
{
    private Mashare3Hokm() : base() { }
    public Mashare3Hokm(int baloot, int sra, int khamsen, int me2a) : base(sra, khamsen, me2a)
    {
        Baloot = baloot;
    }
    public int Baloot { get; set; }

    public override int CalcDoubledValue() =>
       (CalcValue() * 2) - (Baloot * BalootConstants.Mashare3HokmValues["Baloot"]);

    public override int CalcValue() =>
        Sra * BalootConstants.Mashare3HokmValues["Sra"] +
        Khamsen * BalootConstants.Mashare3HokmValues["Khamsen"] +
        Me2a * BalootConstants.Mashare3HokmValues["Me2a"] +
        Baloot * BalootConstants.Mashare3HokmValues["Baloot"];

    public override Result AddMashare3(Mashare3 mashare3)
    {
        if (mashare3 is Mashare3Hokm mashare3Sun)
        {
            Sra += mashare3Sun.Sra;
            Khamsen += mashare3Sun.Khamsen;
            Me2a += mashare3Sun.Me2a;
            Baloot += mashare3Sun.Baloot;
            return Result.Ok();
        }
        else
        {
            var err = new InvalidBodyInputError();
            err.ValidationErrors.Add(nameof(mashare3), ["يجب ان تكون المشاريع من نوع حكم"]);
            return Result.Fail(err);
        }
    }
}
public abstract class MoshtaraScore
{
    public string DisplayValue { get; set; } = null!;
    public string Name { get; set; } = null!;
    public int Value { get; set; }
    public override bool Equals(object? obj)
    {
        if (obj is null || obj is not MoshtaraScore otherRoundScore)
            return false;
        return Name == otherRoundScore.Name && Value == otherRoundScore.Value;
    }
    public override int GetHashCode() =>
        Name.GetHashCode() ^ Value.GetHashCode();
}
public class SunMoshtaraScore : MoshtaraScore
{
    public SunMoshtaraScoresId[] GoesToIds { get; set; } = null!;
}

public class HokmMoshtaraScore : MoshtaraScore
{
    public HokmMoshtaraScoresId[] GoesToIds { get; set; } = null!;
}
