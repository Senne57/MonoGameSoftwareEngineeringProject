using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;

namespace MonoGameProject.Core
{
    /// <summary>
    /// MusicManager - Verantwoordelijk voor ALLE muziek in het spel
    /// Volgt SOLID: Single Responsibility Principle
    /// </summary>
    public class MusicManager
    {
        private static MusicManager _instance;
        private Dictionary<string, Song> _songs;
        private Song _currentSong;
        private float _volume = 0.5f; // Default volume (0.0 - 1.0)

        private MusicManager()
        {
            _songs = new Dictionary<string, Song>();
        }

        // Singleton pattern - één MusicManager voor hele game
        public static MusicManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new MusicManager();
                return _instance;
            }
        }

        /// <summary>
        /// Laad een muziek bestand en geef het een naam
        /// </summary>
        public void LoadMusic(ContentManager content, string name, string assetPath)
        {
            if (!_songs.ContainsKey(name))
            {
                Song song = content.Load<Song>(assetPath);
                _songs.Add(name, song);
            }
        }

        /// <summary>
        /// Speel muziek af met optionele loop
        /// </summary>
        public void Play(string name, bool repeat = true)
        {
            if (_songs.ContainsKey(name))
            {
                Song song = _songs[name];

                // Stop huidige muziek als het een andere song is
                if (_currentSong != song)
                {
                    MediaPlayer.Stop();
                    _currentSong = song;
                    MediaPlayer.IsRepeating = repeat;
                    MediaPlayer.Volume = _volume;
                    MediaPlayer.Play(_currentSong);
                }
            }
        }

        /// <summary>
        /// Stop de huidige muziek
        /// </summary>
        public void Stop()
        {
            MediaPlayer.Stop();
            _currentSong = null;
        }

        /// <summary>
        /// Pause de huidige muziek
        /// </summary>
        public void Pause()
        {
            if (MediaPlayer.State == MediaState.Playing)
                MediaPlayer.Pause();
        }

        /// <summary>
        /// Resume de muziek na pause
        /// </summary>
        public void Resume()
        {
            if (MediaPlayer.State == MediaState.Paused)
                MediaPlayer.Resume();
        }

        /// <summary>
        /// Zet volume (0.0 = stil, 1.0 = max)
        /// </summary>
        public void SetVolume(float volume)
        {
            _volume = MathHelper.Clamp(volume, 0f, 1f);
            MediaPlayer.Volume = _volume;
        }

        /// <summary>
        /// Fade out effect (geleidelijk zachter)
        /// </summary>
        public void FadeOut(float fadeSpeed = 0.02f)
        {
            if (_volume > 0)
            {
                _volume -= fadeSpeed;
                if (_volume < 0) _volume = 0;
                MediaPlayer.Volume = _volume;
            }
            else
            {
                Stop();
            }
        }

        /// <summary>
        /// Check of muziek speelt
        /// </summary>
        public bool IsPlaying => MediaPlayer.State == MediaState.Playing;

        /// <summary>
        /// Huidige volume
        /// </summary>
        public float Volume => _volume;
    }
}