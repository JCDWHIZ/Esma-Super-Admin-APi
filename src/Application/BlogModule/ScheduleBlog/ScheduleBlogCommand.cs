namespace Application.BlogModule.ScheduleBlog;

public sealed record ScheduleBlogCommand(Guid PublicId, DateTime? PublishDate) : ICommand<string>;
