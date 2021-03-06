﻿using PureDI;
using IOCCTest.TestCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static IOCCTest.Utils;

namespace IOCCTest
{
    [TestClass]
    public class GenericTest
    {
        [TestMethod]
        public void ShouldCreateTreeWithGenerics()
        {
            RefersToGeneric rtg = new DependencyInjector().CreateAndInjectDependencies<RefersToGeneric>().rootBean;
            Assert.AreEqual("Generic<T>", rtg?.GenericInt?.Name);
        }
        [TestMethod]
        public void ShouldCreateTreeWithGenericRoot()
        {
            Generic<int> gi = new DependencyInjector().CreateAndInjectDependencies<Generic<int>>().rootBean;
            Assert.IsNotNull(gi);
        }
        [TestMethod]
        public void ShouldCreateTreeWithGenericParameter()
        {
            GenericHolderParent ghp = new DependencyInjector().CreateAndInjectDependencies<GenericHolderParent>().rootBean;
            Assert.AreEqual("GenericHeld", ghp?.GenericHolder?.GenericHeld.Name);
        }
        [TestMethod]
        public void ShouldCreateTreeWhenRootHasGenericParameter()
        {
            GenericHolder<GenericHeld> ghgh
                = new DependencyInjector().CreateAndInjectDependencies<GenericHolder<GenericHeld>>().rootBean;
            Assert.AreEqual("GenericHeld", ghgh?.GenericHeld?.Name);
        }

        [TestMethod]
        public void ShouldCreateTreeForGenericsWithMultipleParameters()
        {
            MultipleParamGenericUser mpgu
                = new DependencyInjector().CreateAndInjectDependencies<MultipleParamGenericUser>().rootBean;
            Assert.IsNotNull(mpgu?.Multiple);
        }

        [TestMethod]
        public void ShouldCreateTreeForNestedGeneric()
        {
            WrapperUser wu = new DependencyInjector().CreateAndInjectDependencies<WrapperUser>().rootBean;
            Assert.IsNotNull(wu?.Nested);
        }
        [TestMethod]
        public void ShouldInjectBeanFromGenericInterface()
        {
            (var result, var diagnostics) = CreateAndRunAssembly("GenericTestData", "Generic");
            Assert.AreEqual(0, result.GetResults().Value);
            Assert.IsFalse(Falsify(diagnostics.HasWarnings));
        }
    }
}