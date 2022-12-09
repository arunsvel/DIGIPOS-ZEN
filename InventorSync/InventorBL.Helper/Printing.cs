//////https://stackoverflow.com/questions/8849835/how-to-invoke-print-button-of-microsoft-report-viewer-using-c-sharp-programmatic
//directly copy this code in a class called "Printing" and Call the "Run" method with your reportviewer name as parameter. Example obj.Run(reportviewer1);

////////////////////////////////////////////////////////////////////////////////////
using System;
using System.IO;
using System.Data;
using System.Text;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;
using System.Drawing;
using DigiposZen.InventorBL.Helper;

public class Printing : IDisposable
{
    private int m_currentPageIndex;
    private IList<Stream> m_streams;

    Common Comm = new Common();

    private DataTable LoadSalesData()
    {
        // Create a new DataSet and read sales data file 
        //    data.xml into the first DataTable.
        DataSet dataSet = new DataSet();
        dataSet.ReadXml(@"..\..\data.xml");
        return dataSet.Tables[0];
    }
    // Routine to provide to the report renderer, in order to
    //    save an image for each page of the report.
    private Stream CreateStream(string name,
      string fileNameExtension, Encoding encoding,
      string mimeType, bool willSeek)
    {
        Stream stream = new MemoryStream();
        m_streams.Add(stream);
        return stream;
    }
    // Export the given report as an EMF (Enhanced Metafile) file.
    private void Export(LocalReport report, string strPrintSetting, string ReportName, decimal NoOfItems)
    {
        string[] separator1 = { ";;" };
        string[] separator2 = { "||" };
        string[] strSplit = strPrintSetting.Split(separator1, StringSplitOptions.RemoveEmptyEntries);

        string PageWidth = "";
        string HeaderHeight = "";
        string ItemHeight = "";
        string FooterHeight = "";


        for (int i = 0; i < strSplit.Length; i++)
        {
            if (strSplit[i] != null)
            {
                string[] strSplit2 = strSplit[i].Split(separator2, StringSplitOptions.RemoveEmptyEntries);

                if (strSplit2[1].ToString() == ReportName)
                {
                    PageWidth = strSplit2[2].ToString();
                    HeaderHeight = strSplit2[3].ToString();
                    ItemHeight = strSplit2[4].ToString();
                    FooterHeight = strSplit2[5].ToString();

                    break;
                }
            }
        }

        //0.98596 * 3
        //            headerheight + (itemheight * (noofitems + 1 (item header))) + footerheight
        decimal paperheight = 0;
        paperheight = (decimal)(Comm.ToDecimal(HeaderHeight) + (Comm.ToDecimal(ItemHeight) * (NoOfItems + 1)) + Comm.ToDecimal(FooterHeight));

            string deviceInfo =
          @"<DeviceInfo>
                <OutputFormat>EMF</OutputFormat>
                <PageWidth>" + PageWidth + @"cm</PageWidth>
                <PageHeight>" + paperheight + @"cm</PageHeight>
                <MarginTop>0.1cm</MarginTop>
                <MarginLeft>0.1cm</MarginLeft>
                <MarginRight>0.1cm</MarginRight>
                <MarginBottom>0.1cm</MarginBottom>
            </DeviceInfo>";
        Warning[] warnings;
        m_streams = new List<Stream>();
        report.Render("Image", deviceInfo, CreateStream,
           out warnings);

        //If rsReport!paperheight = 0 Or rsReport!paperheight > 450 Then
        //if (m_streams.Count > mTotalPages)
        //{
        //    AddPaperHeight = AddPaperHeight + 0.2;
        //    GoTo renderagain
        //}
        //End If
            
        foreach (Stream stream in m_streams)
            stream.Position = 0;
    }
    // Handler for PrintPageEvents
    private void PrintPage(object sender, PrintPageEventArgs ev)
    {
        Metafile pageImage = new
           Metafile(m_streams[m_currentPageIndex]);

        // Adjust rectangular area with printer margins.
        Rectangle adjustedRect = new Rectangle(
            ev.PageBounds.Left - (int)ev.PageSettings.HardMarginX,
            ev.PageBounds.Top - (int)ev.PageSettings.HardMarginY,
            ev.PageBounds.Width,
            ev.PageBounds.Height);

        // Draw a white background for the report
        ev.Graphics.FillRectangle(Brushes.White, adjustedRect);

        // Draw the report content
        ev.Graphics.DrawImage(pageImage, adjustedRect);

        // Prepare for the next page. Make sure we haven't hit the end.
        m_currentPageIndex++;
        ev.HasMorePages = (m_currentPageIndex < m_streams.Count);
    }

    private void Print()
    {
        if (m_streams == null || m_streams.Count == 0)
            throw new Exception("Error: no stream to print.");
        PrintDocument printDoc = new PrintDocument();
        if (!printDoc.PrinterSettings.IsValid)
        {
            throw new Exception("Error: cannot find the default printer.");
        }
        else
        {
            printDoc.PrintPage += new PrintPageEventHandler(PrintPage);
            m_currentPageIndex = 0;
            printDoc.Print();
        }
    }
    // Create a local report for Report.rdlc, load the data,
    //    export the report to an .emf file, and print it.
    public void Run(ReportViewer rpt, string PrintSettings, string ReportName, decimal NoOfItems)
    {
        Export(rpt.LocalReport, PrintSettings, ReportName, NoOfItems);
        Print();
    }

    public void Dispose()
    {
        if (m_streams != null)
        {
            foreach (Stream stream in m_streams)
                stream.Close();
            m_streams = null;
        }
    }

    //public static void Main(string[] args)
    //{
    //    using (Demo demo = new Demo())
    //    {
    //        demo.Run();
    //    }
    //}
}
