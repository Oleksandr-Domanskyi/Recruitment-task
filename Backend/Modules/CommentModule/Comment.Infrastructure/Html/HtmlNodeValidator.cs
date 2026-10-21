
using System.Xml.Linq;
using FluentValidation;
using FluentValidation.Results;

namespace Comment.Infrastructure.Html;

internal static class HtmlNodeValidator
{
    private static readonly HashSet<string> AllowedTags = new(StringComparer.Ordinal)
    {
        "a", "code", "i", "strong"
    };

    public static void Validate(XElement root)
    {
        foreach (var node in root.Nodes())
            ValidateNode(node);
    }

    private static void ValidateNode(XNode node)
    {
        if (node is XText)
            return;

        if (node is not XElement element)
            throw Invalid("Unsupported HTML content.");

        if (!AllowedTags.Contains(element.Name.LocalName) || element.Name.Namespace != XNamespace.None)
            throw Invalid($"HTML tag '{element.Name}' is not allowed.");

        ValidateAttributes(element);

        foreach (var child in element.Nodes())
            ValidateNode(child);
    }

    private static void ValidateAttributes(XElement element)
    {
        foreach (var attribute in element.Attributes())
        {
            if (element.Name.LocalName != "a" ||
                attribute.IsNamespaceDeclaration ||
                attribute.Name.Namespace != XNamespace.None ||
                attribute.Name.LocalName is not ("href" or "title"))
            {
                throw Invalid($"Attribute '{attribute.Name}' is not allowed.");
            }

            if (attribute.Name.LocalName == "href" && !HtmlUrlValidator.IsSafe(attribute.Value))
                throw Invalid("Invalid link URL.");
        }
    }

    private static ValidationException Invalid(string message)
        => new ValidationException(new[]
        {
            new ValidationFailure("Text", message)
        });
}
