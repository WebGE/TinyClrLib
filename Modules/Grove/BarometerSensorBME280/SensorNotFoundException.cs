using System;
// ReSharper disable UnusedMember.Global

namespace Bauland.Grove
{
    /// <summary>
    /// 
    /// </summary>
    public class SensorNotFoundException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        public SensorNotFoundException()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public SensorNotFoundException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="inner"></param>
        public SensorNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }

    }
}
