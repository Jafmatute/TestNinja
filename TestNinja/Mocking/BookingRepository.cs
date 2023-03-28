using System.Linq;

namespace TestNinja.Mocking
{
    public interface IBookingRepository
    {
        IQueryable<Booking> GetActiveBookings(int? excludebookingId=null);
    }

    public class BookingRepository : IBookingRepository
    {
        public IQueryable<Booking> GetActiveBookings(int? excludebookingId=null)
        {
            var unitOfWork = new UnitOfWork();
            var bookings =
                unitOfWork.Query<Booking>()
                    .Where(
                        b =>  b.Status != "Cancelled");

            if (excludebookingId.HasValue)
                bookings = bookings.Where(b => b.Id != excludebookingId.Value);

            return bookings;
            
        }
    }
}