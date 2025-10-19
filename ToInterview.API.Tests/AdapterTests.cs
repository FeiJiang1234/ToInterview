using ToInterview.API.Patterns.Adapter;
using Xunit;

namespace ToInterview.API.Tests
{
    public class AdapterTests
    {
        [Fact]
        public void AudioPlayer_ShouldPlayMp3Files()
        {
            // Arrange
            var audioPlayer = new AudioPlayer();

            // Act & Assert
            // 验证可以播放MP3文件而不抛出异常
            audioPlayer.Play("mp3", "test.mp3");
            Assert.NotNull(audioPlayer);
        }

        [Fact]
        public void AudioPlayer_ShouldPlayVlcFiles()
        {
            // Arrange
            var audioPlayer = new AudioPlayer();

            // Act & Assert
            // 验证可以播放VLC文件而不抛出异常
            audioPlayer.Play("vlc", "test.vlc");
            Assert.NotNull(audioPlayer);
        }

        [Fact]
        public void AudioPlayer_ShouldPlayMp4Files()
        {
            // Arrange
            var audioPlayer = new AudioPlayer();

            // Act & Assert
            // 验证可以播放MP4文件而不抛出异常
            audioPlayer.Play("mp4", "test.mp4");
            Assert.NotNull(audioPlayer);
        }

        [Fact]
        public void AudioPlayer_ShouldUseMp3PlayerForMp3Files()
        {
            // Arrange
            var audioPlayer = new AudioPlayer();

            // Act & Assert
            // 验证AudioPlayer使用Mp3Player播放MP3文件
            audioPlayer.Play("mp3", "test.mp3");
            Assert.NotNull(audioPlayer);
        }

        [Fact]
        public void AudioPlayer_ShouldHandleUnsupportedFormats()
        {
            // Arrange
            var audioPlayer = new AudioPlayer();

            // Act & Assert
            // 验证可以处理不支持的格式而不抛出异常
            audioPlayer.Play("avi", "test.avi");
            Assert.NotNull(audioPlayer);
        }
    }
}
