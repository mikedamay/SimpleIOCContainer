﻿using System;
using PureDI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PureDI.Common;

namespace IOCCTest
{
    [TestClass]
    public class CheckArgumentsTest
    {
        //[Ignore]
        //[TestMethod]
        public void SHouldThrowExceptionIfNullAssemblies()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () =>
                {
                    var pdi = new PDependencyInjector();
                    //pdi.SetAssemblies(null);
                });

        }

        [TestMethod]
        public void ShouldThrowExceptionForNullBeanName()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () =>
                {
                    var pdi = new PDependencyInjector();
                    pdi.CreateAndInjectDependencies<CheckArgumentsTest>( rootBeanSpec: new RootBeanSpec(rootBeanName: null));
                });

        }

        [TestMethod]
        public void ShouldThrowExceptionForNullConstructorName()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () =>
                {
                    var pdi = new PDependencyInjector();
                    pdi.CreateAndInjectDependencies<CheckArgumentsTest>( rootBeanSpec: new RootBeanSpec(rootBeanName: Constants.DefaultBeanName, rootConstructorName: null)
                    );
                });

        }

        [TestMethod]
        public void ShouldThrowExceptionForNullBeanName2()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () =>
                {
                    var pdi = new PDependencyInjector();
                    pdi.CreateAndInjectDependencies<CheckArgumentsTest>(rootBeanSpec: new RootBeanSpec(rootBeanName: null));
                });

        }

        [TestMethod]
        public void ShouldThrowExceptionForNullConstructorName2()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () =>
                {
                    var pdi = new PDependencyInjector();
                    pdi.CreateAndInjectDependencies<CheckArgumentsTest>(
                      rootBeanSpec: new RootBeanSpec(rootBeanName: Constants.DefaultBeanName, rootConstructorName: null)
                    );
                });

        }

        [TestMethod]
        public void ShouldThrowExceptionForNullType()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () =>
                {
                    var pdi = new PDependencyInjector();
                    pdi.CreateAndInjectDependencies((string)null);
                }); 
            

        }

        [TestMethod]
        public void ShouldThrowExceptionForNullBeanName3()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () =>
                {
                    var pdi = new PDependencyInjector();
                    pdi.CreateAndInjectDependencies("CheckArgumentsTest", rootBeanSpec: new RootBeanSpec( rootBeanName: null));
                });

        }

        [TestMethod]
        public void ShouldThrowExceptionForNullConstructorName3()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () =>
                {
                    var pdi = new PDependencyInjector();
                    pdi.CreateAndInjectDependencies("CheckArgumentsTest", rootBeanSpec: new RootBeanSpec( rootBeanName: Constants.DefaultBeanName, rootConstructorName: null)
                    );
                });

        }
        [TestMethod]
        public void ShouldThrowExceptionForMissingType()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () =>
                {
                    var pdi = new PDependencyInjector();
                    pdi.CreateAndInjectDependencies((Type)null);
                });

        }

        [TestMethod]
        public void ShouldThrowExceptionForNullBeanNameWithTypeParam()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () =>
                {
                    var pdi = new PDependencyInjector();
                    pdi.CreateAndInjectDependencies(this.GetType(), null, new RootBeanSpec(rootBeanName: null));
                }
            );
        }
        [TestMethod]
        public void ShouldThrowExceptionForEmptyProfile()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () =>
                {
                    var pdi = new PDependencyInjector(profiles: new[] { (string)null });
                });

        }
        [TestMethod]
        public void ShouldThrowExceptionForEmptyProfile2()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () =>
                {
                    var pdi = new PDependencyInjector(profiles: new[] { "" });
                });

        }
        [TestMethod]
        public void ShouldThrowExceptionForEmptyProfile3()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () =>
                {
                    var pdi = new PDependencyInjector(profiles: new[] { " " });
                });

        }
        [TestMethod]
        public void ShouldThrowExceptionForEmptyProfile4()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () =>
                {
                    var pdi = new PDependencyInjector(profiles: new[] { "goodstuff", null });
                });

        }
    }
}