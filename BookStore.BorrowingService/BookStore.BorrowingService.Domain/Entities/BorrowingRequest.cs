namespace BookStore.Domain.Entities;

public class BorrowingRequest
{
    public Guid Id { get; set; }
    public int UserId { get; set; }
    public int BookId { get; set; }
    public DateTime BorrowedDate { get; set; }
    public DateTime? ReturnedDate { get; set; }
    public BorrowingStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public enum BorrowingStatus
{
    Pending,
    Approved,
    Returned,
    Rejected
}