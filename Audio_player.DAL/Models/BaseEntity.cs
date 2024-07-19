namespace Audio_player.DAL.Models;

public abstract class BaseEntity<TKey>
    where TKey : struct, IEquatable<TKey>
{
    public TKey Id { get; set; }
}