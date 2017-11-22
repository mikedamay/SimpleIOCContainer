﻿using System;
using PureDI.Public;
using System.Collections.Immutable;
using System.Reflection;

namespace PureDI
{
    /// <summary>
    /// An object of this class lists assemblies that should be included in the injection process
    /// </summary>
    public class AssemblySpec
    {
        /// <summary>
        /// creates readonly object
        /// </summary>
        /// <param name="exclude">2 assemblies are included by default unless this parameter is specified</param>
        /// <param name="assemblies">assemblies required to be included in the injection</param>
        public AssemblySpec(AssemblyExclusion exclude 
          = AssemblyExclusion.ExcludedNone, params Assembly[] assemblies)
        {
            _explicitAssemblies = ImmutableArray.Create(assemblies);
            ExcludedAssemblies = exclude;
            this.IsEmpty = assemblies.Length == 0;
        }

        /// <summary>
        /// empty assembly spec
        /// </summary>
        private AssemblySpec()
        {
            _explicitAssemblies = ImmutableArray<Assembly>.Empty;
//            _explicitAssemblies = ImmutableArray.Create(new Assembly[] {this.GetType().Assembly});
            this.IsEmpty = true;
        }

        private static AssemblySpec _empty = new AssemblySpec();
        /// <summary>
        /// AssemblySpec.Empty serves up a reliable emtpy spec.
        /// </summary>
        public static AssemblySpec Empty => _empty;
        /// <summary>
        /// see constructor
        /// </summary>
        public AssemblyExclusion ExcludedAssemblies { get; }
        /// <summary>
        /// see constructor
        /// </summary>
        public ImmutableArray<Assembly> ExplicitAssemblies {
            get { return _explicitAssemblies; } }

        /// <summary>
        /// true indicates that the assembly spec contains no assemblies
        /// </summary>
        public Boolean IsEmpty { get; }
        private ImmutableArray<Assembly> _explicitAssemblies;
    }
}