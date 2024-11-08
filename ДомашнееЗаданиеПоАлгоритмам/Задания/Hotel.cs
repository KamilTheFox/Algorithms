using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Hotel
{
    private record VisitTicket(DateTime Date, int visited);

    private record People(TicketHotel Ticket);

    private IList<People> Peoples { get; set; } = new List<People>();

    public void Add(TicketHotel ticket)
    {
        Peoples.Add(new People(ticket));
    }
    public DateTime FindMostCrowdedDay()
    {
        // Создаем список событий: +1 при заезде, -1 при выезде
        var events = new List<VisitTicket>();

        foreach (var visitPeople in Peoples)
        { 
            events.Add(new VisitTicket(visitPeople.Ticket.EnterDate, 1));
            events.Add(new VisitTicket(visitPeople.Ticket.ExitDate.AddDays(1), -1)); // AddDays(1) потому что выезд в конце дня
        }

        // Сортируем события по дате
        events.Sort((a, b) => a.Date.CompareTo(b.Date));

        int currentGuests = 0;
        int maxGuests = 0;
        DateTime maxPeopleDate = events[0].Date;

        foreach (var evt in events)
        {
            currentGuests += evt.visited;
            if (currentGuests > maxGuests)
            {
                maxGuests = currentGuests;
                maxPeopleDate = evt.Date;
            }
        }
        return maxPeopleDate;
    }


}

public struct TicketHotel
{
    public TicketHotel(DateTime EnterDate, DateTime ExitDate)
    {
        this.EnterDate = EnterDate;
        this.ExitDate = ExitDate;
    }
    public DateTime EnterDate, ExitDate;

    public static TicketHotel Create(string enterDate, string exitDate)
    {
        return new TicketHotel(DateTime.Parse(enterDate), DateTime.Parse(exitDate));
    }
}
