﻿using System;
using System.CodeDom;
using System.Reflection;
using PureDI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IOCCTest.TestCode;

namespace IOCCTest
{
    [TestClass]
    public class MultipleCallsTest
    {
        [TestMethod]
        public void ShouldExtendTreeWhenMoreCallsAreMode()
        {
            (DependencyInjector pdi, Assembly assembly) = Utils.CreateIOCCinAssembly("MultipleCallsTestData", "Simple");
            (object simpleBean, InjectionState injectionState) 
              = pdi.CreateAndInjectDependencies(
              "IOCCTest.MultipleCallsTestData.Simple"
              , assemblies: new Assembly[] { assembly});
            (object furtherBean, InjectionState injectionState2) 
              = pdi.CreateAndInjectDependencies(
              "IOCCTest.MultipleCallsTestData.Further", injectionState
              , assemblies: new Assembly[] { assembly});
            Assert.AreEqual(simpleBean, ((IResultGetter)furtherBean)?.GetResults().Simple );
        }

        [TestMethod]
        public void ShouldRejectAttemptToCreateTreeForASecondTimeWithoutState()
        {
            //DependencyInjector pdi = Utils.CreateIOCCinAssembly("MultipleCallsTestData", "Fails");
            DependencyInjector pdi = new DependencyInjector();
                    // remote assembly refuses to work (Bad IL Format) in this test despite
                    // being identical to the one above
            Assert.ThrowsException<ArgumentException>(() =>
                {
                    (object simpleBean, InjectionState injectionState) 
                      = pdi.CreateAndInjectDependencies(
                      "IOCCTest.MultipleCallsTestData.Fails",
                      assemblies: new Assembly[] { this.GetType().Assembly});
                    (object furtherBean, InjectionState injectionState2) 
                      = pdi.CreateAndInjectDependencies(
                      "IOCCTest.MultipleCallsTestData.FurtherFails"
                      ,assemblies: new Assembly[] { this.GetType().Assembly});
                }
            );
        }

        [TestMethod]
        public void ShouldCreateTreesWithMultipleCallsWithEmptyState()
        {
            (DependencyInjector pdi, Assembly assembly) = Utils.CreateIOCCinAssembly("MultipleCallsTestData", "Empty");
            (object empty1, InjectionState injectionState) 
              = pdi.CreateAndInjectDependencies(
              "IOCCTest.MultipleCallsTestData.Empty", assemblies: new Assembly[] { assembly});
            (object empty2, InjectionState @is2) = pdi.CreateAndInjectDependencies(
                "IOCCTest.MultipleCallsTestData.Empty", InjectionState.Empty, assemblies: new Assembly[] { assembly});
            Assert.AreNotEqual(empty1, empty2);
        }
        [TestMethod]
        public void ShouldCreateTreesWithRecursingFactories()
        {
            (DependencyInjector pdi, Assembly assembly) = Utils.CreateIOCCinAssembly("MultipleCallsTestData", "SimpleFactory");
            (object factory1, InjectionState injectionState) 
              = pdi.CreateAndInjectDependencies(
              "IOCCTest.MultipleCallsTestData.SimpleFactory", assemblies: new Assembly[] { assembly});
            Assert.IsNotNull( ((IResultGetter)factory1)?.GetResults().SimpleChild);
        }

