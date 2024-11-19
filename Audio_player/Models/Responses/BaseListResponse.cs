namespace Audio_player.Models.Responses;

public class BaseListResponse<T>
{
    public List<T> Result { get; set; } = [];
}
