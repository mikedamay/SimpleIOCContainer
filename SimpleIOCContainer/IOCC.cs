﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;

namespace com.TheDisappointedProgrammer.IOCC
{
    // TODO warning when a field or property has already been initialised
    // DONE guard against circular references - Done
    // DONE handle structs
    // DONE handle properties
    // TODO object factories
    // DONE handle multiple assemblies - Done
    // TODO references held in tuples
    // TODO references held in embedded structs 
    // TODO references held as objects
    // TODO references to arrays
    // TODO use fully qualified type names in comparisons
    // TODO use immutable collections
    // TODO detect duplicate type, name, profile, os combos (ensure any are compared to specific os and profile)
    // TODO handle or document generic classes
    // TODO handle dynamic types
    // TODO change "dependency" to "bean"
    // TODO check arguments' validity for externally facing methods
    // DONE unit test to validate XML - Done
    // TODO run code analysis
    // TODO make Diagnostics constructor private
    // TODO improve names of queries assigned from complex linq structures
    // TODO use immutable collections
    // TODO license
    // TODO An optional name should be passed to IOCC.GetOrCreateDependencyTree
    // TODO address static fields and beans.  Beans are invalid or maybe not
    // TODO readonly fields
    // TODO look at MEF implementations - heard on dnr 8-8-17
    // TODO change text on ReadOnlyProperty to mention that this can be set by using the constructor
    // TODO suppress code analysis messages
    // TODO change wording of no-arg constructor diagnostic to include constructor based injections
    // TODO document / investigate other classes derived from ValueType
    // TODo ensure there is a test that uses an object multiple times in the tree.
    // TODO document the fact that member type is based on the type's GetIOCCName() attribute
    // TODO and that generics have the for classname`1[TypeParam]
    // TODO move the majority of unit tests to separate assemblies
    // TODO test generics with multiple parameters
    // TODO test generics with nested parameters
    // TODO ensure where interface->base class->derived class occurs there is no problem with duplication of beans
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// constraints:
    ///     1) the object tree (i.e. the program's static model) is required to be static.
    ///     if objects are added to the tree through code at run-time this will not be 
    ///     reflected in the IOC container.
    ///     2) The root class has to be visible to the caller of GetOrCreateObjectTree.
    ///     2a) The root of the tree cannot be specified using reflection.  I'll probably regret that.
    ///     3) static classes and members are not handled.
    ///     4) If a member is incorrectly marked as [IOCCInjectedDependency] then
    ///        it will be set to its default value even if it is an initialized member.
    /// </remarks>
    public class IOCC
    {
        public enum OS { Any, Linux, Windows, MacOS } OS os = new StdOSDetector().DetectOS();
        public static IOCC Instance { get; } = new IOCC();
        internal const string DEFAULT_PROFILE = "";
        internal const string DEFAULT_DEPENDENCY_NAME = "";

        private bool getOrCreateObjectTreeCalled = false;
        private IList<string> assemblyNames = new List<string>();
        private readonly IDictionary<string, IOCObjectTreeContainer> mapObjectTreeContainers 
          = new Dictionary<string, IOCObjectTreeContainer>();

        private IDictionary<(Type, string), Type> typeMap;

        private class AssemblyNameComparer : IEqualityComparer<string>
        {
            public bool Equals(string x, string y)
            {
                return x.Equals(y, StringComparison.OrdinalIgnoreCase);
            }

