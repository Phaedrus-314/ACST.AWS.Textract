
namespace ACST.AWS.Common
{
    using System;

    [Serializable]
    public class RequiredPropertyException : Exception
    {
        public RequiredPropertyException()
        {

        }

        public RequiredPropertyException(string description)
            : base(description)
        {

        }

        public RequiredPropertyException(string name, string typeName, string matchedElementTypeName)
            : base($"Required property '{typeName}.{name}', not found in AWS Textract element '{matchedElementTypeName}'.")
        {

        }
    }
}
