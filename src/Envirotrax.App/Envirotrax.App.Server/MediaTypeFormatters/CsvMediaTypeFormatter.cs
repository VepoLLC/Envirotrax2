
using System.Collections;
using System.Net.Mime;
using System.Text;
using DeveloperPartners.SortingFiltering;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using Envirotrax.Common.Domain.Services.Defintions;

namespace Envirotrax.App.Server.MediaTypeFormatters
{
    public class CsvMediaTypeFormatter : TextOutputFormatter
    {
        private const string FileNameHeader = "Vp-File-Name";
        private const string ColumnsHeader = "Vp-Columns";

        private static readonly char[] _invalidFileNameChars = Path.GetInvalidFileNameChars();

        public CsvMediaTypeFormatter()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/csv"));

            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);
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

            throw new InvalidOperationException("The object type cannot be converted to CSV");
        }

        private async Task WriteBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding, string csvData)
        {
            var downloadFileNmaeHeader = context.HttpContext.Request.Headers[FileNameHeader];

            if (!string.IsNullOrEmpty(downloadFileNmaeHeader))
            {
                var cleanFileName = new string(downloadFileNmaeHeader.ToString().Select(ch => _invalidFileNameChars.Contains(ch) ? '_' : ch).ToArray());

                var contentDispositionValue = new ContentDisposition
                {
                    FileName = Path.ChangeExtension(cleanFileName, ".csv")
                };

                context.HttpContext.Response.ContentType = "application/octet-stream";
                context.HttpContext.Response.Headers.Append("Content-Disposition", contentDispositionValue.ToString());
            }

            await context.HttpContext.Response.WriteAsync(csvData, selectedEncoding);
        }

        private IDictionary<string, string> GetSelectedColumns(IHeaderDictionary headers)
        {
            // The field names may be in camelCase.
            var list = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (headers.ContainsKey(ColumnsHeader))
            {
                var parsed = QueryHelpers.ParseQuery(headers[ColumnsHeader]);

                foreach (var keyValuePair in parsed)
                {
                    var value = keyValuePair
                        .Value
                        .FirstOrDefault(v => !string.IsNullOrEmpty(v));

                    list.Add(keyValuePair.Key, value);
                }
            }

            return list;
        }

        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            if (context.Object != null)
            {
                var enumerableObject = GetEnumerableObject(context);
                var selectedColumns = GetSelectedColumns(context.HttpContext.Request.Headers);

                var csvHelper = context.HttpContext.RequestServices.GetRequiredService<ICsvHelperService>();
                var csvData = await csvHelper.WriteAsStringAsync(enumerableObject, selectedColumns);

                await WriteBodyAsync(context, selectedEncoding, csvData);
            }
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
            if (context.HttpContext.Request.Headers["Accept"].Contains("text/csv", StringComparer.OrdinalIgnoreCase))
            {
                return base.CanWriteResult(context);
            }

            return false;
        }
    }
}