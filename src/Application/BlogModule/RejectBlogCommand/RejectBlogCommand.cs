namespace Application.BlogModule.RejectBlogCommand;
public sealed record RejectBlogCommand(Guid PublicId, string RejectReason) : ICommand<string>;
