using System;

namespace Application.BlogModule.CreateBlogCommands.PublishBlog;

public sealed record PublishBlogCommand(Guid PublicId) : ICommand<string>;