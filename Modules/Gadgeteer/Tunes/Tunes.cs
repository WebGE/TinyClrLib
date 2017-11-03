using System;
using System.Collections;
using System.Threading;
using GHIElectronics.TinyCLR.Devices.Pwm;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace Bauland.Gadgeteer
{
    /// <summary>
    /// Wrapper class for Tunes Gadgeteer Module
    /// </summary>
    public class Tunes
    {
        private readonly PwmPin _pwmPin;
        private readonly Queue _playlist;
        private Thread _worker;
        private readonly object _syncRoot;

        /// <summary>Whether or not there is something being played.</summary>
        public bool IsPlaying
        {
            get
            {
                lock (_playlist)
                    return _playlist.Count != 0;
            }
        }

        /// <summary>
        /// Constructor of Tunes
        /// </summary>
        /// <param name="controller">string of controller (must be a P Socket)</param>
        /// <param name="pin">Pin number (generally pin 9 of P Socket)</param>
        public Tunes(string controller, int pin)
        {
            PwmController ctl = PwmController.FromId(controller);
            _pwmPin = ctl.OpenPin(pin);

            _playlist = new Queue();
            _syncRoot = new object();
        }

        /// <summary>Plays the melody.</summary>
        /// <param name="melody">The melody to play.</param>
        public void Play(Melody melody)
        {
            Stop();

            foreach (var i in melody.List)
                lock (_syncRoot)
                {
                    _playlist.Enqueue(i);
                }

            _worker = new Thread(DoWork);
            _worker.Start();
        }

        /// <summary>Stops playback.</summary>
        public void Stop()
        {
            if (IsPlaying)
            {
                lock (_syncRoot)
                    _playlist.Clear();

                _worker.Join(250);

                if (_worker != null && _worker.IsAlive)
                    _worker.Abort();
            }
            _pwmPin.Controller.SetDesiredFrequency(100.0);
            _pwmPin.SetActiveDutyCyclePercentage(0.0001);
            _pwmPin.Stop();
        }

        private void DoWork()
        {
            while (true)
            {
                MusicNote note;
                lock (_syncRoot)
                {
                    if (_playlist.Count == 0)
                        break;

                    note = (MusicNote)_playlist.Dequeue();
                }

                if (Math.Abs(note.Tone.Frequency) > 0.01)
                {
                    _pwmPin.Controller.SetDesiredFrequency((int)note.Tone.Frequency);
                    _pwmPin.SetActiveDutyCyclePercentage(0.5);
                    _pwmPin.Start();
                }
                else
                {
                    _pwmPin.Stop();
                }

                Thread.Sleep(note.Duration);
            }

            _pwmPin.Controller.SetDesiredFrequency(100);
            _pwmPin.SetActiveDutyCyclePercentage(0.0001);
        }
    }
}
