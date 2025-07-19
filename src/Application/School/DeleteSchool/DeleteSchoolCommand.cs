using System;

namespace Application.School.DeleteSchool;

public sealed record DeleteSchoolCommand(Guid PublicId) : ICommand<string>;
