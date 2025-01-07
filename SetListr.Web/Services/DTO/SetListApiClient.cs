
using System.Collections.Immutable;

namespace SetListr.Web.Services.DTO;

public class SetListApiClient(HttpClient httpClient, ILoggerFactory loggerFactory)
{
    private readonly ILogger<SetListApiClient> _logger = loggerFactory.CreateLogger<SetListApiClient>();
    public async Task<IQueryable<Song>?> GetSongsForBandAsync(Guid bandId, int maxItems = 10, CancellationToken cancellationToken = default)
    {
        List<Song>? songs = null;

        try
        {
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
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting songs for band {bandId}", bandId);
            return null;
        }
    }

    public async Task<IQueryable<SetList>?> GetSetListsAsync(Guid? bandId, CancellationToken cancellationToken, int maxItems = 10)
    {
        List<SetList>? setLists = null;

        var response = bandId.HasValue
            ? httpClient.GetFromJsonAsAsyncEnumerable<SetList>($"/setLists?bandId={bandId}", cancellationToken)
            : httpClient.GetFromJsonAsAsyncEnumerable<SetList>("/setLists", cancellationToken);

        try 
        {
            await foreach (var setList in response)
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
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting set lists");
            return null;
        }

    }

    public async Task<IQueryable<SetList>?> GetSetListsAsync(int maxItems = 10, CancellationToken cancellationToken = default)
    {
        List<SetList>? setLists = null;

        try 
        {
            await foreach (var setList in httpClient.GetFromJsonAsAsyncEnumerable<SetList>("/setLists", cancellationToken))
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
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting set lists");
            return null;
        }
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