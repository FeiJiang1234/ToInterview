namespace ToInterview.API.Practice
{
    public interface IMediaPlayer
    {
        void Play(string autioType);
    }

    public class MP3Player : IMediaPlayer
    {
        public void Play(string autioType)
        {
            Console.WriteLine("play mp3");
        }
    }

    public class AudioPlayer : IMediaPlayer
    {
        private MP3Player _mp3Player;

        public AudioPlayer()
        {
            _mp3Player = new MP3Player();
        }

        public void Play(string autioType)
        {
            _mp3Player.Play(autioType);
        }
    }
}
