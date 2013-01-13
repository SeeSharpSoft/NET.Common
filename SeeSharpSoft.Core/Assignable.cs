using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.Serialization;
using System.IO;
using System.ComponentModel;
using System.Xml.Serialization;

namespace SeeSharpSoft
{
    /// <summary>
    /// Mark a property as assignable, or remove assignable attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple=false, Inherited=true)]
    public class AssignableAttribute : Attribute
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public AssignableAttribute() : base() { }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="assignable">False to disable assign-ability for inherited properties, true default.</param>
        public AssignableAttribute(bool assignable) : this() { Assignable = assignable; }

        private bool _assignable = true;
        /// <summary>
        /// Get or set whether marked class/property is assignable.
        /// </summary>
        public bool Assignable
        {
            get { return _assignable; }
            set { _assignable = value; }
        }

        /// <summary>
        /// Gets or sets property conditions for assigning the property.
        /// </summary>
        public AssignableCondition Condition { get; set; }

        /// <summary>
        /// Assign all assignable properties from source to target.
        /// </summary>
        /// <param name="target">Object to set properties.</param>
        /// <param name="source">Object to assign.</param>
        internal void Assign(object target, object source)
        {
            if (ReferenceEquals(target, source) || source == null) return;

            Type targetType = target.GetType();

            foreach (PropertyDescriptor propInfo in TypeDescriptor.GetProperties(target))
            {
                AssignableAttribute assignableAttribute = propInfo.Attributes.OfType<AssignableAttribute>().FirstOrDefault();

                if (assignableAttribute == null) assignableAttribute = this;

                assignableAttribute.Assign(source, target, propInfo);
            }
        }

        /// <summary>
        /// Assign value from source to target.
        /// </summary>
        /// <param name="source">Source object to get value from.</param>
        /// <param name="target">Target object to set value to.</param>
        /// <param name="targetProperty">Property to set value to.</param>
        internal void Assign(object source, object target, PropertyDescriptor targetProperty)
        {
            if (!Assignable || target == null || source == null || targetProperty == null) return;

            Assign(source, TypeDescriptor.GetProperties(source).OfType<PropertyDescriptor>().FirstOrDefault(elem => elem.Name.Equals(targetProperty.Name)) , target, targetProperty);
        }

        /// <summary>
        /// Assign value from source to target.
        /// </summary>
        /// <param name="source">Source object to get value from.</param>
        /// <param name="sourceProperty">Property to get value from.</param>
        /// <param name="target">Target object to set value to.</param>
        /// <param name="targetProperty">Property to set value to.</param>
        protected virtual void Assign(object source, PropertyDescriptor sourceProperty, object target, PropertyDescriptor targetProperty)
        {
            if (!CanAssign(source, sourceProperty, target, targetProperty)) return;

            object value = sourceProperty.GetValue(source);

            targetProperty.SetValue(target, value);
        }

        protected virtual bool CanAssign(object source, PropertyDescriptor sourceProperty, object target, PropertyDescriptor targetProperty)
        {
            if (!Assignable || ReferenceEquals(source, target) || sourceProperty == null || targetProperty == null || targetProperty.IsReadOnly || sourceProperty.PropertyType != targetProperty.PropertyType) return false;

            if ((Condition & AssignableCondition.ShouldSerializeValue) == AssignableCondition.ShouldSerializeValue && !sourceProperty.ShouldSerializeValue(source)) return false;

            return true;
        }
    }

    [Flags]
    public enum AssignableCondition
    {
        None = 0,
        ShouldSerializeValue = 1
    }

    /// <summary>
    /// Methods for IAssignable objects.
    /// </summary>
    public static class AssignableExtensions
    {
        /// <summary>
        /// Assign all assignable properties from source to target.
        /// </summary>
        /// <param name="target">Object to set properties.</param>
        /// <param name="source">Object to assign.</param>
        public static void Assign(this object target, object source)
        {
            if (ReferenceEquals(target, source) || source == null) return;

            Type targetType = target.GetType();

            AssignableAttribute classAssignableAttribute = TypeDescriptor.GetAttributes(targetType).OfType<AssignableAttribute>().FirstOrDefault();

            if (classAssignableAttribute != null)
            {
                classAssignableAttribute.Assign(target, source);
            }
            else
            {
                foreach (PropertyDescriptor propInfo in TypeDescriptor.GetProperties(target))
                {
                    AssignableAttribute assignableAttribute = propInfo.Attributes.OfType<AssignableAttribute>().FirstOrDefault();

                    if (assignableAttribute == null) continue;

                    assignableAttribute.Assign(source, target, propInfo);
                }
            }
        }

        /// <summary>
        /// Creates a copy of the object with respect to <code>Assignable</code> properties.
        /// </summary>
        /// <param name="source">Object to create copy of.</param>
        /// <param name="constructorParams">Parameters for constructor of the copy.</param>
        /// <returns></returns>
        public static T AssignToCopy<T>(this T source, params object[] constructorParams) where T : class, new()
        {
            T result = (T)Activator.CreateInstance(source.GetType(), constructorParams);
            result.Assign(source);
            return result;
        }

        /// <summary>
        /// Serializes properties to writer.
        /// </summary>
        /// <param name="source">Object to store.</param>
        /// <param name="writer">Target writer.</param>
        public static void AssignToXml<T>(this T source, TextWriter writer)
        {
            XmlSerializer serializer = new XmlSerializer(source.GetType());
            serializer.Serialize(writer, source);
        }

        /// <summary>
        /// Deserializes properties from writer and assign all assignable properties.
        /// </summary>
        /// <remarks>Requires default constructor.</remarks>
        /// <param name="source">Object to restore.</param>
        /// <param name="reader">Source reader.</param>
        public static void AssignFromXml(this object source, TextReader reader)
        {
            XmlSerializer serializer = new XmlSerializer(source.GetType());
            object restore = serializer.Deserialize(reader);
            source.Assign(restore);
        }
    }

    
}