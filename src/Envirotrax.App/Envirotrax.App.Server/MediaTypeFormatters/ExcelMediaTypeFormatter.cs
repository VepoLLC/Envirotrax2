
using System.Collections;
using System.Net.Mime;
using System.Reflection;
using ClosedXML.Excel;
using DeveloperPartners.SortingFiltering;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;

namespace Envirotrax.App.Server.MediaTypeFormatters
{
    public class ExcelMediaTypeFormatter : OutputFormatter
    {
        private const string FileNameHeader = "Vp-File-Name";
        private const string ColumnsHeader = "Vp-Columns";

        private static readonly char[] _invalidFileNameChars = Path.GetInvalidFileNameChars();

        public ExcelMediaTypeFormatter()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(MimeTypes.Excel));
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

            throw new InvalidOperationException("The object type cannot be converted to Excel");
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

        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context)
        {
            if (context.Object == null)
            {
                return;
            }

            var enumerableObject = GetEnumerableObject(context);
            var selectedColumns = GetSelectedColumns(context.HttpContext.Request.Headers);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Data");

            var columnKeys = selectedColumns.Keys.ToList();
            var columnHeaders = selectedColumns.Values.ToList();

            for (var i = 0; i < columnHeaders.Count; i++)
            {
                worksheet.Cell(1, i + 1).Value = columnHeaders[i];
            }

            if (columnHeaders.Count > 0)
            {
                var headerRange = worksheet.Range(1, 1, 1, columnHeaders.Count);
                headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#4682B4");
                headerRange.Style.Font.FontColor = XLColor.White;
            }

            var row = 2;
            if (enumerableObject != null)
            {
                foreach (var record in enumerableObject)
                {
                    for (var col = 0; col < columnKeys.Count; col++)
                    {
                        var value = GetNestedValue(record, columnKeys[col]);
                        worksheet.Cell(row, col + 1).Value = value != null ? XLCellValue.FromObject(value) : Blank.Value;
                    }

                    if (row % 2 == 1 && columnKeys.Count > 0)
                    {
                        worksheet.Range(row, 1, row, columnKeys.Count).Style.Fill.BackgroundColor = XLColor.FromHtml("#E6E6E6");
                    }

                    row++;
                }
            }

            using var memoryStream = new MemoryStream();
            workbook.SaveAs(memoryStream);
            memoryStream.Position = 0;

            var downloadFileNameHeader = context.HttpContext.Request.Headers[FileNameHeader];

            if (!string.IsNullOrEmpty(downloadFileNameHeader))
            {
                var cleanFileName = new string(downloadFileNameHeader.ToString().Select(ch => _invalidFileNameChars.Contains(ch) ? '_' : ch).ToArray());

                var contentDispositionValue = new ContentDisposition
                {
                    FileName = Path.ChangeExtension(cleanFileName, ".xlsx")
                };

                context.HttpContext.Response.ContentType = MimeTypes.Excel;
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
            if (context.HttpContext.Request.Headers["Accept"].Contains(MimeTypes.Excel, StringComparer.OrdinalIgnoreCase))
            {
                return base.CanWriteResult(context);
            }

            return false;
        }
    }
}
