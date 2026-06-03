
using System.Collections;
using System.Net.Mime;
using System.Reflection;
using System.Xml;
using DeveloperPartners.SortingFiltering;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;

namespace Envirotrax.App.Server.MediaTypeFormatters
{
    public class XmlMediaTypeFormatter : OutputFormatter
    {
        private const string FileNameHeader = "Vp-File-Name";
        private const string ColumnsHeader = "Vp-Columns";
        private const string XmlMimeType = "application/xml";

        private static readonly char[] _invalidFileNameChars = Path.GetInvalidFileNameChars();

        public XmlMediaTypeFormatter()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(XmlMimeType));
        }

        private IEnumerable? GetEnumerableObject(OutputFormatterWriteContext context)
        {
            if (typeof(IEnumerable).IsAssignableFrom(context.ObjectType))
            {
                return context.Object as IEnumerable;
            }

            if (context.ObjectType != null && IsPagedData(context.ObjectType))
            {
                var dataProperty = context.ObjectType.GetProperty(nameof(IPagedData<object>.Data))!;
                return dataProperty.GetValue(context.Object) as IEnumerable;
            }

            throw new InvalidOperationException("The object type cannot be converted to XML");
        }

        private IDictionary<string, string> GetSelectedColumns(IHeaderDictionary headers)
        {
            var list = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (headers.ContainsKey(ColumnsHeader))
            {
                var parsed = QueryHelpers.ParseQuery(headers[ColumnsHeader]);

                foreach (var keyValuePair in parsed)
                {
                    var value = keyValuePair
                        .Value
                        .FirstOrDefault(v => !string.IsNullOrEmpty(v));

                    list.Add(keyValuePair.Key, value ?? keyValuePair.Key);
                }
            }

            return list;
        }

        private static object? GetNestedValue(object record, string dotPath)
        {
            var parts = dotPath.Split('.');
            object? current = record;

            foreach (var part in parts)
            {
                if (current == null) return null;

                var prop = current.GetType().GetProperty(part, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                current = prop?.GetValue(current);
            }

            return current;
        }

        private static string ToXmlElementName(string name)
        {
            if (string.IsNullOrEmpty(name)) return "Field";

            var chars = name.Select((ch, i) =>
            {
                if (char.IsLetter(ch) || ch == '_' || (i > 0 && (char.IsDigit(ch) || ch == '-' || ch == '.')))
                    return ch;
                if (char.IsWhiteSpace(ch) || ch == '/')
                    return '_';
                return '_';
            }).ToArray();

            var result = new string(chars);

            if (!char.IsLetter(result[0]) && result[0] != '_')
                result = "_" + result;

            return result;
        }

        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context)
        {
            if (context.Object == null)
            {
                return;
            }

            var enumerableObject = GetEnumerableObject(context);
            var selectedColumns = GetSelectedColumns(context.HttpContext.Request.Headers);

            var columnKeys = selectedColumns.Keys.ToList();
            var columnCaptions = selectedColumns.Values.ToList();

            using var memoryStream = new MemoryStream();

            var settings = new XmlWriterSettings
            {
                Async = true,
                Indent = true
            };

            await using (var writer = XmlWriter.Create(memoryStream, settings))
            {
                await writer.WriteStartDocumentAsync();
                await writer.WriteStartElementAsync(null, "Recordset", null);

                if (enumerableObject != null)
                {
                    foreach (var record in enumerableObject)
                    {
                        await writer.WriteStartElementAsync(null, "Record", null);

                        for (var col = 0; col < columnKeys.Count; col++)
                        {
                            var elementName = ToXmlElementName(columnCaptions[col]);
                            var value = GetNestedValue(record, columnKeys[col]);

                            await writer.WriteElementStringAsync(null, elementName, null, value?.ToString() ?? string.Empty);
                        }

                        await writer.WriteEndElementAsync();
                    }
                }

                await writer.WriteEndElementAsync();
                await writer.WriteEndDocumentAsync();
            }

            memoryStream.Position = 0;

            var downloadFileNameHeader = context.HttpContext.Request.Headers[FileNameHeader];

            if (!string.IsNullOrEmpty(downloadFileNameHeader))
            {
                var cleanFileName = new string(downloadFileNameHeader.ToString().Select(ch => _invalidFileNameChars.Contains(ch) ? '_' : ch).ToArray());

                var contentDispositionValue = new ContentDisposition
                {
                    FileName = Path.ChangeExtension(cleanFileName, ".xml")
                };

                context.HttpContext.Response.ContentType = XmlMimeType;
                context.HttpContext.Response.Headers.Append("Content-Disposition", contentDispositionValue.ToString());
            }

            await memoryStream.CopyToAsync(context.HttpContext.Response.Body);
        }

        private bool IsPagedData(Type type)
        {
            return type
                .GetInterfaces()
                .Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IPagedData<>));
        }

        protected override bool CanWriteType(Type? type)
        {
            if (type != null)
            {
                if (typeof(IEnumerable).IsAssignableFrom(type))
                {
                    return true;
                }

                if (IsPagedData(type))
                {
                    return true;
                }
            }

            return false;
        }

        public override bool CanWriteResult(OutputFormatterCanWriteContext context)
        {
            if (context.HttpContext.Request.Headers["Accept"].Contains(XmlMimeType, StringComparer.OrdinalIgnoreCase))
            {
                return base.CanWriteResult(context);
            }

            return false;
        }
    }
}
