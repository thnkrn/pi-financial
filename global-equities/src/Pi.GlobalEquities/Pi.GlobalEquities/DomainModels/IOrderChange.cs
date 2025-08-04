namespace Pi.GlobalEquities.DomainModels;

public interface IOrderChange
{
    public DateTime CreatedAt { get; }
    public DateTime? UpdatedAt { get; }
    public bool HasBeenModified { get; }
}
