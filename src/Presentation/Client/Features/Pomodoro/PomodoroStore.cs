﻿using Fluxor;
using System.Timers;
using Medical.Client.Features.Database.Repositories;
using Medical.Client.Features.Notifications;

namespace Medical.Client.Features.Pomodoro
{
    public record PomodoroTickAction();
    public record PomodoroInitializeTimerAction();
    public record PomodoroLoadDurationAction();
    public record PomodoroSaveDurationAction(int Duration);
    public record PomodoroSetDurationAction(int Duration);
    public record PomodoroSetInitializedAction();
    public record PomodoroStartTimerAction();
    public record PomodoroStopTimerAction();
    public record PomodoroSetRunningAction(bool Running);
    public record PomodoroSetFinishedAction();
    public record PomodoroResetAction();

    public record PomodoroState
    {
        public bool Initialized { get; init; }
        public bool Running { get; init; }
        public bool Finished { get; init; }
        public DateTime? StartTime { get; init; }
        public TimeSpan Remaining { get; init; }
        public TimeSpan Elapsed { get; init; }
        public TimeSpan Banked { get; init; }
        public int DefaultDurationInMinutes { get; init; }
    }

    public class PomodoroFeature : Feature<PomodoroState>
    {
        public override string GetName() => "Pomodoro";

        protected override PomodoroState GetInitialState()
        {
            int defaultduration = 25;
            return new PomodoroState
            {
                Initialized = false,
                Running = false,
                Finished = false,
                StartTime = null,
                Remaining = TimeSpan.FromMinutes(defaultduration),
                Elapsed = TimeSpan.Zero,
                Banked = TimeSpan.Zero,
                DefaultDurationInMinutes = defaultduration
            };
        }
    }

    public static class PomodoroReducers
    {
        [ReducerMethod]
        public static PomodoroState OnSetDuration(PomodoroState state, PomodoroSetDurationAction action)
        {
            return state with
            {
                DefaultDurationInMinutes = action.Duration,
                Remaining = TimeSpan.FromMinutes(action.Duration)
            };
        }

        [ReducerMethod(typeof(PomodoroSetInitializedAction))]
        public static PomodoroState OnSetInitialized(PomodoroState state)
        {
            return state with
            {
                Initialized = true
            };
        }

        [ReducerMethod(typeof(PomodoroTickAction))]
        public static PomodoroState OnPomodoroTick(PomodoroState state)
        {
            var newElapsed = DateTime.Now - state.StartTime.GetValueOrDefault() + state.Banked;
            var newRemaining = TimeSpan.FromMinutes(state.DefaultDurationInMinutes) - newElapsed + TimeSpan.FromSeconds(1);
            return state with
            {
                Remaining = newRemaining,
                Elapsed = newElapsed
            };
        }

        [ReducerMethod(typeof(PomodoroStopTimerAction))]
        public static PomodoroState OnStopTimer(PomodoroState state) 
        {
            return state with
            {
                Banked = state.Elapsed
            };
        }

        [ReducerMethod]
        public static PomodoroState OnSetRunning(PomodoroState state, PomodoroSetRunningAction action)
        {
            return state with
            {
                Running = action.Running,
                StartTime = DateTime.Now
            };
        }

        [ReducerMethod(typeof(PomodoroSetFinishedAction))]
        public static PomodoroState OnSetFinished(PomodoroState state)
        {
            return state with
            {
                Running = false,
                Finished = true
            };
        }

        [ReducerMethod]
        public static PomodoroState OnReset(PomodoroState state, PomodoroResetAction action)
        {
            return state with
            {
                Remaining = TimeSpan.FromMinutes(state.DefaultDurationInMinutes),
                Elapsed = TimeSpan.Zero,
                Banked = TimeSpan.Zero,
                StartTime = null,
                Running = false,
                Finished = false
            };
        }
    }

    public class PomodoroEffects
    {
        private const string APP_SETTING_POMODORO_DURATION = "PomodoroDuration";
        private readonly PomodoroTimerService _timerService;
        private readonly IState<PomodoroState> _state;
        private readonly ApplicationSettingRepository _appSettingRepo;

        public PomodoroEffects(PomodoroTimerService timerService, IState<PomodoroState> state, ApplicationSettingRepository appSettingRepo)
        {
            _timerService = timerService;
            _state = state;
            _appSettingRepo = appSettingRepo;
        }

        [EffectMethod(typeof(PomodoroInitializeTimerAction))]
        public async Task OnTimerInitialize(IDispatcher dispatcher)
        {
            // bail if already initialized
            if (_state.Value.Initialized) return;

            await Task.Yield();
            var timer = _timerService.Timer;
            timer.Elapsed += (object source, ElapsedEventArgs e) => dispatcher.Dispatch(new PomodoroTickAction());
            dispatcher.Dispatch(new PomodoroSetInitializedAction());
        }

        [EffectMethod(typeof(PomodoroLoadDurationAction))]
        public async Task OnLoadDuration(IDispatcher dispatcher)
        {
            // bail if already initialized
            if (_state.Value.Initialized) return;

            var appSetting = await _appSettingRepo.GetOrAdd(APP_SETTING_POMODORO_DURATION, defaultValue: "25");
            dispatcher.Dispatch(new PomodoroSetDurationAction(int.Parse(appSetting.Value)));
        }

        [EffectMethod]
        public async Task OnSaveDuration(PomodoroSaveDurationAction action, IDispatcher dispatcher)
        {
            await _appSettingRepo.Update(APP_SETTING_POMODORO_DURATION, action.Duration.ToString());
            dispatcher.Dispatch(new PomodoroSetDurationAction(action.Duration));
        }

        [EffectMethod(typeof(PomodoroStartTimerAction))]
        public async Task OnTimerStart(IDispatcher dispatcher)
        {
            await Task.Yield();
            _timerService.Timer.Start();
            dispatcher.Dispatch(new PomodoroSetRunningAction(true));
        }

        [EffectMethod(typeof(PomodoroStopTimerAction))]
        public async Task OnTimerStop(IDispatcher dispatcher)
        {
            await Task.Yield();
            _timerService.Timer.Stop();
            dispatcher.Dispatch(new PomodoroSetRunningAction(false));
        }

        [EffectMethod(typeof(PomodoroTickAction))]
        public async Task OnTimerTick(IDispatcher dispatcher)
        {
            await Task.Yield();

            if (_state.Value.Remaining < TimeSpan.FromSeconds(1))
            {
                _timerService.Timer.Stop();
                dispatcher.Dispatch(new PomodoroSetFinishedAction());
                dispatcher.Dispatch(new SnackbarShowSuccessAction("Pomodoro Finished. Time for a break!"));
            }
        }

        [EffectMethod(typeof(PomodoroResetAction))]
        public async Task OnReset(IDispatcher dispatcher)
        {
            await Task.Yield();
            _timerService.Timer.Stop();
        }
    }
}
