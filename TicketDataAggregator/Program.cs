﻿using System.Globalization;
using System.Text;
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
    private readonly Dictionary<string, string> _domainToCultureMapping = new()
    {
        [".com"] = "en-US",
        [".fr"] = "fr-FR",
        [".jp"] = "ja-JP"

    };

    public TicketAggregator(string ticketsFolder)
    {
        _ticketsFolder = ticketsFolder;
    }

    public void Run()
    {
        var stringBuilder = new StringBuilder();
        foreach (var filePath in Directory.GetFiles(_ticketsFolder, "*.pdf"))
        {
            using PdfDocument document = PdfDocument.Open(filePath);


            // Page number starts from 1, not 0.
            Page page = document.GetPage(1);

            string text = page.Text;
            var split = text.Split(new[] { "Title:", "Date:", "Time:", "Visit us:" },
                  StringSplitOptions.None);


            var domain = ExtractDomain(split.Last());
            var ticketCulture = _domainToCultureMapping[domain];

            for (int i = 1; i < split.Length - 3; i += 3)
            {
                var title = split[i];
                var dateAsString = split[i + 1];
                var timeAsString = split[i + 2];

                var date = DateOnly.Parse(dateAsString, new CultureInfo(ticketCulture));
                var time = TimeOnly.Parse(timeAsString, new CultureInfo(ticketCulture));

                var dateAsStringInvariant = date.ToString(CultureInfo.InvariantCulture);
                var timeAsStringInvariant = time.ToString(CultureInfo.InvariantCulture);

                var ticketData = $"{title,-40} | {dateAsStringInvariant}|{timeAsStringInvariant}";
                stringBuilder.AppendLine(ticketData);

            }

        }

        var resultPath = Path.Combine(_ticketsFolder, "aggregatedTickets.txt");
        File.WriteAllText(resultPath, stringBuilder.ToString());
        Console.WriteLine("Results saved to " + resultPath);

    }

    private static string ExtractDomain(string webAddress)
    {
        var lastDotIndex = webAddress.LastIndexOf('.');
        return webAddress.Substring(lastDotIndex);
    }
}