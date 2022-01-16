using System;
using IrrKlang;

namespace Graphics2D
{
    class Sound
    {
        #region Class Constructors
        /// <summary>
        /// Default sound constructor
        /// </summary>
        public Sound() {}
        #endregion

        #region Class Methods
        /// <summary>
        /// Play the sound clip once
        /// </summary>
        public void Play()
        {
            try
            {
                // Creates the sound engine
                ISoundEngine engine = new ISoundEngine();
                engine.Play2D(@"C:\Users\quinn\Downloads\Graphics2D\Graphics2D\sound.wav", false);
            }
            catch (Exception)
            { }
        }
        #endregion
    }
}