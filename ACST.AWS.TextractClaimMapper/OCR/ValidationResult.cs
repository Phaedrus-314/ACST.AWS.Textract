
namespace ACST.AWS.TextractClaimMapper.OCR
{
    using System;
    using ACST.AWS.Textract.Model;

    //interface IValidator<T> 
    //    where T : class
    //{
    //    bool TryValidate(T model, out List<ValidationResult> validationResults);

    //    List<ValidationResult> Validate(T model);
    //}


    public class ValidationResult
    {

        public string Name { get; set; }

        public string Group { get; set; }

        public string Result { get; set; }

        public Field Field { get; private set; }

        public ValidationResult(string name, string result)
        {
            this.Name = name;
            this.Result = result;
        }

        public ValidationResult(string name, string result, string group)
            : this(name, result)
        {
            this.Group = group;
        }

        public ValidationResult(string name, string result, string group, Field field)
            : this(name, result, group)
        {
            this.Field = field;
        }
    }
}
