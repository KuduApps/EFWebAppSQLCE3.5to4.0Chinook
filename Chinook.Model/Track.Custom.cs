using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chinook.Model
{
     public partial class Track
    {
        //For display only
        public string AlbumName
        {
            get
            {
                if (this.Album != null)
                {
                    return this.Album.Title;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

    }
}
