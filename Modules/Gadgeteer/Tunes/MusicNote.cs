using System;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Bauland.Gadgeteer
{
    /// <summary>
    /// Note of music: it's a tone and a duration
    /// </summary>
    public class MusicNote
    {
        /// <summary>
        /// Access of tone of Note
        /// </summary>
        public Tone Tone { get; set; }

        /// <summary>
        /// Access of duration of Note
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        /// Constructor of MusicNote
        /// </summary>
        /// <param name="tone">Tone of Note</param>
        /// <param name="duration">Duration of Note</param>
        public MusicNote(Tone tone, int duration)
        {
            if (duration < 1)
                throw new ArgumentException("Duration can't be negative", nameof(duration));
            Tone = tone;
            Duration = duration;
        }
    }
}
