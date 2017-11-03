using System;
using System.Collections;
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Bauland.Gadgeteer
{
    /// <summary>
    /// Melody is a queue of Notes
    /// </summary>
    public class Melody
    {
        /// <summary>
        /// List of notes
        /// </summary>
        public readonly Queue List;

        /// <summary>Constructs a new instance.</summary>
        public Melody()
        {
            List = new Queue();
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
            if (frequency < 0) throw new ArgumentOutOfRangeException(nameof(frequency), "frequency must be non-negative.");
            if (duration < 1) throw new ArgumentOutOfRangeException(nameof(duration), "duration must be positive.");

            Add(new Tone(frequency), duration);
        }

        /// <summary>Adds a new note to the list to play.</summary>
        /// <param name="tone">The tone of the note.</param>
        /// <param name="duration">The duration of the note.</param>
        public void Add(Tone tone, int duration)
        {
            if (duration < 1) throw new ArgumentOutOfRangeException(nameof(duration), "duration must be positive.");

            Add(new MusicNote(tone, duration));
        }

        /// <summary>Adds an existing note to the list to play.</summary>
        /// <param name="note">The note to add.</param>
        public void Add(MusicNote note)
        {
            List.Enqueue(note);
        }

        /// <summary>Adds notes to the list to play.</summary>
        /// <param name="notes">The list of notes to add to the melody.</param>
        public void Add(params MusicNote[] notes)
        {
            foreach (MusicNote i in notes)
                Add(i);
        }
    }
}
