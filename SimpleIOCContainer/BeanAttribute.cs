﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.TheDisappointedProgrammer.IOCC
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
    public class BeanAttribute : Attribute
    {
        public string Name = "";
        public string Profile = "";
        public SimpleIOCContainer.OS OS = SimpleIOCContainer.OS.Any;
    }

}