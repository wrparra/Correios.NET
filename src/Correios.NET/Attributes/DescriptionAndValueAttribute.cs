using System;

namespace Correios.NET.Attributes
{
    /// <devdoc>
    ///    <para>Specifies a description for a property
    ///       or event.</para>
    /// </devdoc>
    [AttributeUsage(AttributeTargets.All)]
    public class DescriptionAndValueAttribute : Attribute
    {
        /// <devdoc>
        /// <para>Specifies the default value for the <see cref='System.ComponentModel.DescriptionAttribute'/> , which is an
        ///    empty string (""). This <see langword='static'/> field is read-only.</para>
        /// </devdoc>
        public static readonly DescriptionAndValueAttribute Default = new DescriptionAndValueAttribute();

        protected string description;
        protected string value;

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public DescriptionAndValueAttribute() : this(string.Empty, string.Empty)
        {
        }

        /// <devdoc>
        ///    <para>Initializes a new instance of the <see cref='System.ComponentModel.DescriptionAttribute'/> class.</para>
        /// </devdoc>
        public DescriptionAndValueAttribute(string description, string value)
        {
            this.description = description;
            this.value = value;
        }



        /// <devdoc>
        ///    <para>Gets the description stored in this attribute.</para>
        /// </devdoc>
        public virtual string Description
        {
            get
            {
                return description;
            }
        }

        /// <devdoc>
        ///    <para>Gets the description stored in this attribute.</para>
        /// </devdoc>
        public virtual string Value
        {
            get
            {
                return value;
            }
        }


        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }

            DescriptionAndValueAttribute other = obj as DescriptionAndValueAttribute;

            return (other != null) && other.Description == Description && other.Value == Value;
        }

        public override int GetHashCode()
        {
            return Description.GetHashCode() ^ Value.GetHashCode();
        }



    }

    public static class DescriptionAndValueAttributeExtentions
    {
        public static string GetAttributeValue(this Enum item)
        {

            var field = item.GetType().GetField(item.ToString());
            string attributeValue = item.ToString();

            if (field != null)
            {
                DescriptionAndValueAttribute attribute = (DescriptionAndValueAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAndValueAttribute));

                if (attribute != null)
                    attributeValue = attribute.Value;
            }

            return attributeValue;
        }

        public static string GetAttributeDescription(this Enum item)
        {

            var field = item.GetType().GetField(item.ToString());
            string attributeDescription = item.ToString();

            if (field != null)
            {
                DescriptionAndValueAttribute attribute = (DescriptionAndValueAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAndValueAttribute));

                if (attribute != null)
                    attributeDescription = attribute.Description;
            }

            return attributeDescription;
        }

    }
}