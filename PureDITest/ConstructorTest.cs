﻿using System.Dynamic;
using System.Reflection;
using PureDI;
using IOCCTest.TestCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static IOCCTest.Utils;

namespace IOCCTest
{
    [TestClass]
    public class ConstructorTest
    {
        private const string CONSTRUCTOR_TEST_NAMESPACE = "ConstructorTestData";
        [TestMethod]
        public void ShouldThrowExceptionForNoArgConstructor()
        {
            Assert.ThrowsException<DIException>(() =>
            {
                (NoArgRoot st, InjectionState InjectionState) = new DependencyInjector().CreateAndInjectDependencies<
                    NoArgRoot>();
                Diagnostics diags = InjectionState.Diagnostics;
                Assert.IsTrue(diags.HasWarnings);
            });
        }
        [TestMethod]
        public void ShouldWarnForNoArgClassTree()
        {
            (NoArgClassTree nact, InjectionState InjectionState) = new DependencyInjector().CreateAndInjectDependencies<
                NoArgClassTree>();
            Diagnostics diags = InjectionState.Diagnostics;
            Assert.IsTrue(diags.HasWarnings);
        }

        [TestMethod]
        public void ShouldInstantiateSingleObjectFromMultipleInterfaces()
        {
            ClassWithMultipleInterfaces cwmi
                = new DependencyInjector().CreateAndInjectDependencies<ClassWithMultipleInterfaces>().rootBean;
            Assert.IsNotNull(cwmi?.GetResults().Interface1);
            Assert.IsTrue(cwmi?.GetResults().Interface1 == cwmi?.GetResults().Interface2);
        }
        [TestMethod]
        public void ShouldCreateTreeWithSimpleConstructor()
        {
            (dynamic result, var diagnostics) = Utils.CreateAndRunAssembly(
                CONSTRUCTOR_TEST_NAMESPACE, "SimpleConstructor", usePureDiTestAssembly: false);
            Assert.AreEqual(42, result?.GetResults().SomeValue);
            Assert.IsFalse(Falsify(diagnostics.HasWarnings));
        }
        [TestMethod]
        public void ShouldCreateTreeWithMultipleParameters()
        {
            (dynamic result, var diagnostics) = Utils.CreateAndRunAssembly(
                CONSTRUCTOR_TEST_NAMESPACE, "MultipleParams");
            Assert.IsNotNull(result?.GetResults().ParamOne);
            Assert.IsNotNull(result?.GetResults().ParamTwo);
            Assert.IsFalse(Falsify(diagnostics.HasWarnings));
        }

        [TestMethod]
        public void ShouldCreateDeepHierarchy()
        {
            (dynamic result, var diagnostics) = Utils.CreateAndRunAssembly(
                CONSTRUCTOR_TEST_NAMESPACE, "DeepHierarchy");
            Assert.IsNotNull(result?.GetResults().Level1?.GetResults().Level2);
            Assert.IsFalse(Falsify(diagnostics.HasWarnings));            
        }

