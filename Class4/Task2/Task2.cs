using System.Text;

namespace Task2
{
    public class Task2
    {
       
        public static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            if (args is not [var filePath, var encodingFrom, var encodingTo])
            {
                Console.WriteLine("Usage: [task2.exe] filePath encodingFrom encodingTo");
                return;
            }
            var text = File.ReadAllText(filePath, Encoding.GetEncoding(encodingFrom));
            File.WriteAllText(filePath, text, Encoding.GetEncoding(encodingTo));
        }
    }
}
