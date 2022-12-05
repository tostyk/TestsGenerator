namespace TestsGenerator.ConsoleApp
{
    public class Program
    {
        static int _maxReadingFiles;
        static int _maxProcessingFiles;
        static int _maxWritingFiles;
        static string? _outDirectory;
        static string? _sourceDirectory;

        public static async Task Main()
        {
            //GetParameters();
            //TestFilesGenerator generator = new(_maxReadingFiles, _maxProcessingFiles, _maxWritingFiles, _outDirectory);
            TestFilesGenerator generator = new(3, 3, 3, "dir");
            _sourceDirectory = "D:\\Универ\\5_сем\\СПП\\Лабы\\Лаба 4\\TestsGenerator\\TestsGenerator.ConsoleApp";
            await generator.Generate(Directory.GetFiles(_sourceDirectory, "*.cs"));
        }
        private static void GetParameters()
        {
            Console.WriteLine("Enter the maximum number of parallel reading tasks:");
            while(!int.TryParse(Console.ReadLine(), out _maxReadingFiles) || _maxReadingFiles <= 0)
            {
                Console.WriteLine("Wrong number!");
                Console.WriteLine("Enter the maximum number of parallel reading tasks:");
            }

            Console.WriteLine("Enter the maximum number of parallel processing tasks:");
            while (!int.TryParse(Console.ReadLine(), out _maxProcessingFiles) || _maxProcessingFiles <= 0)
            {
                Console.WriteLine("Wrong number!");
                Console.WriteLine("Enter the maximum number of parallel processing tasks:");
            }

            Console.WriteLine("Enter the maximum number of parallel writing tasks:");
            while (!int.TryParse(Console.ReadLine(), out _maxWritingFiles) || _maxWritingFiles <= 0)
            {
                Console.WriteLine("Wrong number!");
                Console.WriteLine("Enter the maximum number of parallel writing tasks:");
            }

            Console.WriteLine("Enter the directory for test classes:");
            _outDirectory = Console.ReadLine();
            while (_outDirectory == null)
            {
                Console.WriteLine("Wrong path!");
                Console.WriteLine("Enter the directory for test classes:");
                _outDirectory = Console.ReadLine();
            }
            if (!Directory.Exists(_outDirectory)) Directory.CreateDirectory(_outDirectory);

            Console.WriteLine("Enter the directory with source classes:");
            _sourceDirectory = Console.ReadLine();
            if (_sourceDirectory == null || !Directory.Exists(_sourceDirectory))
            {
                Console.WriteLine("Wrong path!");
                Console.WriteLine("Enter the directory with source classes:");
                _sourceDirectory = Console.ReadLine();
            }
        }
    }
}
