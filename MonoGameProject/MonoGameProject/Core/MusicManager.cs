using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;

namespace MonoGameProject.Core
{
    /// <summary>
    /// MusicManager - Single Responsibility Principle
    /// Deze klasse heeft 1 verantwoordelijkheid: muziek management
    /// Doet GEEN gameplay logic, collision detection of rendering
    /// Scenes hoeven niet te weten hoe MediaPlayer werkt
    /// </summary>
    public class MusicManager
    {
        // Singleton instance
        private static MusicManager _instance;
        private Dictionary<string, Song> _songs;
        private Song _currentSong;
        private float _volume = 0.5f;

        private MusicManager()
        {
            _songs = new Dictionary<string, Song>();
        }

        public static MusicManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new MusicManager();
                return _instance;
            }
        }

        public void LoadMusic(ContentManager content, string name, string assetPath)
        {
            if (!_songs.ContainsKey(name))
            {
                Song song = content.Load<Song>(assetPath);
                _songs.Add(name, song);
            }
        }

        // SRP: Play method doet alleen muziek afspelen, geen side effects
        public void Play(string name, bool repeat = true)
        {
            if (_songs.ContainsKey(name))
            {
                Song song = _songs[name];

                // Only switch if different song
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

        public void Stop()
        {
            MediaPlayer.Stop();
            _currentSong = null;
        }

        public void Pause()
        {
            if (MediaPlayer.State == MediaState.Playing)
                MediaPlayer.Pause();
        }

        public void Resume()
        {
            if (MediaPlayer.State == MediaState.Paused)
                MediaPlayer.Resume();
        }

        public void SetVolume(float volume)
        {
            _volume = MathHelper.Clamp(volume, 0f, 1f);
            MediaPlayer.Volume = _volume;
        }

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

        public bool IsPlaying => MediaPlayer.State == MediaState.Playing;
        public float Volume => _volume;
    }
}