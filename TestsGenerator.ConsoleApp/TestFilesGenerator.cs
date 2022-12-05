using System.Threading.Tasks.Dataflow;
using TestsGenerator.Core;

namespace TestsGenerator.ConsoleApp
{
    internal class TestFilesGenerator
    {
        int _maxReadingFiles;
        int _maxProcessingFiles;
        int _maxWritingFiles;
        string _outDirectoryPath;
        Generator _generator = new Generator();

        TransformBlock<string, string> _readFile;
        TransformBlock<string, TestClassInfo[]> _processFile;
        ActionBlock<TestClassInfo[]> _writeFile;

        public TestFilesGenerator(int maxReadingFiles, int maxProcessingFiles, int maxWritingFiles, string outDirectoryPath)
        {
            _maxReadingFiles = maxReadingFiles;
            _maxProcessingFiles = maxProcessingFiles;
            _maxWritingFiles = maxWritingFiles;
            _outDirectoryPath = outDirectoryPath;

            _readFile = new TransformBlock<string, string>(
                async filename => await ReadFile(filename), 
                new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = _maxReadingFiles });

            _processFile = new TransformBlock<string, TestClassInfo[]>(
                text => _generator.GenerateTests(text),
                new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = _maxProcessingFiles });

            _writeFile = new ActionBlock<TestClassInfo[]>(
                async testClassInfo => await WriteFile(testClassInfo),
                new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = _maxWritingFiles });

            var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };
            _readFile.LinkTo(_processFile, linkOptions);
            _processFile.LinkTo(_writeFile, linkOptions);
        }

        public async Task Generate(string[] files)
        {
            foreach (string file in files)
            {
                _readFile.Post(file);
            }
            _readFile.Complete();
            await _writeFile.Completion;
        }

        private async Task<string> ReadFile(string filename)
        {
            string text = string.Empty;
            using (var reader = new StreamReader(filename))
            {
                text = await reader.ReadToEndAsync();
            }
            return text;
        }
        private async Task WriteFile(TestClassInfo[] testClassesInfo)
        {
            foreach (var testClassInfo in testClassesInfo)
            {
                using (var writer = new StreamWriter(_outDirectoryPath + "\\" + testClassInfo.Name + ".cs"))
                {
                    await writer.WriteAsync(testClassInfo.Content);
                }
            }
        }
    }
}
