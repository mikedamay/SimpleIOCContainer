﻿using System;
using com.TheDisappointedProgrammer.IOCC;
namespace SimpleIOCCDemo
{
    [Bean]
    internal class DisplayFactory : IFactory
    {
        [BeanReference] private SimpleIOCContainer iocContainer = null;
        public object Execute(BeanFactoryArgs args)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                return iocContainer.CreateAndInjectDependencies<VSOutputWindow>(beanName : "outputWindow");
            }
            else
            {
                return iocContainer.CreateAndInjectDependencies<ConsoleDisplay>();
            }
        }
    }
}