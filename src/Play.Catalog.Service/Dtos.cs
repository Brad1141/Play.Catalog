using System;
using System.ComponentModel.DataAnnotations;

namespace Play.Catelog.Service.Dtos
{
    public record ItemDto(Guid Id, string Name, string Description, decimal Price, DateTimeOffset CreatedDate);

    // let's do some model validation
    // Required - makes sure that the field is not null
    // Range(0, 1000) - values have to be in the range
    public record CreateItemDto([Required] string Name, string Description, [Range(0, 1000)] decimal Price);

    public record UpdateItemDto([Required] string Name, string Description, [Range(0, 1000)] decimal Price);

}

