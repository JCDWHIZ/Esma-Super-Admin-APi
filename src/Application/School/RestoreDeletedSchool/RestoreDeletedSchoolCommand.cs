namespace Application.School.RestoreDeletedSchool;

public sealed record RestoreDeletedSchoolCommand(Guid PublicId) : ICommand<string>;