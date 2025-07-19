using System;
using System.Globalization;
using System.Reflection;
using Application.Interfaces;
using CsvHelper;
using CsvHelper.Configuration;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using Application.Exports;
using SharedKernel.Enums;

namespace Infrastructure.Services;

public class DynamicExportStrategy<T> : IExportStrategy where T : class
{
    private readonly IGenericRepository<T> _repository;

    public DynamicExportStrategy(
        IGenericRepository<T> repository,
        AdminModule moduleType)
    {
        _repository = repository;
        ModuleType = moduleType;
    }

    public AdminModule ModuleType { get; }

    AdminModule IExportStrategy.ModuleType => ModuleType;

    public async Task<ExportDataResultDto> ExportDataAsync(ExportType exportType)
    {
        List<T> data = await _repository.GetAllAsync();
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
        using var streamWriter = new StreamWriter(memoryStream);
        using var csvWriter = new CsvWriter(streamWriter, new CsvConfiguration(CultureInfo.InvariantCulture));

        csvWriter.WriteRecords(data);
        streamWriter.Flush();

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
        using var writer = new PdfWriter(memoryStream);
        using var pdf = new PdfDocument(writer);

        // Set to landscape A4
        pdf.SetDefaultPageSize(PageSize.A4.Rotate());

        var document = new Document(pdf);

        // Title
        Paragraph title = new Paragraph($"{typeof(T).Name} Export")
            .SetTextAlignment(TextAlignment.CENTER)
            .SetFontSize(18).SetBackgroundColor(ColorConstants.LIGHT_GRAY);
        document.Add(title);

        document.Add(new Paragraph($"Export Date: {DateTime.Now:g}"));
        document.Add(new Paragraph("")); // Empty line

        // Get properties
        PropertyInfo[] properties = typeof(T).GetProperties();

        Table table = new Table(properties.Length)
            .SetWidth(UnitValue.CreatePercentValue(100))
            .SetMarginTop(10)
            .SetMarginBottom(10);

        foreach (PropertyInfo prop in properties)
        {
            Cell headerCell = new Cell()
                .Add(new Paragraph(prop.Name)
                    .SetFontSize(12))
                .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                .SetTextAlignment(TextAlignment.CENTER);
            table.AddHeaderCell(headerCell);
        }

        foreach (T item in data)
        {
            foreach (PropertyInfo prop in properties)
            {
                object? rawValue = prop.GetValue(item);
                string cellValue = string.Empty;

                if (rawValue != null)
                {
                    Type? enumType = null;
                    if (prop.PropertyType.IsEnum)
                    {
                        enumType = prop.PropertyType;
                    }
                    else
                    {
                        Type? underlyingType = Nullable.GetUnderlyingType(prop.PropertyType);
                        if (underlyingType != null && underlyingType.IsEnum)
                        {
                            enumType = underlyingType;
                        }
                    }

                    cellValue = enumType != null
                        ? Enum.GetName(enumType, rawValue) ?? string.Empty
                        : rawValue.ToString() ?? string.Empty;
                }

                Cell cell = new Cell()
                    .Add(new Paragraph(cellValue)
                        .SetFontSize(10))
                    .SetTextAlignment(TextAlignment.LEFT);
                table.AddCell(cell);
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
}