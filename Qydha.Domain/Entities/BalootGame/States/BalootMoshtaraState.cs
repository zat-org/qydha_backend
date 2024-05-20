using Stateless;

namespace Qydha.Domain.Entities;

public class BalootMoshtaraState
{
    #region  Moshtara state , triggers enums 
    public enum MoshtaraState
    {
        Created,
        Running,
        Paused,
        Ended
    }
    public enum MoshtaraTrigger
    {
        StartMoshtara,
        EndMoshtara,
        PauseMoshtara,
        ResumeMoshtara,
        Back,
        AddMashare3
    }
    #endregion

    #region data
    public int UsScore { get => MoshtaraData?.UsAbnat ?? 0; }
    public int ThemScore { get => MoshtaraData?.ThemAbnat ?? 0; }
    public MoshtaraData? MoshtaraData { get; set; }
    public MoshtaraState State => _stateMachine.State;
    public DateTimeOffset StartedAt { get; set; }
    public DateTimeOffset? EndedAt { get; set; } = null;
    public List<(DateTimeOffset StartAt, DateTimeOffset? EndAt)> PausingIntervals { get; set; } = [];

    public TimeSpan MoshtaraInterval
    {
        get
        {
            if (!_stateMachine.IsInState(MoshtaraState.Ended) || EndedAt == null) return TimeSpan.Zero;
            return EndedAt.Value - StartedAt - PausingIntervals.Aggregate(TimeSpan.Zero,
                (total, interval) => total + (interval.Item1 - interval.Item2!.Value));
        }
    }
    private readonly StateMachine<MoshtaraState, MoshtaraTrigger> _stateMachine;
    #endregion


    public BalootGameStatistics GetStatistics()
    {
        var usStatistics = new BalootGameTeamStatistics();
        var themStatistics = new BalootGameTeamStatistics();

        if (State == MoshtaraState.Ended && MoshtaraData != null &&
                MoshtaraData.RecordingMode == BalootRecordingMode.Advanced && MoshtaraData.AdvancedDetails != null)
        {
            usStatistics.Ekak += MoshtaraData.AdvancedDetails.UsData.Ekak;
            themStatistics.Ekak += MoshtaraData.AdvancedDetails.ThemData.Ekak;

            usStatistics.Aklat += MoshtaraData.AdvancedDetails.UsData.Aklat;
            themStatistics.Aklat += MoshtaraData.AdvancedDetails.ThemData.Aklat;

            usStatistics.Sra += MoshtaraData.AdvancedDetails.UsData.Mashare3.Sra;
            themStatistics.Sra += MoshtaraData.AdvancedDetails.ThemData.Mashare3.Sra;

            usStatistics.Khamsen += MoshtaraData.AdvancedDetails.UsData.Mashare3.Khamsen;
            themStatistics.Khamsen += MoshtaraData.AdvancedDetails.ThemData.Mashare3.Khamsen;

            usStatistics.Me2a += MoshtaraData.AdvancedDetails.UsData.Mashare3.Me2a;
            themStatistics.Me2a += MoshtaraData.AdvancedDetails.ThemData.Mashare3.Me2a;

            if (MoshtaraData.AdvancedDetails.Moshtara == MoshtaraType.Sun)
            {
                usStatistics.Rob3ome2a += (MoshtaraData.AdvancedDetails.UsData.Mashare3 as Mashare3Sun)!.Rob3ome2a;
                themStatistics.Rob3ome2a += (MoshtaraData.AdvancedDetails.ThemData.Mashare3 as Mashare3Sun)!.Rob3ome2a;
                usStatistics.SunKaboot += MoshtaraData.AdvancedDetails.UsData.IsScoreKaboot() ? 1 : 0;
                themStatistics.SunKaboot += MoshtaraData.AdvancedDetails.ThemData.IsScoreKaboot() ? 1 : 0;

                if (MoshtaraData.AdvancedDetails.MoshtaraOwner == BalootGameTeam.Us)
                {
                    usStatistics.MoshtaraSunCount += 1;
                    if (MoshtaraData.AdvancedDetails.IsMoshtaraSucceeded) usStatistics.WonMoshtaraCount += 1;
                    else usStatistics.LostMoshtaraCount += 1;
                }
                else
                {
                    themStatistics.MoshtaraSunCount += 1;
                    if (MoshtaraData.AdvancedDetails.IsMoshtaraSucceeded) themStatistics.WonMoshtaraCount += 1;
                    else themStatistics.LostMoshtaraCount += 1;
                }

            }
            else
            {
                usStatistics.Baloot += (MoshtaraData.AdvancedDetails.UsData.Mashare3 as Mashare3Hokm)!.Baloot;
                themStatistics.Baloot += (MoshtaraData.AdvancedDetails.ThemData.Mashare3 as Mashare3Hokm)!.Baloot;
                usStatistics.HokmKaboot += MoshtaraData.AdvancedDetails.UsData.IsScoreKaboot() ? 1 : 0;
                themStatistics.HokmKaboot += MoshtaraData.AdvancedDetails.ThemData.IsScoreKaboot() ? 1 : 0;

                if (MoshtaraData.AdvancedDetails.MoshtaraOwner == BalootGameTeam.Us)
                {
                    usStatistics.MoshtaraHokmCount += 1;
                    if (MoshtaraData.AdvancedDetails.IsMoshtaraSucceeded) usStatistics.WonMoshtaraCount += 1;
                    else usStatistics.LostMoshtaraCount += 1;
                }
                else
                {
                    themStatistics.MoshtaraHokmCount += 1;
                    if (MoshtaraData.AdvancedDetails.IsMoshtaraSucceeded) themStatistics.WonMoshtaraCount += 1;
                    else themStatistics.LostMoshtaraCount += 1;
                }
            }
        }
        return new BalootGameStatistics(usStatistics, themStatistics);

    }

