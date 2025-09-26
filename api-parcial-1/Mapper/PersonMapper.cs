using api_parcial_1.Data;
using api_parcial_1.Dto;

namespace api_parcial_1.Mapper
{
    public static class PersonMapper
    {

        public static PersonDto ToResponse(Person persona)
        {
            return new PersonDto(persona.Dni, persona.FirstName, persona.LastName);
        }

        public static Person ToEntity(PersonDto request)
        {
            return new Person
            {
                Dni = request.Dni,
                FirstName = request.FirstName,
                LastName = request.LastName,
            };
        }

    }
}
