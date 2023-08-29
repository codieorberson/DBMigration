using System;

namespace SSOModels.Attributes
{
    public class TruncateAttribute : Attribute
    {
        public int MaxLength { get; set; }

        public TruncateAttribute(int maxLength)
        {
            MaxLength = maxLength;
        }
    }
}
