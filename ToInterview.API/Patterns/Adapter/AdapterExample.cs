namespace ToInterview.API.Patterns.Adapter;

public interface IMediaPlayer
{
    void Play(string audioType, string fileName);
}

public class Mp3Player : IMediaPlayer
{
    public void Play(string audioType, string fileName)
    {
        if (audioType.Equals("mp3", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine($"播放MP3文件: {fileName}");
        }
        else
        {
            Console.WriteLine($"MP3播放器不支持 {audioType} 格式");
        }
    }
}

public class AdvancedMediaPlayer
{
    public void PlayVlc(string fileName)
    {
        Console.WriteLine($"播放VLC文件: {fileName}");
    }

    public void PlayMp4(string fileName)
    {
        Console.WriteLine($"播放MP4文件: {fileName}");
    }
}

public class MediaAdapter : IMediaPlayer
{
    private AdvancedMediaPlayer _advancedPlayer;

    public MediaAdapter(string audioType)
    {
        _advancedPlayer = new AdvancedMediaPlayer();
    }

    public void Play(string audioType, string fileName)
    {
        if (audioType.Equals("vlc", StringComparison.OrdinalIgnoreCase))
        {
            _advancedPlayer.PlayVlc(fileName);
        }
        else if (audioType.Equals("mp4", StringComparison.OrdinalIgnoreCase))
        {
            _advancedPlayer.PlayMp4(fileName);
        }
    }
}

public class AudioPlayer : IMediaPlayer
{
    private MediaAdapter _mediaAdapter;
    private Mp3Player _mp3Player;

    public AudioPlayer()
    {
        _mp3Player = new Mp3Player();
    }

    public void Play(string audioType, string fileName)
    {
        // 使用Mp3Player播放MP3格式
        if (audioType.Equals("mp3", StringComparison.OrdinalIgnoreCase))
        {
            _mp3Player.Play(audioType, fileName);
        }
        // 使用适配器支持其他格式
        else if (audioType.Equals("vlc", StringComparison.OrdinalIgnoreCase) || 
                 audioType.Equals("mp4", StringComparison.OrdinalIgnoreCase))
        {
            _mediaAdapter = new MediaAdapter(audioType);
            _mediaAdapter.Play(audioType, fileName);
        }
        else
        {
            Console.WriteLine($"不支持的音频格式: {audioType}");
        }
    }
}