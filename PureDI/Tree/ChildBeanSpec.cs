using System.Reflection;

namespace PureDI.Tree
{
    /// <summary>
    /// objects of this type are instantiated when the beans that will be used as
    /// members and constructors are being created
    /// and used when the object is assigned to the member or passed as an
    /// argument to the constructor
    /// </summary>
    internal class ChildBeanSpec
    {
        public MemberInfo FieldOrPropertyInfo
        {
            get { return VariableInfo.FieldOrPropertyInfo; }
        }

        public ParameterInfo ParameterInfo
        {
            get { return VariableInfo.ParameterInfo; }
        }
        public object MemberOrFactoryBean { get; }
        public bool IsFactory { get; }
        private VariableInfo VariableInfo { get; }

        /// <param name="fieldOrPropertyVariableInfo">describes the member variable or constructor parameter
        /// to which this bean will be assigned.  The object wraps the member or 
        /// parameter info and provides a common interface for many operations where members and parameters
        /// are treated identically.</param>
        /// <param name="memberOrFactoryBean">an instance of the bean to be assigned to the member
        /// or parameter or the factory which will produce the bean.</param>
        /// <param name="isFactory">true if the member variable or constructor parameter is to be
        /// assigned as the result of a factory operation.</param>
        public ChildBeanSpec(VariableInfo fieldOrPropertyVariableInfo, object memberOrFactoryBean, bool isFactory)
        {
            this.VariableInfo = fieldOrPropertyVariableInfo;
            this.MemberOrFactoryBean = memberOrFactoryBean;
            this.IsFactory = isFactory;
        }
    }
}