﻿using PureDI;

namespace IOCCTest.TestData
{
    [Bean]
    public class Generic<T>
    {
        
    }
    [Bean]
    public class GenericUser
    {
    }

    [Bean]
    public class GenericChild : Generic<int>
    {
        
    }
}
