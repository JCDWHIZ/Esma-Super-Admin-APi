using System;
using admin_service.Application.Common.Interfaces;
using CsvHelper;
using iTextSharp.text;
using iTextSharp.text.pdf;
using admin_service.Application.Exports.Queries;
using admin_service.Domain.Enums;
using System.Globalization;

namespace admin_service.Infrastructure.Services;

public class DynamicExportStrategy<T> : IExportStrategy where T : class
    {
            private readonly IGenericRepository<T> _repository;
            private readonly AdminModule _moduleType; // Store module type during construction

            public DynamicExportStrategy(
                IGenericRepository<T> repository, 
                AdminModule moduleType) // Add module type as a constructor parameter
            {
                _repository = repository;
                _moduleType = moduleType; // Set the module type
            }

            public AdminModule ModuleType => _moduleType;


        public async Task<ExportDataResultDto> ExportDataAsync(ExportType exportType)
        {
            var data = await _repository.GetAllAsync();
            return exportType switch
            {
                ExportType.CSV => ExportToCsv(data),
                ExportType.PDF => ExportToPdf(data),
                _ => throw new NotImplementedException("Unsupported export type.")
            };
        }

        private static ExportDataResultDto ExportToCsv(List<T> data)
        {
            if (data == null || data.Count == 0)
            {
                throw new ArgumentException("No data available for export");
            }
            using var memoryStream = new MemoryStream();
            using (var streamWriter = new StreamWriter(memoryStream))
            using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
            {
                csvWriter.WriteRecords(data);
                streamWriter.Flush();
            }

            return new ExportDataResultDto
            {
                FileBytes = memoryStream.ToArray(),
                FileName = $"{typeof(T).Name}_Export.csv",
                ContentType = "text/csv"
            };
        }

        private static ExportDataResultDto ExportToPdf(List<T> data)
        {
             using var memoryStream = new MemoryStream();
            var document = new Document(PageSize.A4.Rotate());
            PdfWriter.GetInstance(document, memoryStream);
            document.Open();
            var titleFont = new Font(Font.FontFamily.HELVETICA, 18, Font.BOLD);
            var title = new Paragraph($"{typeof(T).Name} Export", titleFont)
            {
                Alignment = Element.ALIGN_CENTER
            };
            document.Add(title);
            document.Add(new Paragraph($"Export Date: {DateTime.Now:g}"));
            document.Add(Chunk.NEWLINE);
            var properties = typeof(T).GetProperties();
            var table = new PdfPTable(properties.Length)
            {
                WidthPercentage = 100,
                SpacingBefore = 10f,
                SpacingAfter = 10f
            };

            foreach (var prop in properties)
            {
                var headerCell = new PdfPCell(new Phrase(prop.Name, new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD)))
                {
                    BackgroundColor = BaseColor.LIGHT_GRAY
                };
                table.AddCell(headerCell);
            }

           foreach (var item in data)
    {
        foreach (var prop in properties)
        {
            var rawValue = prop.GetValue(item);
            string cellValue = string.Empty;

            if (rawValue != null)
            {
                var enumType = prop.PropertyType.IsEnum
                    ? prop.PropertyType
                    : Nullable.GetUnderlyingType(prop.PropertyType)?.IsEnum == true
                        ? Nullable.GetUnderlyingType(prop.PropertyType)
                        : null;

                if (enumType != null)
                {
                    cellValue = Enum.GetName(enumType, rawValue) ?? string.Empty;
                }
                else
                {
                    cellValue = rawValue.ToString() ?? string.Empty;
                }
            }

            table.AddCell(new Phrase(cellValue, new Font(Font.FontFamily.HELVETICA, 10)));
        }
    }



            document.Add(table);
            document.Close();

            return new ExportDataResultDto
            {
                FileBytes = memoryStream.ToArray(),
                FileName = $"{typeof(T).Name}_Export.pdf",
                ContentType = "application/pdf"
            };
        }

        private static AdminModule GetModuleTypeFromEntity()
        {
            return Enum.TryParse(typeof(T).Name.ToUpper(), out AdminModule module)
                ? module
                : throw new InvalidOperationException($"No module mapping for {typeof(T).Name}");
        }
    }
