using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chinook.Model;

namespace Chinook.Repository
{
    public class TrackRepository
    {

public List<Track> GetAll(string sortBy, int maximumRows, int startRowIndex)
{
    using (var context = new ChinookEntities())
    {
        if (string.IsNullOrWhiteSpace(sortBy))
        {
            sortBy = "Name";
        }
        return context.Tracks.Include(t => t.Album.Artist).Include(t => t.Genre).OrderUsingSortExpression(sortBy).Skip(startRowIndex).Take(maximumRows).ToList();
    }
}

public int GetCount()
{
    using (var context = new ChinookEntities())
    {
        return context.Tracks.Count();
    }
}

    }
}
