using System;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Bauland.Gadgeteer
{
    /// <summary>
    /// Tone which correspond to one frequency
    /// </summary>
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
            if (frequency < 0) throw new ArgumentOutOfRangeException(nameof(frequency), "frequency must be non-negative.");

            Frequency = frequency;
        }
    }
}
