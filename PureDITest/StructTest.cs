﻿using PureDI;
using IOCCTest.TestCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IOCCTest
{
    [TestClass]
    public class StructTest
    {
        [TestMethod]
        public void ShouldInstantiateStruct()
        {
            StructRoot root = new DependencyInjector().CreateAndInjectDependencies<StructRoot>().rootBean;
            Assert.IsNotNull(root.child);
        }

        [TestMethod]
        public void ShouldCreateATreeWithStructs()
        {
            StructTree tree = new DependencyInjector().CreateAndInjectDependencies<StructTree>().rootBean;
            Assert.IsNotNull(tree.structChild);
        }

        [TestMethod]
        public void ShouldCreateTreeWithMixOfClassesAndStructs()
        {
            ClassTree tree = new DependencyInjector().CreateAndInjectDependencies<ClassTree>().rootBean;
            Assert.IsNotNull(tree);
            Assert.AreEqual(1, tree?.GetStructChild2().GetClassChild()?.someValue);
        }
    }
}