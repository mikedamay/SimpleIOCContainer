﻿using System.Dynamic;
using System.Reflection;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static IOCCTest.Utils;

namespace IOCCTest
{
    [TestClass]
    public class NamespaceTest
    {
        [TestMethod]
        public void ShouldCreateTreeWithNoNamespace()
        {
            var iocc = Utils.CreateIOCCinAssembly("NamespaceData", "NoNamespace");
            object rootBean = iocc.CreateAndInjectDependencies("NoNamespace"
              , out IOCCDiagnostics diagnostics);
            System.Diagnostics.Debug.WriteLine(diagnostics);
            Assert.IsNotNull(rootBean);
            Assert.IsFalse(Falsify(diagnostics.HasWarnings));

        }
        [TestMethod]
        public void ShouldCreateTreeWithReferenceFromNoNamespaceToNamespace()
        {
            var iocc = Utils.CreateIOCCinAssembly("NamespaceData", "ReferenceToNamespacedClass");
            object rootBean = iocc.CreateAndInjectDependencies("ReferenceToNamespacedClass"
                , out IOCCDiagnostics diagnostics);
            System.Diagnostics.Debug.WriteLine(diagnostics);
            Assert.IsNotNull(rootBean);
            Assert.IsNotNull(((IResultGetter)rootBean).GetResults().Referred);
            Assert.IsFalse(Falsify(diagnostics.HasWarnings));

        }
        [TestMethod]
        public void ShouldCreateTreeWithReferenceToNoNamespaceFromNamespace()
        {
            var iocc = Utils.CreateIOCCinAssembly("NamespaceData", "ReferenceFromNamespacedClass");
            object rootBean = iocc.CreateAndInjectDependencies("IOCCTest.NoNamespaceData.ReferenceFromNamespacedClass"
                , out IOCCDiagnostics diagnostics);
            System.Diagnostics.Debug.WriteLine(diagnostics);
            Assert.IsNotNull(rootBean);
            Assert.IsNotNull(((IResultGetter)rootBean).GetResults().Referred);
            Assert.IsFalse(Falsify(diagnostics.HasWarnings));
        }

        // we currently have no way of handling 2 assemblies with the same namespace/classes
        // (i.e. where extern alias would be employed).
        // This half hearted test is just a place holder - we don't actually do the
        // extern alias bit.  We'll just document it.
        [TestMethod]
        public void ShouldHandleDuplicateAssemblies()
        {
            Assert.ThrowsException<IOCCInternalException>(() =>
                {
                    string codeText = GetResource(
                        "SimpleIOCContainerTest.NamespaceData.DuplicateAssemblies.cs");
                    //string codeText = "public class abc {}";
                    Assembly assembly = new AssemblyMaker().MakeAssembly(
                        codeText, "RemoteAssembly", new[] { this.GetType().Assembly});
                    SimpleIOCContainer sic = new SimpleIOCContainer(Assemblies: new [] {assembly, this.GetType().Assembly});
                    object obj = sic.CreateAndInjectDependencies<global::IOCCTest.DuplicateAssemblies.DuplicateAssemblies>();
                    Assert.IsNotNull(obj);
                    
                }
            );
        }
    }
}
namespace IOCCTest.DuplicateAssemblies
{
    [Bean]
    public class DuplicateAssemblies : IResultGetter
    {
        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.AssemblyName = GetType().Assembly.GetName().FullName;
            return eo;
        }
    }
}