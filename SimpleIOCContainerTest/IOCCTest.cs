﻿using System;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WithNames = IOCCTest.TestCode.WithNames;

namespace IOCCTest
{
    [TestClass]
    public class IOCCTest
    {
        [TestMethod]
        public void SelfTest()
        {
            SimpleIOCContainer iocc = new SimpleIOCContainer();
            iocc.SetAssemblies("mscorlib", "System", "SimpleIOCContainerTest");
            TestRoot twf 
              = iocc.GetOrCreateObjectTree<TestRoot>();
            Assert.IsNotNull(twf.test);
        }
        [TestMethod]
        public void ShouldHaveRootClassWithNoArgConstructor()
        {
            void DoTest()
            {
                new SimpleIOCContainer().GetOrCreateObjectTree<int>();
            }
            Assert.ThrowsException<IOCCException>((System.Action)DoTest);
        }
        enum Mike { Mike1}
        public void ShouldNotTreatEnumAsClass()
        {
            void DoTest()
            {
                
                new SimpleIOCContainer().GetOrCreateObjectTree<Mike>();
            }
            Assert.ThrowsException<IOCCException>((System.Action)DoTest);
        }
        [TestMethod]
        public void ShouldInjectIntoDeepHierarchy()
        {
            DeepHierahy root = SimpleIOCContainer.Instance.GetOrCreateObjectTree<DeepHierahy>();
            Assert.IsNotNull(root);
            Assert.IsNotNull(root?.GetResults().Level2a);
            Assert.IsNotNull(root?.GetResults().Level2b);
            Assert.IsNotNull(root?.GetResults().Level2a?.GetResults().Level2a3a);
            Assert.IsNotNull(root?.GetResults().Level2a?.GetResults().Level2a3b);
            Assert.IsNotNull(root?.GetResults().Level2b?.GetResults().Level2b3a);
            Assert.IsNotNull(root?.GetResults().Level2b?.GetResults().Level2b3b);
        }
       
        [TestMethod]
        public void ShouldBuildTreeWithSelfReferentialClass()
        {
            SelfReferring sr = SimpleIOCContainer.Instance.GetOrCreateObjectTree<SelfReferring>();
            Assert.IsNotNull(sr);
        }
        [TestMethod, Timeout(100)]
        public void ShouldWorkWithCyclicalDependencies()
        {
            try
            {
                // this should not run forever
                CyclicalDependency cd = SimpleIOCContainer.Instance.GetOrCreateObjectTree<CyclicalDependency>();
                Assert.IsNotNull(cd);
                Assert.IsNotNull(cd?.GetResults().Child);
                Assert.IsNotNull(cd?.GetResults().Child?.GetResults().Parent);
                Assert.IsNotNull(cd?.GetResults().Child?.GetResults().GrandChild);
                Assert.IsNotNull(cd?.GetResults().Child?.GetResults().GrandChild?.GetResults().GrandParent);
            }
            catch (StackOverflowException soe)
            {
                Assert.Fail("The stack overflowed indicating cyclical dependencies");
            }
        }
        [TestMethod, Timeout(100)]
        public void ShouldWorkWithCyclicalInterfaces()
        {
            ParentWithInterface cd 
              = SimpleIOCContainer.Instance.GetOrCreateObjectTree<ParentWithInterface>();
            Assert.IsNotNull(cd);
            Assert.IsNotNull(cd.GetResults().IChild);
            Assert.IsNotNull(cd.GetResults().IChild?.GetResults().IParent);
        }
        [TestMethod, Timeout(100)]
        public void ShouldCreateTreeForCyclicalBaseClasses()
        {
            BaseClass cd 
              = SimpleIOCContainer.Instance.GetOrCreateObjectTree<BaseClass>();
            Assert.IsNotNull(cd);
            Assert.IsNotNull(cd?.GetResults().ChildClass);
            Assert.IsNotNull(cd?.GetResults().ChildClass?.GetResults().BasestClass);
        }
        [TestMethod]
        public void ShouldInjectIntoDeepHierarchyWithNames()
        {
            WithNames.DeepHierahy root = SimpleIOCContainer.Instance.GetOrCreateObjectTree<WithNames.DeepHierahy>();
            Assert.IsNotNull(root);
            Assert.IsNotNull(root?.GetResults().Level2a);
            Assert.IsNotNull(root?.GetResults().Level2b);
            Assert.IsNotNull(root?.GetResults().Level2a?.GetResults().Level2a3a);
            Assert.IsNotNull(root?.GetResults().Level2a?.GetResults().Level2a3b);
            Assert.IsNotNull(root?.GetResults().Level2b?.GetResults().Level2b3a);
            Assert.IsNotNull(root?.GetResults().Level2b?.GetResults().Level2b3b);
        }
        [TestMethod, Timeout(100)]
        public void ShouldCreateTreeForBeansWithNames()
        {
            WithNames.CyclicalDependency cd 
              = SimpleIOCContainer.Instance.GetOrCreateObjectTree<
                    WithNames.CyclicalDependency>(out IOCCDiagnostics diags, SimpleIOCContainer.DEFAULT_PROFILE, "name-A");
            Assert.IsNotNull(cd);
            Assert.IsNotNull(cd?.GetResults().Child);
            Assert.IsNotNull(cd?.GetResults().Child?.GetResults().Parent);
            Assert.IsNotNull(cd?.GetResults().Child?.GetResults().GrandChild);
            Assert.IsNotNull(cd?.GetResults().Child?.GetResults().GrandChild?.GetResults().GrandParent);
        }
        [TestMethod, Timeout(100)]
        public void ShouldWorkWithCyclicalInterfacesWithNames()
        {
            WithNames.ParentWithInterface cd
                = SimpleIOCContainer.Instance.GetOrCreateObjectTree<WithNames.ParentWithInterface>(out IOCCDiagnostics diags, SimpleIOCContainer.DEFAULT_PROFILE, "name-B");
            Assert.IsNotNull(cd);
            Assert.IsNotNull(cd.GetResults().IChild);
            Assert.AreEqual("name-B", cd.GetResults().IChild?.GetResults().IParent?.GetResults().Name);
            Assert.AreEqual("name-B2", cd.GetResults().IChild?.GetResults().IParent2?.GetResults().Name);
        }
        [TestMethod, Timeout(100)]
        public void ShouldCreateTreeForCyclicalBaseClassesWithNames()
        {
            WithNames.BaseClass cd
                = SimpleIOCContainer.Instance.GetOrCreateObjectTree<WithNames.BaseClass>(out IOCCDiagnostics diags, SimpleIOCContainer.DEFAULT_PROFILE, "basest");
            Assert.IsNotNull(cd);
            Assert.IsNotNull(cd?.GetResults().ChildClass);
            Assert.AreEqual("basest", cd?.GetResults().ChildClass?.GetResults().BasestClass?.GetResults().Name);
        }
    }
    [IOCCBean]
    internal class TestRoot
    {
#pragma warning disable 649
        [IOCCBeanReference]
        public ITest test;
    }

    interface ITest
    {
        
    }

    [IOCCBean]
    class Test : ITest
    {
        
    }
}
