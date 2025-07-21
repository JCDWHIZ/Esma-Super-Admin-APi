using System;

namespace Application.School.SoftDeleteSchool;

public sealed record SoftDeleteSchoolCommand(Guid PublicId) : ICommand<string>;