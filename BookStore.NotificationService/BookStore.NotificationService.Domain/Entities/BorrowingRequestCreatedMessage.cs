namespace BookStore.NotificationService.Domain.Entities;

public class BorrowingRequestCreatedMessage
{
    public Guid BorrowingRequestId { get; set; }
    public int UserId { get; set; }
    public int BookId { get; set; }
    public DateTime BorrowedDate { get; set; }
}