            public int GetHashCode(string obj)
            {
                return base.GetHashCode();
            }
        }
        /// <summary>
        /// for testing only
        /// </summary>
        internal IOCC()
        {
        }
        /// <example>SetAssemblies("MyApp", "MyLib")</example>
        public void SetAssemblies(params string[] assemblyNames)
        {
            if (getOrCreateObjectTreeCalled)
            {
                throw new InvalidOperationException(
                  "SetAssemblies has been called after GetOrCreateObjectTree."
                  + "  This is not permitted.");
            }
            this.assemblyNames = assemblyNames.ToList();
        }
        // TODO complete the documentation item 3 below if and when factory types are implemented
        // TODO handle situation where there is no console window
        /// <summary>
        /// 1. mainly used to create the complete object tree at program startup
        /// 2. may be used to create object tree fragments when running tests
        /// 3. may be used to create an object or link to an existing object
        /// </summary>
        /// <typeparam name="TRootType">The concrete class (not an interface) of the top object in the tree</typeparam>
        /// <returns>an ojbect of root type</returns>
        public TRootType GetOrCreateObjectTree<TRootType>(string profile = DEFAULT_PROFILE)
        {
            IOCCDiagnostics diagnostics = null;
            TRootType rootObject = default(TRootType);
            try
            {
                diagnostics = new DiagnosticBuilder().Diagnostics;
                rootObject = GetOrCreateObjectTreeEx<TRootType>(ref diagnostics, profile);
            }
            finally
            {
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    System.Diagnostics.Debug.Write(diagnostics);
                }
                else
                {
                    Console.WriteLine(diagnostics);    
                }
                 
            }
            return rootObject;
        }

        public TRootType GetOrCreateObjectTree<TRootType>(out IOCCDiagnostics diagnostics
            , string profile = DEFAULT_PROFILE, string rootBeanName = DEFAULT_DEPENDENCY_NAME )
        {
            diagnostics = new DiagnosticBuilder().Diagnostics;
            return GetOrCreateObjectTreeEx<TRootType>(ref diagnostics, profile, rootBeanName);
        }
        /// <summary>
        /// <see cref="GetOrCreateObjectTree"/>
        /// this overload does not print out the diagnostics
        /// </summary>
        /// <param name="diagnostics">This overload exposes the diagnostics object to the caller</param>
        private TRootType GetOrCreateObjectTreeEx<TRootType>(ref IOCCDiagnostics diagnostics
            , string profile = DEFAULT_PROFILE, string rootBeanName = DEFAULT_DEPENDENCY_NAME)
        {
            getOrCreateObjectTreeCalled = true;
            IList<Assembly> assemblies = AssembleAssemblies(assemblyNames, typeof(TRootType).Assembly, ref diagnostics);
            typeMap = new TypeMapBuilder().BuildTypeMapFromAssemblies(assemblies
              , ref diagnostics, profile, os);
            IOCObjectTreeContainer container;
            if (mapObjectTreeContainers.ContainsKey(profile))
            {
                container = mapObjectTreeContainers[profile];
            }
            else
            {
                container = new IOCObjectTreeContainer(profile, typeMap);
            }
            var rootObject = container.GetOrCreateObjectTree<TRootType>(ref diagnostics, rootBeanName);
            if (rootObject == null && diagnostics.HasWarnings)
            {
                throw new IOCCException("Failed to create object tree - see diagnostics for details", diagnostics);
            }
            return rootObject;
        }

        /// <summary>
        /// builds list of all the assemblies involved in the dependency tree
        /// </summary>
        /// <param name="assemblyNames">list of names provided by caller.
        /// This should include the names of all assemblies containing dependencies (concrete classes)
        /// involved in the object model of this app.  Optionally this can include the assembly
        /// of the root class in the tree</param>
        /// <param name="rootAssembly">The assembly which contains the root class</param>
        /// <returns></returns>
        private IList<Assembly> AssembleAssemblies(IList<string> assemblyNames
          , Assembly rootAssembly, ref IOCCDiagnostics diagnostics)
        {
            ISet<string> assemblyNamesSet = new HashSet<string>(assemblyNames, new AssemblyNameComparer());
            ISet<Assembly> assembliesSet = new HashSet<Assembly>();
            assembliesSet.Add(rootAssembly);
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Console.WriteLine(assembly.GetName().Name);
                if (assemblyNamesSet.Contains(assembly.GetName().Name))
                {
                    assembliesSet.Add(assembly);
                }
            }
            return assembliesSet.ToList();
        }
    }
}
