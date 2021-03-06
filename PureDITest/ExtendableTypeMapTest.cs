﻿using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PureDI;
using static IOCCTest.Utils;

namespace IOCCTest
{
    [TestClass]
    public class ExtendableTypeMapTest
    {
        [TestMethod]
        public void ShouldHonourAssembliesAddedAtSecondInjection()
        {
            (var pdi, var initialAssembly) = CreateIOCCinAssembly("InitialAssemblyData", "EntryPoint");
            (var obj1, var @is) = pdi.CreateAndInjectDependencies("IOCCTest.InitialAssemblyData.EntryPoint"
                , assemblies: new Assembly[] { initialAssembly});
            Assert.IsNotNull(obj1);
            Assembly additionalAssembly = CreateAssembly($"{TestResourcePrefix}.AdditionalAssemblyData.EntryPoint.cs");
            (var obj2, _) = pdi.CreateAndInjectDependencies("IOCCTest.AdditionalAssemblyData.EntryPoint", @is
              , assemblies: new Assembly[] {additionalAssembly});
            Assert.IsNotNull(obj2);
        }
    }
}