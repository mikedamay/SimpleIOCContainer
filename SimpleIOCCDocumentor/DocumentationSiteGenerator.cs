﻿using System.Reflection.Metadata;
using com.TheDisappointedProgrammer.IOCC;
using System.IO;
using com.TheDisappointedProgrammer.IOCC.Common;

namespace SimpleIOCCDocumentor
{
    public interface IDocumentationSiteGenerator
    {
        void GenerateSite();
    }
    [Bean]
    public class DocumentationSiteGenerator : IDocumentationSiteGenerator
    {
        // very wasteful to instantiate the parsers 2x.
        // we really need constructor paraemters
        [IOCCDocumentParser(
            DocumentPath: "SimpleIOCContainer.Docs.UserGuide.xml"
            , XmlRoot: Constants.USER_GUIDE_ROOT)]
        private IOCCDocumentParser userGuideDocumentParser = null;
        [IOCCDocumentParser(
            DocumentPath: "SimpleIOCContainer.Docs.DiagnosticSchema.xml"
            , XmlRoot: Constants.DIAGNOSTIC_SCHEMA_ROOT)]
        private IOCCDocumentParser diagnosticSchemaDocumentParser = null;

        private string path = "c:/projects/SimpleIOCContainer/Simple";

        [BeanReference] private IDocumentProcessor documentProcessor = null;

        public void GenerateSite()
        {
            GenerateIndex();
            GenerateFilesFromDocument("UserGuide", userGuideDocumentParser);
            GenerateFilesFromDocument("DiagnosticSchema", diagnosticSchemaDocumentParser);
        }

        private void GenerateIndex()
        {
            var index = documentProcessor.ProcessDocument(string.Empty, string.Empty);
            File.WriteAllText(Path.Combine(path, "index.html"), index);
        }

        private void GenerateFilesFromDocument(string documentName, IOCCDocumentParser documentParser)
        {
            var index = documentParser.GetDocumentIndex().Keys;
            foreach (var fragmentKey in index)
            {
                string fragmentName = Cleanup(fragmentKey);
                string fragment = documentProcessor.ProcessDocument(documentName, fragmentName);
                SaveAsDocument(documentName, fragmentName, fragment);
            }
        }
        /// <param name="fragmentKey">e.g."[Introduction](/Simple/UserGuide/Instroduction)"</param>
        /// <returns>"Introduction"</returns>
        private string Cleanup(string fragmentKey)
        {
            var parts = fragmentKey.Split("]");
            return parts[0].Replace("[", string.Empty);
                    // TODO we should be using the router to do this
        }

        /// <param name="documentName">e.g. UserGuide or DiagnosticSchema - no extension</param>
        /// <param name="fragment">a string of html which will sit quietly until rendered</param>
        private void SaveAsDocument(string documentName, string fragmentName, string fragment)
        {
            File.WriteAllText(Path.Combine(Path.Combine(path, documentName), fragmentName) + ".html", fragment);
        }
    }
}