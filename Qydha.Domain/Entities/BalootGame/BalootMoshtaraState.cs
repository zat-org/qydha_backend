﻿using Stateless;

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
    private readonly StateMachine<MoshtaraState, MoshtaraTrigger> _stateMachine;
    #endregion

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
            return Result.Fail(new()
            {
                Code = ErrorType.InvalidBalootGameAction,
                Message = $"Can't Fire {trigger} On Moshtara Current State {_stateMachine.State}"
            });
        return Result.Ok();
    }
    public Result StartMoshtara()
    {
        return CanFire(MoshtaraTrigger.StartMoshtara)
        .OnSuccess(() =>
        {
            _stateMachine.Fire(MoshtaraTrigger.StartMoshtara);
        });
    }

    public Result EndMoshtara(MoshtaraData moshtaraData)
    {
        return CanFire(MoshtaraTrigger.EndMoshtara)
        .OnSuccess(() =>
        {
            _stateMachine.Fire(MoshtaraTrigger.EndMoshtara);
            MoshtaraData = moshtaraData;
        });
    }
    public Result PauseMoshtara()
    {
        return CanFire(MoshtaraTrigger.PauseMoshtara)
        .OnSuccess(() =>
        {
            _stateMachine.Fire(MoshtaraTrigger.PauseMoshtara);
        });
    }
    public Result ResumeMoshtara()
    {
        return CanFire(MoshtaraTrigger.ResumeMoshtara)
        .OnSuccess(() =>
        {
            _stateMachine.Fire(MoshtaraTrigger.ResumeMoshtara);
        });
    }
    public Result Back()
    {
        return CanFire(MoshtaraTrigger.Back)
        .OnSuccess(() =>
        {
            _stateMachine.Fire(MoshtaraTrigger.Back);
            MoshtaraData = null;
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
                return Result.Fail(new()
                {
                    Code = ErrorType.InvalidBalootGameAction,
                    Message = ex.Message
                });
            }

        }).OnSuccess(() => _stateMachine.Fire(MoshtaraTrigger.AddMashare3));
    }
    public Result AddMashare3(Mashare3 usMashare3, Mashare3 themMashare3)
    {
        return CanFire(MoshtaraTrigger.AddMashare3)
         .OnSuccess(() =>
        {
            try
            {
                MoshtaraData!.AddMashare3(usMashare3, themMashare3);
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(new()
                {
                    Code = ErrorType.InvalidBalootGameAction,
                    Message = ex.Message
                });
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