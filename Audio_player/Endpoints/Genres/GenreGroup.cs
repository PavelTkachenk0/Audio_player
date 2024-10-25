using FastEndpoints;

namespace Audio_player.Endpoints.Genres;

public class GenreGroup : Group
{
    public GenreGroup()
    {
        Configure("genres", ep => { });
    }
}
