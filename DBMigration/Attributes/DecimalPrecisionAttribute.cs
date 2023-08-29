using System;

namespace SSOModels.Attributes
{
    public class DecimalAttribute : Attribute
    {
        public int Precision { get; set; }
        public int Scale { get; set; }

        public DecimalAttribute(int precision, int scale)
        {
            Precision = precision;
            Scale = scale;
        }
    }
}
