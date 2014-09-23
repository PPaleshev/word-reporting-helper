﻿using System;
using System.IO;
using NUnit.Framework;
using SampleWordHelper.Model;
using SampleWordHelper.Providers.FileSystem;

namespace SampleWordHelper.Tests
{
    [TestFixture]
    [Explicit]
    public class FileSystemParserTests
    {
        [Test]
        public void ParserTests()
        {
            var parser = new CatalogBuilder(new DirectoryInfo(@"d:\Projects\Private\WordHelper\SampleCatalog"));
            var model = new CatalogModel();
            parser.BuildCatalog(model);
        }

        [TestCase(@"d:\Projects\Private\WordHelper\SampleCatalog\")]
        [TestCase(@"d:\Projects\Private\WordHelper\SampleCatalog")]
        public void UriTests(string rootPath)
        {
            var rootUri = new Uri(rootPath);
            var fileUri = new Uri(@"d:\Projects\Private\WordHelper\SampleCatalog\Основное.docx");
            var subdirectoryUriSlash = new Uri(@"d:\Projects\Private\WordHelper\SampleCatalog\1\");
            var subdirectoryUriNoSlash = new Uri(@"d:\Projects\Private\WordHelper\SampleCatalog\1");
            Console.WriteLine(GetRelative(rootUri, fileUri));
            Console.WriteLine(GetRelative(rootUri, subdirectoryUriSlash));
            Console.WriteLine(GetRelative(rootUri, subdirectoryUriNoSlash));
            Console.WriteLine(GetRelative(rootUri, new Uri(@"d:\Projects\Private\WordHelper\SampleCatalog\")));
            Console.WriteLine(GetRelative(rootUri, new Uri(@"d:\Projects\Private\WordHelper\SampleCatalog")));
        }

        string GetRelative(Uri root, Uri test)
        {
            return Uri.UnescapeDataString(root.MakeRelativeUri(test).OriginalString);
        }
    }
}