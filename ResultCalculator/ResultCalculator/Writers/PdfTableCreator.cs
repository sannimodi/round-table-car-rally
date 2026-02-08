using PdfSharp.Drawing;
using PdfSharp.Pdf;

internal class PdfTableCreator
{
    private readonly double _xStart = 50;
    private readonly double _yStart = 60;
    private readonly double _rowHeight = 20;
    private readonly XFont _titleFont = new ("Verdana", 14, XFontStyleEx.Bold);
    private readonly XFont _rowFont = new ("Verdana", 10, XFontStyleEx.Regular);

    public void CreateTablePdf(string tableName, string[] values, string filename)
    {
        // Create a new PDF document
        PdfDocument document = new();

        document.Info.Title = tableName;

        // Create an empty page
        PdfPage page = document.AddPage();

        // Start drawing on the page
        XGraphics gfx = XGraphics.FromPdfPage(page);
        double pageHeight = page.Height.Point;
        DrawTableHeader(gfx, tableName, page);

        double yPosition = _yStart;

        foreach (string value in values)
        {
            // Check if we need a new page
            if (yPosition + _rowHeight > pageHeight - _xStart)
            {
                // Add a new page
                page = document.AddPage();
                gfx = XGraphics.FromPdfPage(page);
                pageHeight = page.Height.Point;
                yPosition = _yStart; // Reset y position for the new page

                DrawTableHeader(gfx, tableName, page); // Optionally redraw the table header on each page
            }

            string[] rowValues = value.Split(',');
            DrawRow(gfx, rowValues, page, ref yPosition);
        }

        // Save the document
        document.Save(Path.Combine(ConfigProvider.GetResultPath(), $"{filename}.pdf"));

        // Cleanup
        document.Dispose();
    }

    private void DrawTableHeader(XGraphics gfx, string tableName, PdfPage page)
    {
        gfx.DrawString(tableName, _titleFont, XBrushes.Black, new XRect(0, 20, page.Width.Point, page.Height.Point), XStringFormats.TopCenter);
    }

    private void DrawRow(XGraphics gfx, string[] rowValues, PdfPage page, ref double yPosition)
    {
        double columnWidth = (page.Width.Point - 100) / rowValues.Length;
        double textMargin = 15; // Margin between the top of the cell and the text

        for (int i = 0; i < rowValues.Length; i++)
        {
            // Draw cell border
            gfx.DrawRectangle(XPens.Black, _xStart + (columnWidth * i), yPosition, columnWidth, _rowHeight);

            // Adjust y position for text to avoid overlapping with the border
            double textYPosition = yPosition + textMargin;

            // Draw text inside the cell, considering the text margin
            gfx.DrawString(rowValues[i], _rowFont, XBrushes.Black, _xStart + (columnWidth * i) + 5, textYPosition);
        }

        yPosition += _rowHeight; // Move to the next row position
    }
}
