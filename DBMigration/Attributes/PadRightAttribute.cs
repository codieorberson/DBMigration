using System;

namespace SSOModels.Attributes
{
    public class PadRightAttribute : Attribute
    {
        public int Length { get; set; }
        public char Character { get; set; }

        public PadRightAttribute(int length, char character = ' ')
        {
            Length = length;
            Character = character;
        }
    }
}
