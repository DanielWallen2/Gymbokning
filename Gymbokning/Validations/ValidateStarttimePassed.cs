using Gymbokning.Data.Migrations;
using System.ComponentModel.DataAnnotations;

namespace Gymbokning.Validations
{
    public class ValidateStarttimePassed : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var errMsg = "This time has allready passed";       // ?

            if(value is DateTime input)
            {
                var gymClass = validationContext.ObjectInstance as GymClass;

                if(gymClass != null)
                {
                    if (input > DateTime.Now)
                    {
                        return ValidationResult.Success;
                    }
                    else
                    {
                        return new ValidationResult(ErrorMessage);
                    }
                }


            }

            return new ValidationResult(ErrorMessage);
                
        }
    }
}
