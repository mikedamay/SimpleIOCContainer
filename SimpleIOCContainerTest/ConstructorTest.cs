﻿using System.Dynamic;
using com.TheDisappointedProgrammer.IOCC;
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
            Assert.ThrowsException<IOCCException>(() =>
            {
                (NoArgRoot st, InjectionState InjectionState) = new SimpleIOCContainer().CreateAndInjectDependencies<
                    NoArgRoot>();
                IOCCDiagnostics diags = InjectionState.Diagnostics;
                Assert.IsTrue(diags.HasWarnings);
            });
        }
        [TestMethod]
        public void ShouldWarnForNoArgClassTree()
        {
            (NoArgClassTree nact, InjectionState InjectionState) = new SimpleIOCContainer().CreateAndInjectDependencies<
                NoArgClassTree>();
            IOCCDiagnostics diags = InjectionState.Diagnostics;
            Assert.IsTrue(diags.HasWarnings);
        }

        [TestMethod]
        public void ShouldInstantiateSingleObjectFromMultipleInterfaces()
        {
            ClassWithMultipleInterfaces cwmi
                = new SimpleIOCContainer().CreateAndInjectDependenciesSimple<ClassWithMultipleInterfaces>();
            Assert.IsNotNull(cwmi?.GetResults().Interface1);
            Assert.IsTrue(cwmi?.GetResults().Interface1 == cwmi?.GetResults().Interface2);
        }
        [TestMethod]
        public void ShouldCreateTreeWithSimpleConstructor()
        {
            (dynamic result, var diagnostics) = Utils.CreateAndRunAssembly(
                CONSTRUCTOR_TEST_NAMESPACE, "SimpleConstructor");
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
            Assert.ThrowsException<IOCCException>(() =>
            {
                (dynamic result, var diagnostics) = Utils.CreateAndRunAssembly(
                    CONSTRUCTOR_TEST_NAMESPACE, "DuplicateConstructors");
            });
        }
        [TestMethod]
        public void ShouldWarnIfSomeParametersAreNotMarkedForInjetion()
        {
            Assert.ThrowsException<IOCCException>(() =>
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
            Assert.ThrowsException<IOCCException>(() =>
            {
                (dynamic result, var diagnostics) = Utils.CreateAndRunAssembly(
                    CONSTRUCTOR_TEST_NAMESPACE, "UnmarkedMatchingConstructor");
            });
        }
        [TestMethod]
        public void ShouldWarnIfParmeterNotInjectable()
        {
            // TODO this should really be an exception
            // TODO we con't seem to have the equivalent for injected members
            (dynamic result, var diagnostics) = Utils.CreateAndRunAssembly(
                CONSTRUCTOR_TEST_NAMESPACE, "ParameterNotInjectable");
            Assert.IsTrue(diagnostics.HasWarnings);
            Assert.AreEqual(1, diagnostics.Groups["MissingBean"].Occurrences.Count);
            Assert.AreEqual("abc"
              , ((dynamic)diagnostics.Groups["MissingBean"].Occurrences[0]).MemberName);
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
            catch (IOCCException iex)
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
            SimpleIOCContainer sic =
                CreateIOCCinAssembly(CONSTRUCTOR_TEST_NAMESPACE
                , "NamedRootConstructor");
            var result = sic.CreateAndInjectDependenciesWithString(
              "IOCCTest.ConstructorTestData.NamedRootConstructor"
              , out var diagnostics, rootConstructorName : "TestConstructor") as IResultGetter;
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
            SimpleIOCContainer sic
              = CreateIOCCinAssembly(CONSTRUCTOR_TEST_NAMESPACE, "MultipleConstructorsComplex");
            IResultGetter result = sic.CreateAndInjectDependenciesWithString(
              "IOCCTest.ConstructorTestData.MultipleConstructorsComplex"
                , out var diagnostics) as IResultGetter;
            //Assert.IsNotNull(result.GetResults()?.First.FirstParam);
            //Assert.IsNotNull(result.GetResults()?.Second.SecondParam);
            Assert.IsFalse(Falsify(diagnostics.HasWarnings));

        }
    }
}