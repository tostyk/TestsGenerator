namespace TestsGenerator.ConsoleApp
{
    public class Program
    {
        static int _maxReadingFiles = 0;
        static int _maxProcessingFiles = 0;
        static int _maxWritingFiles = 0;
        static string? _outDirectory;
        static List<string> _sourceFiles = new();

        public static async Task Main()
        {
            GetParameters();
            //_sourceFiles.Add("D:\\Универ\\5_сем\\СПП\\Лабы\\Лаба 4\\TestsGenerator\\TestsGenerator.Example\\ExampleClass.cs");
            //_outDirectory = "D:\\Универ\\5_сем\\СПП\\Лабы\\Лаба 4\\TestsGenerator\\TestsGenerator.Example.Tests";
            TestFilesGenerator generator = new(_maxReadingFiles, _maxProcessingFiles, _maxWritingFiles, _outDirectory);
            await generator.Generate(_sourceFiles.ToArray());
        }
        private static void GetParameters()
        {
            Console.Write("maximum number of parallel reading tasks>");
            while(!int.TryParse(Console.ReadLine(), out _maxReadingFiles) || _maxReadingFiles <= 0)
            {
                Console.WriteLine("Wrong value");
                Console.Write("maximum number of parallel reading tasks>");
            }

            Console.Write("maximum number of parallel processing tasks>");
            while (!int.TryParse(Console.ReadLine(), out _maxProcessingFiles) || _maxProcessingFiles <= 0)
            {
                Console.WriteLine("Wrong value");
                Console.Write("maximum number of parallel processing tasks>");
            }

            Console.Write("maximum number of parallel writing tasks>");
            while (!int.TryParse(Console.ReadLine(), out _maxWritingFiles) || _maxWritingFiles <= 0)
            {
                Console.WriteLine("Wrong value");
                Console.Write("maximum number of parallel writing tasks>");
            }

            Console.Write("out directory>");
            _outDirectory = Console.ReadLine();
            while (_outDirectory == null || _outDirectory.Trim() == "")
            {
                Console.WriteLine("Wrong path");
                Console.Write("out directory>");
                _outDirectory = Console.ReadLine();
            }
            if (!Directory.Exists(_outDirectory)) Directory.CreateDirectory(_outDirectory);

            Console.Write("source file or start>");
            string? str = Console.ReadLine();
            while (str != "start")
            {
                if (str != null && !File.Exists(str))
                {
                    Console.WriteLine("File not exists");
                }
                else if (str != null && Path.GetExtension(str) != ".cs")
                {
                    Console.WriteLine("Incorrect file extension");
                }
                else if(str != null)
                {
                    _sourceFiles.Add(str);
                }
                Console.Write("source file or start>");
                str = Console.ReadLine();
            }
        }
    }
}
