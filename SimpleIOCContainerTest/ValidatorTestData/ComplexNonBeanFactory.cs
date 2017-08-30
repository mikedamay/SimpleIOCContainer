﻿using System;
using com.TheDisappointedProgrammer.IOCC;

namespace IOCCTest.DifficultTypeTestData
{
    [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = true)]
    public class SomeNonFactoryAttribute : Attribute
    {
        
    }
    public class ComplexNonBeanFactory : IFactory
    {
        public object Execute(BeanFactoryArgs args)
        {
            throw new System.NotImplementedException();
        }
    }
    [Bean]
    public class AWellFornedFactory : IFactory
    {
        public object Execute(BeanFactoryArgs args)
        {
            throw new System.NotImplementedException();
        }
    }

    [SomeNonFactory]
    [SomeNonFactory]
    public class ANonFatory : IFactory
    {
        public object Execute(BeanFactoryArgs args)
        {
            throw new NotImplementedException();
        }
    }
    [SomeNonFactory]
    [SomeNonFactory]
    public class ANonFatory2 : IFactory
    {
        public object Execute(BeanFactoryArgs args)
        {
            throw new NotImplementedException();
        }
    }


}