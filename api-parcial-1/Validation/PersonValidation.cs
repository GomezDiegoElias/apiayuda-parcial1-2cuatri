using api_parcial_1.Dto;
using FluentValidation;

namespace api_parcial_1.Validation
{
    public class PersonValidation : AbstractValidator<PersonDto>
    {
        public PersonValidation()
        {
            RuleFor(p => p.Dni)
                .NotEmpty().WithMessage("El DNI es obligatorio");
            RuleFor(p => p.FirstName)
                .NotEmpty().WithMessage("El nombre es obligatorio")
                .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres");
            RuleFor(p => p.LastName)
                .MaximumLength(50).WithMessage("El apellido no puede exceder los 50 caracteres");
        }
    }
}
