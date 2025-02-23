using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

const string TicketsFolder = @"C:\Users\user\Desktop\Tickets";


try
{
    var ticketsAggregator = new TicketAggregator(TicketsFolder);

    ticketsAggregator.Run();

}
catch (Exception ex)
{
    Console.WriteLine("An exception occured. " +
       "Exception message: " + ex.Message);
}

public class TicketAggregator
{
    private readonly string _ticketsFolder;

    public TicketAggregator(string ticketsFolder)
    {
        _ticketsFolder = ticketsFolder;
    }

    public void Run()
    {
        foreach (var filePath in Directory.GetFiles(_ticketsFolder, "*.pdf"))
        {
            using (PdfDocument document = PdfDocument.Open(filePath))
            {


                // Page number starts from 1, not 0.
                Page page = document.GetPage(1);

                string text = page.Text;
            }
        }


    }

}