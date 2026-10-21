
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Comment.Application.Interfaces;
using FluentValidation;
using FluentValidation.Results;

namespace Comment.Infrastructure.Html;

public sealed class CommentHtmlSanitizer : ICommentHtmlSanitizer
{
    public string Sanitize(string html)
    {
        if (string.IsNullOrWhiteSpace(html))
            throw Invalid("Comment text cannot be empty.");

        try
        {
            var settings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Prohibit,
                XmlResolver = null,
                MaxCharactersInDocument = 100_000
            };

            using var reader = XmlReader.Create(new StringReader($"<root>{html}</root>"), settings);
            var root = XDocument.Load(reader).Root!;

            HtmlNodeValidator.Validate(root);

            var output = new StringBuilder();

            using (var writer = XmlWriter.Create(output, new XmlWriterSettings
            {
                ConformanceLevel = ConformanceLevel.Fragment,
                OmitXmlDeclaration = true
            }))
            {
                foreach (var node in root.Nodes())
                    node.WriteTo(writer);
            }

            return output.ToString();
        }
        catch (XmlException)
        {
            throw Invalid("Comment contains invalid XHTML markup.");
        }
    }

    private static ValidationException Invalid(string message)
        => new ValidationException(new[]
        {
            new ValidationFailure("Text", message)
        });
}
