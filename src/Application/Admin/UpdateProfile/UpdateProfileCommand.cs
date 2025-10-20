namespace Application.Admin.UpdateProfile;
public sealed record UpdateProfileCommand(string Username, string PhoneNumber, string Email, string ProfilePic, Guid PublicId) : ICommand<string>;
