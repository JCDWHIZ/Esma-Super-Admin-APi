namespace Application.School.GetSchoolById;


public sealed record GetSchoolByIdQuery(Guid PublicId) : IQuery<SchoolItemDto>;