        [TestMethod]
        public void ShouldCreateTreeWithMultipleConstructors()
        {
            (dynamic result, var diagnostics) = Utils.CreateAndRunAssembly(
                CONSTRUCTOR_TEST_NAMESPACE, "MultipleConstructors");
            Assert.IsNotNull(result?.GetResults().Level1a?.Level2a);
            Assert.IsNotNull(result?.GetResults().Level1b?.Level2b);
            Assert.IsNull(result?.GetResults().Level1a?.Level2b);
            Assert.IsNull(result?.GetResults().Level1b?.Level2a);
            Assert.IsFalse(Falsify(diagnostics.HasWarnings));            
         }
        [TestMethod]
        public void ShouldCreateTreeWithFactoryConstructorParams()
        {
            (dynamic result, var diagnostics) = Utils.CreateAndRunAssembly(
                CONSTRUCTOR_TEST_NAMESPACE, "Factory");
            Assert.IsNotNull(result?.GetResults().Level1?.GetResults().Level2);
            Assert.IsFalse(Falsify(diagnostics.HasWarnings));
        }
        [TestMethod]
        public void ShouldCreateTreeWithFactoryInjectedViaConstructor()
        {
            (dynamic result, var diagnostics) = Utils.CreateAndRunAssembly(
                CONSTRUCTOR_TEST_NAMESPACE, "FactoryConstructor");
            Assert.IsNotNull(result?.GetResults().Level1?.GetResults().Level2);
            Assert.IsFalse(Falsify(diagnostics.HasWarnings));
        }
        [TestMethod]
        public void ShouldCreateTreeWithPrivateConstructor()
        {
            (dynamic result, var diagnostics) = Utils.CreateAndRunAssembly(
                CONSTRUCTOR_TEST_NAMESPACE, "PrivateConstructor");
            Assert.AreEqual(42, result?.GetResults().SomeValue);
            Assert.IsFalse(Falsify(diagnostics.HasWarnings));

        }
        [TestMethod]
        public void ShouldWarnIfDuplicateConstructors()
        {
            Assert.ThrowsException<DIException>(() =>
            {
                (dynamic result, var diagnostics) = Utils.CreateAndRunAssembly(
                    CONSTRUCTOR_TEST_NAMESPACE, "DuplicateConstructors");
            });
        }
        [TestMethod]
        public void ShouldWarnIfSomeParametersAreNotMarkedForInjetion()
        {
            Assert.ThrowsException<DIException>(() =>
            {
                (dynamic result, var diagnostics) = Utils.CreateAndRunAssembly(
                    CONSTRUCTOR_TEST_NAMESPACE, "UnmarkedParameter");
            });
        }
        [TestMethod]
        public void ShouldWarnIfSomeUnmarkedConstructorsContainMarkedParameters()
        {
            (dynamic result, var diagnostics) = Utils.CreateAndRunAssembly(
                CONSTRUCTOR_TEST_NAMESPACE, "UnmarkedConstructor");
        }
        [TestMethod]
        public void ShouldWarnIfSomeUnmarkedMatchingConstructorsContainMarkedParameters()
        {
            Assert.ThrowsException<DIException>(() =>
            {
                (dynamic result, var diagnostics) = Utils.CreateAndRunAssembly(
                    CONSTRUCTOR_TEST_NAMESPACE, "UnmarkedMatchingConstructor");
            });
        }
        [TestMethod]
        public void ShouldWarnIfParmeterNotInjectable()
        {
            try
            {
                (dynamic result, var diagnostics) = Utils.CreateAndRunAssembly(
                    CONSTRUCTOR_TEST_NAMESPACE, "ParameterNotInjectable");
                Assert.Fail();
            }
            catch (DIException ae)
            {
                Assert.AreEqual(1, ae.Diagnostics.Groups["MissingBean"].Occurrences.Count);
                Assert.AreEqual("abc"
                  , ((dynamic)ae.Diagnostics.Groups["MissingBean"].Occurrences[0]).MemberName);                
            }
        }
        [TestMethod]
        public void ShouldWarnIfCyclicalDependency()
        {
            try
            {
                (dynamic result, var diagnostics) = CreateAndRunAssembly(
                    CONSTRUCTOR_TEST_NAMESPACE, "CyclicalDependency");
                Assert.Fail();
            }
            catch (DIException iex)
            {
                System.Diagnostics.Debug.WriteLine(iex.Diagnostics);
                Assert.IsTrue(true);
            }
        }
        [TestMethod]
        public void ShouldWarnIfCyclicalDependencyWithNamedConstructor()
        {
            try
            {
                (dynamic result, var diagnostics) = CreateAndRunAssembly(
                    CONSTRUCTOR_TEST_NAMESPACE, "CyclicalDependencyWithNamedConstructor"
                    , usePureDiTestAssembly: false);
                Assert.Fail();
            }
            catch (DIException iex)
            {
                System.Diagnostics.Debug.WriteLine(iex.Diagnostics);
                Assert.IsTrue(true);
            }
        }
        [TestMethod]
        public void ShouldNotWarnIfConstructorsAndParamatersNotMarked()
        {
            (dynamic result, var diagnostics) = CreateAndRunAssembly(
                CONSTRUCTOR_TEST_NAMESPACE, "UnmarkedConstructorUnmarkedParameters");
            Assert.IsFalse(Falsify(diagnostics.HasWarnings));
        }
        [TestMethod]
        public void ShouldCreateTreeWithNamedRootConstructor()
        {
            (DependencyInjector pdi, Assembly assembly) =
                CreateIOCCinAssembly(CONSTRUCTOR_TEST_NAMESPACE
                , "NamedRootConstructor");
            (object bean, InjectionState injectionState) = pdi.CreateAndInjectDependencies(
              "IOCCTest.ConstructorTestData.NamedRootConstructor"
              , assemblies: new Assembly[] { assembly}
              , rootBeanSpec: new RootBeanSpec( rootConstructorName: "TestConstructor"));
            Diagnostics diagnostics = injectionState.Diagnostics;
            IResultGetter result = bean as IResultGetter;
            System.Diagnostics.Debug.WriteLine(diagnostics);
            Assert.AreEqual("stuff", result?.GetResults().Stuff);
            Assert.IsFalse(Falsify(diagnostics.HasWarnings));
        }
        // I thought we were going to find that beans created
        // via constructors overwrote each other in the mapCreatedSoFar
        // but we don't - I should have looked at the code first.
        [TestMethod]
        public void ShouldCreateTreeWithMultipleConstructorsComplex()
        {
            (DependencyInjector pdi, Assembly assembly)
              = CreateIOCCinAssembly(CONSTRUCTOR_TEST_NAMESPACE, "MultipleConstructorsComplex");
            Diagnostics diagnostics = pdi.CreateAndInjectDependencies(
              "IOCCTest.ConstructorTestData.MultipleConstructorsComplex"
              , assemblies: new Assembly[] { assembly}).injectionState.Diagnostics;

            //Assert.IsNotNull(result.GetResults()?.First.FirstParam);
            //Assert.IsNotNull(result.GetResults()?.Second.SecondParam);
            Assert.IsFalse(Falsify(diagnostics.HasWarnings));

        }
    }
}