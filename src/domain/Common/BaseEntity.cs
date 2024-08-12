namespace Domain.Common;

public class BaseEntity
{
    public Guid Id { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }

    public BaseEntity()
    {
        Id = Guid.NewGuid();
        CreatedDate = DateTime.UtcNow;
    }

    protected BaseEntity(Guid id)
    {
        Id = id;
        CreatedDate = DateTime.UtcNow;
    }

    public virtual void SetIsDeleted(bool isDeleted)
        => IsDeleted = isDeleted;
}