        [TestMethod]
        public void ShouldCreateTreeWithAComplexArrangementOfFactories()
        {
            (DependencyInjector pdi, Assembly assembly) = Utils.CreateIOCCinAssembly("MultipleCallsTestData", "ComplexFactory");
            (object factory1, InjectionState injectionState)
                = pdi.CreateAndInjectDependencies(
                    "IOCCTest.MultipleCallsTestData.ComplexFactory", assemblies: new Assembly[] { assembly});
            IResultGetter result = factory1 as IResultGetter;
            ;
            Assert.IsNotNull(result.GetResults()?.ChildOne);
            Assert.IsNotNull(result.GetResults()?.ChildTwo);
            Assert.IsNotNull(((IResultGetter)result.GetResults()?.ChildOne)?.GetResults().ChildTwo);
            Assert.AreEqual(result?.GetResults().ChildTwo
              , ((IResultGetter)result.GetResults()?.ChildOne)?.GetResults().ChildTwo);
        }
        [TestMethod]
        public void ShouldAddFactoryCreatedParameterBeansToInjectionState()
        {
            (DependencyInjector pdi, Assembly assembly) = Utils.CreateIOCCinAssembly(
              "MultipleCallsTestData", "FactoryMadeWithConstructor");
//            Assembly assembly = this.GetType().Assembly;
//            var pdi = new DependencyInjector();
            (object fmwc, InjectionState injectionState)
                = pdi.CreateAndInjectDependencies(
                    "IOCCTest.MultipleCallsTestData.FactoryMadeWithConstructor", assemblies: new Assembly[] { assembly});
            IResultGetter result = fmwc as IResultGetter;

            Assert.AreEqual(1, result?.GetResults().FurthestCtr);
            (object furthest, _)
                = pdi.CreateAndInjectDependencies(
                    "IOCCTest.MultipleCallsTestData.Furthest", injectionState: injectionState, assemblies: new Assembly[] { assembly});
            Assert.AreEqual(1, result?.GetResults().FurthestCtr);
        }
        [TestMethod]
        public void ShouldAddFactoryCreatedMemberBeansToInjectionState()
        {
            (DependencyInjector pdi, Assembly assembly) = Utils.CreateIOCCinAssembly(
              "MultipleCallsTestData", "FactoryMadeWithMember");
//            Assembly assembly = this.GetType().Assembly;
//            var pdi = new DependencyInjector();
            (object fmwc, InjectionState injectionState)
                = pdi.CreateAndInjectDependencies(
                    "IOCCTest.MultipleCallsTestData.FactoryMadeWithMember", assemblies: new Assembly[] { assembly});
            IResultGetter result = fmwc as IResultGetter;

            Assert.AreEqual(1, result?.GetResults().FurthestCtr);
            (object furthest, _)
                = pdi.CreateAndInjectDependencies(
                    "IOCCTest.MultipleCallsTestData.Furthest", injectionState: injectionState, assemblies: new Assembly[] { assembly});
            Assert.AreEqual(1, result?.GetResults().FurthestCtr);
        }
        [TestMethod]
        public void ShouldAddFactoryCreatedComplexBeansToInjectionState()
        {
            (DependencyInjector pdi, Assembly assembly) = Utils.CreateIOCCinAssembly(
              "MultipleCallsTestData", "FactoryMadeComplex");
//            Assembly assembly = this.GetType().Assembly;
//            var pdi = new DependencyInjector();
            (object fmwc, InjectionState injectionState)
                = pdi.CreateAndInjectDependencies(
                    "IOCCTest.MultipleCallsTestData.FactoryMadeComplex", assemblies: new Assembly[] { assembly});
            IResultGetter result = fmwc as IResultGetter;

            Assert.AreEqual(1, result?.GetResults().FurthestCtr);
            (object furthest, _)
                = pdi.CreateAndInjectDependencies(
                    "IOCCTest.MultipleCallsTestData.Furthest", injectionState: injectionState, assemblies: new Assembly[] { assembly});
            Assert.AreEqual(1, result?.GetResults().FurthestCtr);
        }
        [TestMethod]
        public void ShouldReferenceUserCreatedObject()
        {
            DependencyInjector pdi = new DependencyInjector();
            var isbu = new IOCCTest.MultipleCallsTestCode.InstantiatedByUser();
            isbu.value = 42;
            (_, InjectionState @is) = pdi.CreateAndInjectDependencies(isbu);
            (var cup, InjectionState @is2) = pdi.CreateAndInjectDependencies<IOCCTest.MultipleCallsTestCode.CreatedByPureDI>(@is);
            Assert.IsNotNull(cup.instantiatedByUser);
            Assert.AreEqual(42, cup.instantiatedByUser?.value);
        }
    }
}

namespace IOCCTest.MultipleCallsTestCode
{
    using PureDI.Attributes;
    [Bean]
    public class InstantiatedByUser
    {
        public int value = 0;
    }
    [Bean]
    public class CreatedByPureDI
    {
        [BeanReference] public InstantiatedByUser instantiatedByUser = null;
    }
} 