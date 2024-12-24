
using System.Collections.Immutable;

namespace SetListr.Web.Services.DTO;

public class SetListApiClient(HttpClient httpClient)
{
    public async Task<IQueryable<Song>?> GetSongsForBandAsync(Guid bandId, int maxItems = 10, CancellationToken cancellationToken = default)
    {
        List<Song>? songs = null;

        await foreach (var song in httpClient.GetFromJsonAsAsyncEnumerable<Song>($"/songs?bandId={bandId}", cancellationToken))
        {
            if (songs?.Count >= maxItems)
            {
                break;
            }
            if (song is not null)
            {
                songs ??= [];
                songs.Add(song);
            }
        }

        return songs?.AsQueryable();
    }

    public async Task<IQueryable<SetList>?> GetSetListsAsync(Guid bandId, int maxItems = 10, CancellationToken cancellationToken = default)
    {
        List<SetList>? setLists = null;

        await foreach (var setList in httpClient.GetFromJsonAsAsyncEnumerable<SetList>($"/setLists?bandId={bandId}", cancellationToken))
        {
            if (setLists?.Count >= maxItems)
            {
                break;
            }

            if (setList is not null)
            {
                setLists ??= [];
                setLists.Add(setList);
            }
        }

        return setLists?.AsQueryable();
    }
}

public record Song(Guid Id, string Name, TimeSpan Duration)
{
}

public record SetList(Guid Id, string Name, ImmutableArray<Song> Songs)
{
    public TimeSpan Duration => Songs.Any() 
        ? Songs.Select(s => s.Duration).Aggregate((t1, t2) => t1 + t2)
        : TimeSpan.Zero;
}

public record Band(Guid Id, string Name, ImmutableArray<Song> Songs, ImmutableArray<SetList> SetLists)
{
}