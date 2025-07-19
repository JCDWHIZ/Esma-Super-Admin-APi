using System;

namespace Application.BlogModule.GetBlogById;

public sealed record GetBlogByIdQuery(Guid PublicId) : IQuery<BlogItemDto>;