
namespace ACST.AWS.Common.OCR
{
    public interface IMappedPropertyAttribute
    {
        string Name { get; }

        string GroupName { get; }

        bool Required { get; }
    }
}
