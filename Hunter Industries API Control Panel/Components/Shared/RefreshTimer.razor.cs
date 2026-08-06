// Copyright © - Unpublished - Toby Hunter
using Microsoft.AspNetCore.Components;

namespace HunterIndustriesAPIControlPanel.Components.Shared
{
    public partial class RefreshTimer : IAsyncDisposable
    {
        [Parameter]
        public int IntervalSeconds { get; set; } = 60;
        [Parameter]
        public EventCallback OnRefresh { get; set; }

        private bool IsRunning = true;

        private CancellationTokenSource? _CancellationTokenSource;

        /// <summary>
        /// Sets up the cancellation token.
        /// </summary>
        protected override void OnInitialized()
        {
            _CancellationTokenSource = new CancellationTokenSource();
            _ = RunTimerAsync(_CancellationTokenSource.Token);
        }

        /// <summary>
        /// Runs the refresh method.
        /// </summary>
        private async Task RunTimerAsync(CancellationToken cancellationToken)
        {
            using PeriodicTimer timer = new(TimeSpan.FromSeconds(IntervalSeconds));

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    if (!await timer.WaitForNextTickAsync(cancellationToken))
                    {
                        break;
                    }

                    if (IsRunning)
                    {
                        await InvokeAsync(async () =>
                        {
                            await OnRefresh.InvokeAsync();
                            StateHasChanged();
                        });
                    }
                }

                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Changes the status of the timer.
        /// </summary>
        private void ToggleTimer()
        {
            IsRunning = !IsRunning;
        }

        /// <summary>
        /// Destroys the timer.
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            if (_CancellationTokenSource != null)
            {
                await _CancellationTokenSource.CancelAsync();
                _CancellationTokenSource.Dispose();
                _CancellationTokenSource = null;
            }

            GC.SuppressFinalize(this);
        }
    }
}
