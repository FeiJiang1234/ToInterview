using ToInterview.API.Patterns.Iterator;
using Xunit;

namespace ToInterview.API.Tests
{
    public class IteratorTests
    {
        [Fact]
        public void Book_ShouldCreateCorrectly()
        {
            // Arrange & Act
            var book = new Book("测试书籍", "测试作者", 2023);

            // Assert
            Assert.Equal("测试书籍", book.Title);
            Assert.Equal("测试作者", book.Author);
            Assert.Equal(2023, book.Year);
        }

        [Fact]
        public void Book_ToString_ShouldReturnCorrectFormat()
        {
            // Arrange
            var book = new Book("测试书籍", "测试作者", 2023);

            // Act
            var result = book.ToString();

            // Assert
            Assert.Equal("测试书籍 by 测试作者 (2023)", result);
        }

        [Fact]
        public void BookCollection_ShouldAddAndRemoveBooks()
        {
            // Arrange
            var collection = new BookCollection();
            var book1 = new Book("书1", "作者1", 2020);
            var book2 = new Book("书2", "作者2", 2021);

            // Act
            collection.AddBook(book1);
            collection.AddBook(book2);

            // Assert
            Assert.Equal(2, collection.Count);

            // Remove
            collection.RemoveBook(book1);
            Assert.Equal(1, collection.Count);
        }

        [Fact]
        public void BookIterator_ShouldIterateThroughBooks()
        {
            // Arrange
            var collection = new BookCollection();
            var book1 = new Book("书1", "作者1", 2020);
            var book2 = new Book("书2", "作者2", 2021);
            collection.AddBook(book1);
            collection.AddBook(book2);

            var iterator = collection.GetIterator();
            var books = new List<Book>();

            // Act
            while (iterator.HasNext())
            {
                books.Add(iterator.Next());
            }

            // Assert
            Assert.Equal(2, books.Count);
            Assert.Equal(book1, books[0]);
            Assert.Equal(book2, books[1]);
        }

        [Fact]
        public void BookIterator_ShouldResetCorrectly()
        {
            // Arrange
            var collection = new BookCollection();
            var book = new Book("测试书", "测试作者", 2023);
            collection.AddBook(book);

            var iterator = collection.GetIterator();

            // Act - 第一次迭代
            Assert.True(iterator.HasNext());
            var firstBook = iterator.Next();
            Assert.False(iterator.HasNext());

            // Reset
            iterator.Reset();

            // Act - 第二次迭代
            Assert.True(iterator.HasNext());
            var secondBook = iterator.Next();
            Assert.False(iterator.HasNext());

            // Assert
            Assert.Equal(firstBook, secondBook);
        }

        [Fact]
        public void BookIterator_Next_ShouldThrowExceptionWhenNoMoreElements()
        {
            // Arrange
            var collection = new BookCollection();
            var iterator = collection.GetIterator();

            // Act & Assert
            Assert.False(iterator.HasNext());
            Assert.Throws<InvalidOperationException>(() => iterator.Next());
        }

        [Fact]
        public void Library_ShouldAddBooks()
        {
            // Arrange
            var library = new Library();

            // Act
            library.AddBook("测试书", "测试作者", 2023);

            // Assert
            Assert.Equal(1, library.GetTotalBooks());
        }

        [Fact]
        public void Library_ShouldDisplayAllBooks()
        {
            // Arrange
            var library = new Library();
            library.AddBook("书1", "作者1", 2020);
            library.AddBook("书2", "作者2", 2021);

            // Act & Assert
            // 验证可以显示所有书籍而不抛出异常
            library.DisplayAllBooks();
            Assert.Equal(2, library.GetTotalBooks());
        }

        [Fact]
        public void Library_ShouldSearchBooksByAuthor()
        {
            // Arrange
            var library = new Library();
            library.AddBook("书1", "张三", 2020);
            library.AddBook("书2", "李四", 2021);
            library.AddBook("书3", "张三", 2022);

            // Act & Assert
            // 验证可以搜索作者而不抛出异常
            library.SearchBooksByAuthor("张三");
            Assert.Equal(3, library.GetTotalBooks());
        }

        [Fact]
        public void Library_ShouldHandleEmptyCollection()
        {
            // Arrange
            var library = new Library();

            // Act & Assert
            Assert.Equal(0, library.GetTotalBooks());
            library.DisplayAllBooks(); // 应该不抛出异常
            library.SearchBooksByAuthor("不存在的作者"); // 应该不抛出异常
        }

        [Fact]
        public void BookIterator_ShouldHandleEmptyCollection()
        {
            // Arrange
            var collection = new BookCollection();
            var iterator = collection.GetIterator();

            // Act & Assert
            Assert.False(iterator.HasNext());
            Assert.Throws<InvalidOperationException>(() => iterator.Next());
        }
    }
}
