﻿using System.Dynamic;
using PureDI;
using PureDI.Attributes;
using IOCCTest.TestCode;

namespace IOCCTest.MultipleCallsTestData
{
    [Bean]
    public class FactoryMadeWithMember : IResultGetter
    {
        [BeanReference] private Made made = null;
        public dynamic GetResults()
        {
            _ = made;
            dynamic eo = new ExpandoObject();
            eo.FurthestCtr = Furthest.InstanceCtr;
            return eo;
        }
    }
    [Bean]
    public class Made
    {
        [BeanReference(Factory = typeof(FurtherFactory))]
        private Further further = null;
        public Made()
        {
            _ = further;
        }
    }
    [Bean]
    public class FurtherFactory : IFactory
    {
        [BeanReference] private DependencyInjector injector = null;
        public (object bean, InjectionState injectionState) Execute(InjectionState injectionState, BeanFactoryArgs args)
        {
            return injector.CreateAndInjectDependencies<Further>(injectionState);
        }
    }

    [Bean]
    public class Further : IResultGetter
    {
        [BeanReference] private Furthest furthest = null;
        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Furthest = furthest;
            return eo;
        }
        
    }
    [Bean]
    internal class Furthest
    {
        public static int InstanceCtr;
        public Furthest()
        {
            InstanceCtr++;
        }
    }
}