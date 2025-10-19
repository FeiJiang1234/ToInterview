using ToInterview.API.Patterns.Composite;
using Xunit;

namespace ToInterview.API.Tests
{
    public class CompositeTests
    {
        [Fact]
        public void File_ShouldDisplayCorrectly()
        {
            // Arrange
            var file = new Patterns.Composite.File("test.txt", 1024);

            // Act & Assert
            // 验证可以显示文件而不抛出异常
            file.Display();
            Assert.Equal("test.txt", file.Name);
            Assert.Equal(1024, file.GetSize());
        }

        [Fact]
        public void Folder_ShouldDisplayCorrectly()
        {
            // Arrange
            var folder = new Folder("测试文件夹");

            // Act & Assert
            // 验证可以显示文件夹而不抛出异常
            folder.Display();
            Assert.Equal("测试文件夹", folder.Name);
            Assert.Equal(0, folder.GetSize()); // 空文件夹大小为0
        }

        [Fact]
        public void Folder_ShouldAddAndRemoveComponents()
        {
            // Arrange
            var folder = new Folder("测试文件夹");
            var file1 = new Patterns.Composite.File("file1.txt", 512);
            var file2 = new Patterns.Composite.File("file2.txt", 1024);

            // Act
            folder.Add(file1);
            folder.Add(file2);

            // Assert
            Assert.Equal(2, folder.GetChildren().Count);
            Assert.Equal(1536, folder.GetSize()); // 512 + 1024

            // 删除组件
            folder.Remove(file1);
            Assert.Equal(1, folder.GetChildren().Count);
            Assert.Equal(1024, folder.GetSize());
        }

        [Fact]
        public void Folder_ShouldCalculateTotalSize()
        {
            // Arrange
            var rootFolder = new Folder("根目录");
            var subFolder = new Folder("子文件夹");
            var file1 = new Patterns.Composite.File("file1.txt", 1000);
            var file2 = new Patterns.Composite.File("file2.txt", 2000);
            var file3 = new Patterns.Composite.File("file3.txt", 3000);

            // Act
            rootFolder.Add(file1);
            rootFolder.Add(subFolder);
            subFolder.Add(file2);
            subFolder.Add(file3);

            // Assert
            Assert.Equal(6000, rootFolder.GetSize()); // 1000 + 2000 + 3000
            Assert.Equal(5000, subFolder.GetSize()); // 2000 + 3000
        }

        [Fact]
        public void FileSystemManager_ShouldManageFileSystem()
        {
            // Arrange
            var rootFolder = new Folder("根目录");
            var fileSystemManager = new FileSystemManager(rootFolder);
            var file = new Patterns.Composite.File("test.txt", 1024);

            // Act & Assert
            // 验证可以管理文件系统而不抛出异常
            fileSystemManager.AddComponent(rootFolder, file);
            fileSystemManager.DisplayFileSystem();
            Assert.NotNull(fileSystemManager);
        }

        [Fact]
        public void FileSystemManager_ShouldHandleInvalidOperations()
        {
            // Arrange
            var rootFolder = new Folder("根目录");
            var fileSystemManager = new FileSystemManager(rootFolder);
            var file1 = new Patterns.Composite.File("file1.txt", 1024);
            var file2 = new Patterns.Composite.File("file2.txt", 512);

            // Act & Assert
            // 验证可以处理无效操作而不抛出异常
            fileSystemManager.AddComponent(rootFolder, file1);
            fileSystemManager.AddComponent(file1, file2); // 这应该显示错误消息
            Assert.NotNull(fileSystemManager);
        }

        [Fact]
        public void NestedFolderStructure_ShouldWorkCorrectly()
        {
            // Arrange
            var rootFolder = new Folder("根目录");
            var documentsFolder = new Folder("文档");
            var imagesFolder = new Folder("图片");
            var file1 = new Patterns.Composite.File("doc1.txt", 1000);
            var file2 = new Patterns.Composite.File("image1.jpg", 2000);

            // Act
            rootFolder.Add(documentsFolder);
            rootFolder.Add(imagesFolder);
            documentsFolder.Add(file1);
            imagesFolder.Add(file2);

            // Assert
            Assert.Equal(2, rootFolder.GetChildren().Count);
            Assert.Equal(1, documentsFolder.GetChildren().Count);
            Assert.Equal(1, imagesFolder.GetChildren().Count);
            Assert.Equal(3000, rootFolder.GetSize());
        }

        [Fact]
        public void EmptyFolder_ShouldHaveZeroSize()
        {
            // Arrange
            var folder = new Folder("空文件夹");

            // Act & Assert
            Assert.Equal(0, folder.GetSize());
            Assert.Equal(0, folder.GetChildren().Count);
        }
    }
}
