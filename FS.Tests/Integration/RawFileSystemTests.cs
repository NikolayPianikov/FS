namespace FS.Tests.Integration
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Core;
    using Shouldly;
    using Xunit;

    public class RawFileSystemTests: IDisposable
    {
        private readonly string _tempFile;
        private const int Size = 256000;
        private const int Position = 123;

        public RawFileSystemTests()
        {
            _tempFile = Path.GetTempFileName();
        }

        public void Dispose()
        {
            File.Delete(_tempFile);
            GC.SuppressFinalize(this);
        }

        [Fact]
        public void ShouldReadAndWriteBigFile()
        {
            // Given
            var root = CreateInstance(new RawFileSystemSettings<string>(_tempFile,256, 16));

            // When
            root.FileSystem.TryCreateFile(out var file).ShouldBeTrue();
            using var writer = root.FileSystem.CreateWriter(file);
            using var reader = root.FileSystem.CreateReader(file);

            // Then
            TestTool.ReadAndWriteFile(reader, writer, Position, Size);
        }
        
        [Fact]
        public async Task ShouldReadAndWriteBigFileAsync()
        {
            // Given
            var root = CreateInstance(new RawFileSystemSettings<string>(_tempFile, 256, 16));

            // When
            root.FileSystem.TryCreateFile(out var file).ShouldBeTrue();
            using var writer = root.FileSystem.CreateWriter(file);
            using var reader = root.FileSystem.CreateReader(file);

            // Then
            await TestTool.ReadAndWriteFileAsync(reader, writer, Position, Size);
        }
        
        [Fact]
        public void ShouldReadAndWriteFile()
        {
            // Given
            var root = CreateInstance(new RawFileSystemSettings<string>(_tempFile, 2, 3));

            // When
            root.FileSystem.TryCreateFile(out var file).ShouldBeTrue();
            using var writer = root.FileSystem.CreateWriter(file);
            using var reader = root.FileSystem.CreateReader(file);

            // Then
            TestTool.ReadAndWriteFile(reader, writer, 1, 8);
            root.TableStatistics.NumberOfSectorsUsed.ShouldBe(2);
            root.TableStatistics.NumberOfBlocksUsed.ShouldBe(5);
            root.FileSystemStatistics.NumberOfFiles.ShouldBe(1);
        }

        [Fact]
        public void ShouldDeleteFile()
        {
            // Given
            var root = CreateInstance(new RawFileSystemSettings<string>(_tempFile, 2, 3));
            root.FileSystem.TryCreateFile(out var file).ShouldBeTrue();
            using var writer = root.FileSystem.CreateWriter(file);
            using var reader = root.FileSystem.CreateReader(file);
            TestTool.ReadAndWriteFile(reader, writer, 1, 8);
            
            // When
            var result = root.FileSystem.TryDeleteFile(file);

            // Then
            result.ShouldBeTrue();
            root.TableStatistics.NumberOfSectorsUsed.ShouldBe(2);
            root.TableStatistics.NumberOfBlocksUsed.ShouldBe(0);
            root.FileSystemStatistics.NumberOfFiles.ShouldBe(0);
        }
        
        [Fact]
        public void ShouldFreeEmptySectorsWhenFlush()
        {
            // Given
            var root = CreateInstance(new RawFileSystemSettings<string>(_tempFile, 2, 3));
            root.FileSystem.TryCreateFile(out var file).ShouldBeTrue();
            using var writer = root.FileSystem.CreateWriter(file);
            using var reader = root.FileSystem.CreateReader(file);
            TestTool.ReadAndWriteFile(reader, writer, 1, 8);
            root.FileSystem.TryDeleteFile(file);

            // When
            var result = root.FileSystem.TryFlush();

            // Then
            result.ShouldBeTrue();
            root.TableStatistics.NumberOfSectorsUsed.ShouldBe(0);
            root.TableStatistics.NumberOfBlocksUsed.ShouldBe(0);
            root.FileSystemStatistics.NumberOfFiles.ShouldBe(0);
        }
        
        [Fact]
        public void ShouldUseEmptySpaceAfterFileWasDeleted()
        {
            // Given
            var root = CreateInstance(new RawFileSystemSettings<string>(_tempFile, 2, 3));
            root.FileSystem.TryCreateFile(out var file).ShouldBeTrue();
            using var writer = root.FileSystem.CreateWriter(file);
            using var reader = root.FileSystem.CreateReader(file);
            TestTool.ReadAndWriteFile(reader, writer, 1, 8);
            root.FileSystem.TryDeleteFile(file);

            // When
            root.FileSystem.TryCreateFile(out file);
            using var writer2 = root.FileSystem.CreateWriter(file);
            using var reader2 = root.FileSystem.CreateReader(file);
            TestTool.ReadAndWriteFile(reader2, writer2, 1, 8);

            // Then
            root.TableStatistics.NumberOfSectorsUsed.ShouldBe(2);
            root.TableStatistics.NumberOfBlocksUsed.ShouldBe(5);
            root.FileSystemStatistics.NumberOfFiles.ShouldBe(1);
        }
        
        [Fact]
        public void ShouldUseEmptySpaceAfterFileWasDeletedAndFlush()
        {
            // Given
            var root = CreateInstance(new RawFileSystemSettings<string>(_tempFile, 2, 3));
            root.FileSystemStatistics.NumberOfFiles.ShouldBe(0);
            root.FileSystem.TryCreateFile(out var file).ShouldBeTrue();
            using var writer = root.FileSystem.CreateWriter(file);
            using var reader = root.FileSystem.CreateReader(file);
            TestTool.ReadAndWriteFile(reader, writer, 1, 8);
            root.FileSystem.TryDeleteFile(file);
            
            // When
            root.FileSystem.TryCreateFile(out file);
            //root.FileSystem.TryFlush();
            using var writer2 = root.FileSystem.CreateWriter(file);
            using var reader2 = root.FileSystem.CreateReader(file);
            TestTool.ReadAndWriteFile(reader2, writer2, 1, 8);

            // Then
            root.TableStatistics.NumberOfSectorsUsed.ShouldBe(2);
            root.TableStatistics.NumberOfBlocksUsed.ShouldBe(5);
            root.FileSystemStatistics.NumberOfFiles.ShouldBe(1);
        }
        
        [Fact]
        public void ShouldDeleteFileWhenSeveralFiles()
        {
            // Given
            var root = CreateInstance(new RawFileSystemSettings<string>(_tempFile, 2, 3));

            // When
            root.FileSystem.TryCreateFile(out var file1).ShouldBeTrue();
            using var writer1 = root.FileSystem.CreateWriter(file1);
            using var reader1 = root.FileSystem.CreateReader(file1);
            TestTool.ReadAndWriteFile(reader1, writer1, 1, 8);
            
            root.FileSystem.TryCreateFile(out var file2).ShouldBeTrue();
            using var writer2 = root.FileSystem.CreateWriter(file2);
            using var reader2 = root.FileSystem.CreateReader(file2);
            TestTool.ReadAndWriteFile(reader2, writer2, 1, 2);

            root.FileSystem.TryDeleteFile(file1);
            root.FileSystem.TryFlush();

            // Then
            root.TableStatistics.NumberOfSectorsUsed.ShouldBe(3);
            root.TableStatistics.NumberOfBlocksUsed.ShouldBe(2);
            root.FileSystemStatistics.NumberOfFiles.ShouldBe(1);
        }
        
        [Fact]
        public void ShouldReadAndWriteFileInParallel()
        {
            // Given
            var root = CreateInstance(new RawFileSystemSettings<string>(_tempFile, 2, 3));

            // When
            TestTool.Parallelize(() =>
            {
                root.FileSystem.TryCreateFile(out var file).ShouldBeTrue();
                using var writer = root.FileSystem.CreateWriter(file);
                using var reader = root.FileSystem.CreateReader(file);
                TestTool.ReadAndWriteFile(reader, writer, 1, 8);
            });

            // Then
        }

        private static RawFileSystemTestCompositionRoot<string> CreateInstance(RawFileSystemSettings<string> settings)
        {
            var compositionRoot = Composer.Resolve<RawFileSystemTestCompositionRoot<string>>();
            compositionRoot.Settings.Apply(settings);
            return compositionRoot;
        }
    }
}