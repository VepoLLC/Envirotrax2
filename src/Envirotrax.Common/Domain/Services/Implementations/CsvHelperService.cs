
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using Envirotrax.Common.Domain.Services.Defintions;

namespace Envirotrax.Common.Domain.Services.Implementations
{
    public class CsvHelperService : ICsvHelperService
    {
        private Type? GetListElementType(IEnumerable records)
        {
            var listType = records.GetType();

            return listType.IsArray
                ? listType.GetElementType()
                : listType.GetGenericArguments().LastOrDefault();
        }

        private ClassMap CreateClassMap(Type mapType)
        {
            var genericClass = typeof(DefaultClassMap<>);
            var constructedClass = genericClass.MakeGenericType(mapType);

            return (ClassMap)Activator.CreateInstance(constructedClass)!;
        }

        private void MapProperty(CsvWriter csvWriter, Queue<string> queue, Type elementType, ClassMap classMap, string headerName, int index)
        {
            while (queue.TryDequeue(out var propertyName))
            {
                var property = elementType.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (property != null)
                {
                    if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
                    {
                        var nestedMap = CreateClassMap(property.PropertyType);
                        var referenceMap = classMap.References(nestedMap.GetType(), property);

                        MapProperty(csvWriter, queue, property.PropertyType, referenceMap.Data.Mapping, headerName, index);
                    }
                    else
                    {
                        classMap
                            .Map(elementType, property)
                            .Index(index)
                            .Name(headerName);
                    }

                    csvWriter.Context.RegisterClassMap(classMap);
                }
            }
        }

        private void SetSelectedColumnMapping(CsvWriter csvWriter, IEnumerable records, IDictionary<string, string> selectedColumns)
        {
            if (selectedColumns != null && selectedColumns.Count > 0)
            {
                var listElementType = GetListElementType(records) ?? throw new InvalidOperationException("List element type is unknown.");

                var classMap = CreateClassMap(listElementType);
                var index = 0;

                foreach (var column in selectedColumns)
                {
                    var queue = new Queue<string>(column.Key.Split('.'));
                    MapProperty(csvWriter, queue, listElementType, classMap, column.Value, index++);
                }
            }
        }

        public async Task<string> WriteAsStringAsync(IEnumerable records, IDictionary<string, string> selectedColumns)
        {
            if (records != null)
            {
                var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    ShouldQuote = args => true
                };

                using (var memoryStream = new MemoryStream())
                using (var streamWriter = new StreamWriter(memoryStream))
                using (var csvWriter = new CsvWriter(streamWriter, csvConfig))
                {
                    SetSelectedColumnMapping(csvWriter, records, selectedColumns);

                    await csvWriter.WriteRecordsAsync(records);
                    await csvWriter.FlushAsync();

                    memoryStream.Position = 0;

                    return Encoding.Default.GetString(memoryStream.ToArray());
                }
            }

            return string.Empty;
        }
    }
}