namespace Application.BlogModule.DeleteBlogCommand;

public sealed record DeleteBlogCommand(Guid PublicId) : ICommand<string>;