    #region  ctor
    public BalootMoshtaraState()
    {
        _stateMachine = new StateMachine<MoshtaraState, MoshtaraTrigger>(MoshtaraState.Created);
        ConfigureStateMachine();
    }

    [JsonConstructor]
    private BalootMoshtaraState(string state)
    {
        var memberState = (MoshtaraState)Enum.Parse(typeof(MoshtaraState), state);
        _stateMachine = new StateMachine<MoshtaraState, MoshtaraTrigger>(memberState);
        ConfigureStateMachine();
    }
    private void ConfigureStateMachine()
    {
        _stateMachine.Configure(MoshtaraState.Created)
            .Permit(MoshtaraTrigger.StartMoshtara, MoshtaraState.Running);

        _stateMachine.Configure(MoshtaraState.Running)
            .Permit(MoshtaraTrigger.PauseMoshtara, MoshtaraState.Paused)
            .Permit(MoshtaraTrigger.EndMoshtara, MoshtaraState.Ended);

        _stateMachine.Configure(MoshtaraState.Paused)
            .Permit(MoshtaraTrigger.ResumeMoshtara, MoshtaraState.Running);

        _stateMachine.Configure(MoshtaraState.Ended)
            .PermitReentry(MoshtaraTrigger.AddMashare3)
            .Permit(MoshtaraTrigger.Back, MoshtaraState.Running);
    }
    #endregion

    #region event handler

    public Result CanFire(MoshtaraTrigger trigger)
    {
        if (!_stateMachine.CanFire(trigger))
            return Result.Fail(new InvalidBalootGameActionError($"Can't Fire {trigger} On Moshtara Current State {_stateMachine.State}"));
        return Result.Ok();
    }
    public Result StartMoshtara(DateTimeOffset triggeredAt)
    {
        return CanFire(MoshtaraTrigger.StartMoshtara)
        .OnSuccess(() =>
        {
            StartedAt = triggeredAt;
            _stateMachine.Fire(MoshtaraTrigger.StartMoshtara);
        });
    }

    public Result EndMoshtara(MoshtaraData moshtaraData, DateTimeOffset triggeredAt)
    {
        return CanFire(MoshtaraTrigger.EndMoshtara)
        .OnSuccess(() =>
        {
            MoshtaraData = moshtaraData;
            EndedAt = triggeredAt;
            _stateMachine.Fire(MoshtaraTrigger.EndMoshtara);
        });
    }
    public Result PauseMoshtara(DateTimeOffset triggeredAt)
    {
        return CanFire(MoshtaraTrigger.PauseMoshtara)
        .OnSuccess(() =>
        {
            PausingIntervals.Add((triggeredAt, null));
            _stateMachine.Fire(MoshtaraTrigger.PauseMoshtara);
        });
    }
    public Result ResumeMoshtara(DateTimeOffset triggeredAt)
    {
        return CanFire(MoshtaraTrigger.ResumeMoshtara)
        .OnSuccess(() =>
        {
            var pauseInterval = PausingIntervals.Last();
            PausingIntervals.RemoveAt(PausingIntervals.Count - 1);
            pauseInterval.EndAt = triggeredAt;
            PausingIntervals.Add(pauseInterval);
            _stateMachine.Fire(MoshtaraTrigger.ResumeMoshtara);
        });
    }
    public Result Back()
    {
        return CanFire(MoshtaraTrigger.Back)
        .OnSuccess(() =>
        {
            MoshtaraData = null;
            EndedAt = null;
            _stateMachine.Fire(MoshtaraTrigger.Back);
        });
    }
    public Result AddMashare3(int usScore, int themScore)
    {
        return CanFire(MoshtaraTrigger.AddMashare3)
        .OnSuccess(() =>
        {
            try
            {
                MoshtaraData!.AddMashare3(usScore, themScore);
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(new InvalidBalootGameActionError(ex.Message));
            }

        }).OnSuccess(() => _stateMachine.Fire(MoshtaraTrigger.AddMashare3));
    }
    public Result AddMashare3(Mashare3 usMashare3, Mashare3 themMashare3, BalootGameTeam? selectedMoshtaraOwner)
    {
        return CanFire(MoshtaraTrigger.AddMashare3)
         .OnSuccess(() =>
        {
            try
            {
                MoshtaraData!.AddMashare3(usMashare3, themMashare3, selectedMoshtaraOwner);
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(new InvalidBalootGameActionError(ex.Message));
            }
        })
        .OnSuccess(() => _stateMachine.Fire(MoshtaraTrigger.AddMashare3));
    }
    #endregion

    public string ToJson()
    {
        return JsonConvert.SerializeObject(this, BalootConstants.balootEventsSerializationSettings);
    }
}