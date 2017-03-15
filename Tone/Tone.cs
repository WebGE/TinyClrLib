using System;
using System.Collections;
using System.Threading;
using GHIElectronics.TinyCLR.Devices.Pwm;

namespace Module
{
    public class Tunes
    {
        private PwmPin _pwmPin;
        private Queue playlist;
        private Thread _worker;
        private object syncRoot;

        /// <summary>Whether or not there is something being played.</summary>
        public bool IsPlaying
        {
            get
            {
                lock (playlist)
                    return playlist.Count != 0;
            }
        }

        public Tunes(int pin)
        {
            PwmController pwm=PwmController.GetDefault();
            _pwmPin=pwm.OpenPin(pin);
            playlist=new Queue();
            syncRoot=new object();
        }

        /// <summary>Plays the melody.</summary>
        /// <param name="melody">The melody to play.</param>
        public void Play(Melody melody)
        {
            Stop();

            foreach (var i in melody.list)
                lock (syncRoot)
                {
                    playlist.Enqueue(i);
                }

            _worker = new Thread(DoWork);
            _worker.Start();
        }

        /// <summary>Stops playback.</summary>
        public void Stop()
        {
            if (IsPlaying)
            {
                lock (syncRoot)
                    playlist.Clear();

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
            MusicNote note;

            while (true)
            {
                lock (syncRoot)
                {
                    if (playlist.Count == 0)
                        break;

                    note = (MusicNote)playlist.Dequeue();
                }

                if (Math.Abs(note.Tone.Frequency) > 0.01)
                {
                    _pwmPin.Controller.SetDesiredFrequency((int) note.Tone.Frequency);
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

    /// <summary>Represents a list of notes to play in sequence.</summary>
    public class Melody
    {
        internal Queue list;

        /// <summary>Constructs a new instance.</summary>
        public Melody()
        {
            list = new Queue();
        }

        /// <summary>Constructs a new instance.</summary>
        /// <param name="notes">The list of notes to add to the melody.</param>
        public Melody(params MusicNote[] notes)
            : this()
        {
            foreach (MusicNote i in notes)
                Add(i);
        }

        /// <summary>Adds a new note to the list to play.</summary>
        /// <param name="frequency">The frequency of the note.</param>
        /// <param name="duration">The duration of the note in milliseconds.</param>
        public void Add(int frequency, int duration)
        {
            if (frequency < 0) throw new ArgumentOutOfRangeException("frequency", "frequency must be non-negative.");
            if (duration < 1) throw new ArgumentOutOfRangeException("duration", "duration must be positive.");

            Add(new Tone(frequency), duration);
        }

        /// <summary>Adds a new note to the list to play.</summary>
        /// <param name="tone">The tone of the note.</param>
        /// <param name="duration">The duration of the note.</param>
        public void Add(Tone tone, int duration)
        {
            if (duration < 1) throw new ArgumentOutOfRangeException("duration", "duration must be positive.");

            Add(new MusicNote(tone, duration));
        }

        /// <summary>Adds an existing note to the list to play.</summary>
        /// <param name="note">The note to add.</param>
        public void Add(MusicNote note)
        {
            list.Enqueue(note);
        }

        /// <summary>Adds notes to the list to play.</summary>
        /// <param name="notes">The list of notes to add to the melody.</param>
        public void Add(params MusicNote[] notes)
        {
            foreach (MusicNote i in notes)
                Add(i);
        }
    }


    public class MusicNote
    {
        public Tone Tone { get; set; }
        public int Duration { get; set; }

        public MusicNote(Tone tone, int duration)
        {
            if(duration<1)
                throw new ArgumentException("Duration can't be negative","duration");
            Tone = tone;
            Duration = duration;
        }
    }

    /// <summary>Class that holds and manages notes that can be played.</summary>
    public class Tone
    {

        /// <summary>A rest note</summary>
        public static readonly Tone Rest = new Tone(0.0);

        /// <summary>C in the 2nd octave.</summary>
        public static readonly Tone C2 = new Tone(65.41);

        /// <summary>D in the 2nd octave.</summary>
        public static readonly Tone D2 = new Tone(73.42);

        /// <summary>E in the 2nd octave.</summary>
        public static readonly Tone E2 = new Tone(82.41);

        /// <summary>F in the 2nd octave.</summary>
        public static readonly Tone F2 = new Tone(87.31);

        /// <summary>G in the 2nd octave.</summary>
        public static readonly Tone G2 = new Tone(98.00);

        /// <summary>A in the 2nd octave.</summary>
        public static readonly Tone A2 = new Tone(110.00);

        /// <summary>B in the 2nd octave.</summary>
        public static readonly Tone B2 = new Tone(123.47);

        /// <summary>C in the 3rd octave.</summary>
        public static readonly Tone C3 = new Tone(130.81);

        /// <summary>D in the 3rd octave.</summary>
        public static readonly Tone D3 = new Tone(146.83);

        /// <summary>E in the 3rd octave.</summary>
        public static readonly Tone E3 = new Tone(164.81);

        /// <summary>F in the 3rd octave.</summary>
        public static readonly Tone F3 = new Tone(174.61);

        /// <summary>G in the 3rd octave.</summary>
        public static readonly Tone G3 = new Tone(196.00);

        /// <summary>A in the 3rd octave.</summary>
        public static readonly Tone A3 = new Tone(220.00);

        /// <summary>B in the 3rd octave.</summary>
        public static readonly Tone B3 = new Tone(246.94);

        /// <summary>C in the 4th octave.</summary>
        public static readonly Tone C4 = new Tone(261.626);

        /// <summary>D in the 4th octave.</summary>
        public static readonly Tone D4 = new Tone(293.665);

        /// <summary>E in the 4th octave.</summary>
        public static readonly Tone E4 = new Tone(329.628);

        /// <summary>F in the 4th octave.</summary>
        public static readonly Tone F4 = new Tone(349.228);

        /// <summary>G in the 4th octave.</summary>
        public static readonly Tone G4 = new Tone(391.995);

        /// <summary>A in the 4th octave.</summary>
        public static readonly Tone A4 = new Tone(440);

        /// <summary>B in the 4th octave.</summary>
        public static readonly Tone B4 = new Tone(493.883);

        /// <summary>C in the 5th octave.</summary>
        public static readonly Tone C5 = new Tone(523.251);

        /// <summary>D in the 5th octave.</summary>
        public static readonly Tone D5 = new Tone(587.33);

        /// <summary>E in the 5th octave.</summary>
        public static readonly Tone E5 = new Tone(659.25);

        /// <summary>F in the 5th octave.</summary>
        public static readonly Tone F5 = new Tone(698.46);

        /// <summary>G in the 5th octave.</summary>
        public static readonly Tone G5 = new Tone(783.99);

        /// <summary>A in the 5th octave.</summary>
        public static readonly Tone A5 = new Tone(880.00);

        /// <summary>B in the 5th octave.</summary>
        public static readonly Tone B5 = new Tone(987.77);

        /// <summary>C in the 6th octave.</summary>
        public static readonly Tone C6 = new Tone(1046.50);

        /// <summary>Frequency of the note in hertz</summary>
        public double Frequency { get; set; }

        /// <summary>Constructs a new instance.</summary>
        /// <param name="frequency">The frequency of the tone.</param>
        public Tone(double frequency)
        {
            if (frequency < 0) throw new ArgumentOutOfRangeException("frequency", "frequency must be non-negative.");

            Frequency = frequency;
        }
    }

}
