namespace ToInterview.API.Delegates
{
    public delegate int MathOperation(int x, int y);
    
    public delegate T GenericOperation<T>(T x, T y);
    
    public delegate void NotificationHandler(string message);
    
    public delegate void ProcessDataHandler<T>(T data, string operation);

    public class DelegateExamples
    {
        public static int Add(int x, int y) => x + y;

        public static T Max<T>(T x, T y) where T : IComparable<T>
        {
            return x.CompareTo(y) > 0 ? x : y;
        }

        public static void LogToConsole(string message)
        {
            Console.WriteLine($"[Console] {DateTime.Now:HH:mm:ss} - {message}");
        }

        public static void LogToFile(string message)
        {
            Console.WriteLine($"[File] {DateTime.Now:HH:mm:ss} - {message}");
        }

        public static void SendEmail(string message)
        {
            Console.WriteLine($"[Email] {DateTime.Now:HH:mm:ss} - {message}");
        }

        public static void ProcessWithCallback<T>(T[] data, Func<T, T> processor, Action<T> callback)
        {
            foreach (var item in data)
            {
                var result = processor(item);
                callback(result);
            }
        }
    }
}
