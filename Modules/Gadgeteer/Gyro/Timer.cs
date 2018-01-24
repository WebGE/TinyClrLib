using System;
using System.Threading;
// ReSharper disable MemberCanBePrivate.Global

namespace Bauland.Gadgeteer
{
    /// <summary>
    /// Class to wrap usage of Timer
    /// </summary>
    public class Timer
    {
        private readonly System.Threading.Timer _timer;
        private bool _isRunning;
        private readonly object _syncRunning;

        /// <summary>
        /// Create a timer by specifying Interval and (optional) type
        /// </summary>
        /// <param name="interval">Interval in milliseconds of timer</param>
        /// <param name="behavior">Once or Continuously</param>
        public Timer(TimeSpan interval, BehaviorType behavior = BehaviorType.RunOnce)
        {
            Behavior = behavior;
            Interval = interval;
            _syncRunning = new object();
            _timer = new System.Threading.Timer(TimerCallbackFunc, null, 100, (int)Interval.TotalMilliseconds);
        }

        private void TimerCallbackFunc(object state)
        {
            if (Behavior == BehaviorType.RunOnce)
                lock (_syncRunning)
                {
                    _isRunning = false;
                }
            Tick?.Invoke(this, new EventHandlerArgs());
        }

        /// <summary>
        /// Get or set Interval of Timer in milliseconds
        /// </summary>
        public TimeSpan Interval { get; set; }

        /// <summary>
        /// Check if timer is running
        /// </summary>
        public bool IsRunning
        {
            get
            {
                lock (_syncRunning)
                {
                    return _isRunning;

                }
            }
        }
        /// <summary>
        /// Set behavior of timer
        /// </summary>
        public BehaviorType Behavior { get; set; }

        /// <summary>
        /// Stop the timer
        /// </summary>
        public void Stop()
        {
            lock (_syncRunning)
            {
                _isRunning = !_timer.Change(Timeout.Infinite, 0);
            }
        }

        /// <summary>
        /// Start the timer
        /// </summary>
        public void Start()
        {
            lock (_syncRunning)
            {
                if (Behavior == BehaviorType.RunOnce)
                    _isRunning = _timer.Change(Interval, TimeSpan.Zero);
                if (Behavior == BehaviorType.RunContinuously)
                    _isRunning = _timer.Change(Interval, Interval);
            }
        }

        /// <summary>
        /// Event which fire once or periodically.
        /// </summary>
        public event EventHandler Tick;

        /// <summary>
        /// Behaviors of timer
        /// </summary>
        public enum BehaviorType
        {
            /// <summary>
            /// Run timer only one time
            /// </summary>
            RunOnce,
            /// <summary>
            /// Run timer in continous mode
            /// </summary>
            RunContinuously
        }
    }

    /// <summary>
    /// Event fired when timer is running
    /// </summary>
    /// <param name="sender">timer which send the event</param>
    /// <param name="args">Nothing</param>
    public delegate void EventHandler(object sender, EventHandlerArgs args);

    /// <summary>
    /// 
    /// </summary>
    public class EventHandlerArgs
    {
    }
}
