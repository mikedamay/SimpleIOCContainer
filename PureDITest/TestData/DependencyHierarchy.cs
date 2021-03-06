﻿using PureDI.Attributes;

// this module is not compiled as part of the project.  It is embedded as a resource.
namespace IOCCTest.TestData.DependencyHierarchy
{
    [Bean]
    public class ForstGemClassWithManyAncestors8 : SecondGenClass2 , ISecondGenWithAncestors2, ISecondGen3
    {
    }
    public class SecondGenClass2 : IThirdGen
    {
    }

    public interface ISecondGenWithAncestors2 : IThirdGenA
    {
    }

    public interface ISecondGen3 : IThirdGenB, IThirdGenC
    {
        
    }

    public interface IThirdGen
    {
        
    }
    public interface IThirdGenA
    {
        
    }
    public interface IThirdGenB
    {

    }
    public interface IThirdGenC
    {

    }

}
