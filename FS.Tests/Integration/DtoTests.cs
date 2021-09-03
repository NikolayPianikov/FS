namespace FS.Tests.Integration
{
    using System;
    using System.IO;
    using Core;
    using Dto;
    using Google.Protobuf;
    using Shouldly;
    using Xunit;
    
    public class DtoTests: IDisposable
    {
        private readonly string _tempFile;

        public DtoTests() => _tempFile = Path.GetTempFileName();

        public void Dispose()
        {
            File.Delete(_tempFile);
            GC.SuppressFinalize(this);
        }
        
        [Fact]
        public void ShouldSerializeAndDeserializeProtobufDto()
        {
            // Given
            var root = CreateInstance(new RawFileSystemSettings<string>(_tempFile,4, 2));
            var entityToWrite = new Entity
            {
                BlockId = 2,
                SectorId = 7,
                Size = 10,
                Type = EntityType.Directory,
                Name = "Abc"
            };

            var readerToStreamConverter = Composer.Resolve<IConverter<IReader, Stream>>();
            var writerToStreamConverter = Composer.Resolve<IConverter<IWriter, Stream>>();

            root.FileSystem.TryCreateFile(out var file);
            using var writer = root.FileSystem.CreateWriter(file);
            using var reader = root.FileSystem.CreateReader(file);

            // When
            entityToWrite.WriteTo(writerToStreamConverter.Convert(writer));
            writer.Flush();
            var entityToRead = Entity.Parser.ParseFrom(readerToStreamConverter.Convert(reader));

            // Then
            entityToWrite.ShouldBe(entityToRead);
        }
        
        private static RawFileSystemTestCompositionRoot<string> CreateInstance(RawFileSystemSettings<string> settings)
        {
            var compositionRoot = Composer.Resolve<RawFileSystemTestCompositionRoot<string>>();
            compositionRoot.Settings.Apply(settings);
            return compositionRoot;
        }
    }
}