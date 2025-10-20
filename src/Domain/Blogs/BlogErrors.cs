using SharedKernel;

namespace Domain.Blogs;

public static class BlogErrors
{
    public static Error NotFound(Guid publicId) => Error.NotFound(
        "Blog.NotFound",
        $"The blog with the PublicId = '{publicId}' was not found.");
    public static Error BlogAlreadyExists(string Title) => Error.NotFound(
        "Blog.BlogAlreadyExists",
        $"The blog with the Title = '{Title}' already exists.");

    public static readonly Error InvalidTitle = Error.Failure(
        "Blog.InvalidTitle",
        "The blog title cannot be empty or exceed 255 characters.");

    public static readonly Error InvalidContent = Error.Failure(
        "Blog.InvalidContent",
        "The blog content cannot be empty.");

    public static readonly Error AlreadyPublished = Error.Conflict(
        "Blog.AlreadyPublished",
        "The blog is already published and cannot be modified.");

    public static readonly Error CannotPublishDraft = Error.Failure(
        "Blog.CannotPublishDraft",
        "Cannot publish a blog that is in draft status without proper content validation.");

    public static readonly Error InvalidStatus = Error.Failure(
        "Blog.InvalidStatus",
        "The specified blog status is not valid.");

    public static Error InvalidPublishDate(DateTime? publishDate) => Error.Failure(
        "Blog.InvalidPublishDate",
        publishDate.HasValue
            ? $"The publish date '{publishDate.Value.ToString("o")}' is in the past or not specified."
            : "The publish date must be specified and cannot be in the past.");
}
