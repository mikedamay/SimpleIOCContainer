﻿using System.Dynamic;
using com.TheDisappointedProgrammer.IOCC;

namespace IOCCTest.TestCode.WithNames
{
    public interface IResultGetter
    {
        dynamic GetResults();
    }

    [IOCCBean]
    public class DeepHierahy : IResultGetter
    {
        [IOCCBeanReference(Name="level2a")]
        private Level2a level2a = null;
        [IOCCBeanReference(Name="level2b")]
        private Level2b level2b = null;

        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Level2a = level2a;
            eo.Level2b = level2b;
            return eo;
        }
    }
    [IOCCBean(Name="level2b")]
    internal class Level2b : IResultGetter
    {
        [IOCCBeanReference(Name="level2b3a")]
        private Level2b3a level2b3a = null;
        [IOCCBeanReference(Name="level2b3b")]
        private Level2b3b level2b3b = null;
        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Level2b3a = level2b3a;
            eo.Level2b3b = level2b3b;
            return eo;
        }
    }
    
    [IOCCBean(Name="level2b3a")]
    internal class Level2b3a
    {
    }

    [IOCCBean(Name="level2b3b")]
    internal class Level2b3b
    {
    }

    [IOCCBean(Name = "level2a")]
    internal class Level2a : IResultGetter
    {
        [IOCCBeanReference(Name="level2a3a")]
        private Level2a3a level2a3a = null;
        [IOCCBeanReference(Name="level2a3b")]
        private Level2a3b level2a3b = null;
        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Level2a3a = level2a3a;
            eo.Level2a3b = level2a3b;
            return eo;
        }
    }
    [IOCCBean(Name="level2a3a")]
    internal class Level2a3a
    {
    }
    [IOCCBean(Name="level2a3b")]
    internal class Level2a3b
    {
    }

}