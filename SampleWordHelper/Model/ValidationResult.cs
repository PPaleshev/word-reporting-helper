namespace SampleWordHelper.Model
{
    public class ValidationResult
    {
        public static ValidationResult CORRECT = new ValidationResult(null, true);

        public string Message { get; private set; }
        public bool IsValid { get; private set; }

        public ValidationResult(string message, bool isValid = false)
        {
            Message = message;
            IsValid = isValid;
        }
    }
}