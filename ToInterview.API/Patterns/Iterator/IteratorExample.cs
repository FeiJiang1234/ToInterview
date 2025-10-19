namespace ToInterview.API.Patterns.Iterator;

public interface IIterator<T>
{
    bool HasNext();
    T Next();
    void Reset();
}

public interface IIterable<T>
{
    IIterator<T> GetIterator();
}

public class Book
{
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public int Year { get; set; }

    public Book(string title, string author, int year)
    {
        Title = title;
        Author = author;
        Year = year;
    }

    public override string ToString()
    {
        return $"{Title} by {Author} ({Year})";
    }
}

public class BookCollection : IIterable<Book>
{
    private List<Book> _books = new();

    public void AddBook(Book book)
    {
        _books.Add(book);
    }

    public void RemoveBook(Book book)
    {
        _books.Remove(book);
    }

    public int Count => _books.Count;

    public IIterator<Book> GetIterator()
    {
        return new BookIterator(_books);
    }
}

public class BookIterator : IIterator<Book>
{
    private List<Book> _books;
    private int _currentIndex = 0;

    public BookIterator(List<Book> books)
    {
        _books = books;
    }

    public bool HasNext()
    {
        return _currentIndex < _books.Count;
    }

    public Book Next()
    {
        if (!HasNext())
        {
            throw new InvalidOperationException("没有更多元素");
        }

        var book = _books[_currentIndex];
        _currentIndex++;
        return book;
    }

    public void Reset()
    {
        _currentIndex = 0;
    }
}

public class Library
{
    private BookCollection _books;

    public Library()
    {
        _books = new BookCollection();
    }

    public void AddBook(string title, string author, int year)
    {
        _books.AddBook(new Book(title, author, year));
    }

    public void DisplayAllBooks()
    {
        Console.WriteLine("=== 图书馆藏书 ===");
        var iterator = _books.GetIterator();
        
        while (iterator.HasNext())
        {
            var book = iterator.Next();
            Console.WriteLine($"📚 {book}");
        }
        Console.WriteLine();
    }

    public void SearchBooksByAuthor(string author)
    {
        Console.WriteLine($"=== 作者 {author} 的书籍 ===");
        var iterator = _books.GetIterator();
        bool found = false;
        
        while (iterator.HasNext())
        {
            var book = iterator.Next();
            if (book.Author.Contains(author))
            {
                Console.WriteLine($"📚 {book}");
                found = true;
            }
        }
        
        if (!found)
        {
            Console.WriteLine("未找到相关书籍");
        }
        Console.WriteLine();
    }

    public int GetTotalBooks()
    {
        return _books.Count;
    }
}
