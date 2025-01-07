using System.Collections.Immutable;

namespace SetListr.ApiService;
public class SetListManager
{
    private static ImmutableArray<Song> _songs;
    private static ImmutableArray<SetList> _setLists;
    private static ImmutableArray<Band> _bands;

    private readonly ILogger<SetListManager> _logger;

    public SetListManager(ILoggerFactory loggerFactory)
    {
        _songs = GenerateSongs();
        _setLists = GenerateSetLists(_songs);
        _bands = GenerateBands(_songs, _setLists);

        _logger = loggerFactory.CreateLogger<SetListManager>();
    }

    public ImmutableArray<Song> GetSongs() => _songs;
    public ImmutableArray<SetList> GetSetLists() => _setLists;
    public ImmutableArray<Band> GetBands() => _bands;

    public ImmutableArray<SetList> GetSetListsForBand(Guid value)
    {
        _logger.LogInformation($"Getting set lists for band {value}");
        return _setLists;
    }

    public ImmutableArray<SetList> GetSetListsForUser(string name)
    {
        _logger.LogInformation($"Getting set lists for user {name}");
        return _setLists;
    }

    public ImmutableArray<Song>? GetSongsForBand(Guid bandId)
    {
        _logger.LogInformation($"Getting songs for band {bandId}");
        return _bands.FirstOrDefault(b => b.Id == bandId)?.Songs;
    }

    private static ImmutableArray<Band> GenerateBands(ImmutableArray<Song> songs, ImmutableArray<SetList> setLists)
    {
        return [new Band(Guid.NewGuid(), "Band 1", songs, setLists)];
    }

    private static ImmutableArray<Song> GenerateSongs(int amount = 20)
    {
        var songs = new Song[amount];
        for (var i = 0; i < amount; i++)
        {
            songs[i] = new Song(
                Guid.NewGuid(),
                $"Song {i}",
                TimeSpan.FromSeconds(Random.Shared.Next(700)));
        }

        return songs.ToImmutableArray();
    }

    private static ImmutableArray<SetList> GenerateSetLists(ImmutableArray<Song> songs, int amount = 5)
    {
        var setLists = new SetList[amount];
        for (var i = 0; i < amount; i++)
        {
            var a = Random.Shared.Next(songs.Length);
            var b = Random.Shared.Next(songs.Length);
            var start = Math.Min(a, b);
            var end = Math.Max(a, b);

            setLists[i] = new SetList(
                Guid.NewGuid(),
                $"Set List {i}",
                songs[start..end].ToImmutableArray()
            );
        }

        return setLists.ToImmutableArray();
    }
}


public record Song(Guid Id, string Name, TimeSpan Duration)
{
}

public record SetList(Guid Id, string Name, ImmutableArray<Song> Songs)
{
}

public record Band(Guid Id, string Name, ImmutableArray<Song> Songs, ImmutableArray<SetList> SetLists)
{
}