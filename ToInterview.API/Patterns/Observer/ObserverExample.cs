namespace ToInterview.API.Patterns.Observer;

public interface IObserver
{
    void Update(string message);
}

public class EmailNotifier : IObserver
{
    private string _email;

    public EmailNotifier(string email)
    {
        _email = email;
    }

    public void Update(string message)
    {
        Console.WriteLine($"邮件通知 [{_email}]: {message}");
    }
}

public class SmsNotifier : IObserver
{
    private string _phoneNumber;

    public SmsNotifier(string phoneNumber)
    {
        _phoneNumber = phoneNumber;
    }

    public void Update(string message)
    {
        Console.WriteLine($"短信通知 [{_phoneNumber}]: {message}");
    }
}

public interface ISubject
{
    void Attach(IObserver observer);
    void Detach(IObserver observer);
    void Notify(string message);
}

public class NewsPublisher : ISubject
{
    private List<IObserver> _observers = new();
    private string _latestNews = string.Empty;

    public void Attach(IObserver observer)
    {
        _observers.Add(observer);
    }

    public void Detach(IObserver observer)
    {
        _observers.Remove(observer);
    }

    public void Notify(string message)
    {
        foreach (var observer in _observers)
        {
            observer.Update(message);
        }
    }

    public void PublishNews(string news)
    {
        _latestNews = news;
        Notify(news);
    }

    public string GetLatestNews()
    {
        return _latestNews;
    }
}
