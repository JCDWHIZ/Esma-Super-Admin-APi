using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Admin.UpdateProfile;
public sealed record UpdateProfileCommand(string Username, string PhoneNumber, string Email, string ProfilePic, Guid PublicId) : ICommand<string>;
