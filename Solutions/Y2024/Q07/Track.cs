using Utilities.Geometry;

namespace Solutions.Y2024.Q07;

public static class Track
{
    private const string One = "S";
    private const string Two = 
        """
        S-=++=-==++=++=-=+=-=+=+=--=-=++=-==++=-+=-=+=-=+=+=++=-+==++=++=-=-=--
        -                                                                     -
        =                                                                     =
        +                                                                     +
        =                                                                     +
        +                                                                     =
        =                                                                     =
        -                                                                     -
        --==++++==+=+++-=+=-=+=-+-=+-=+-=+=-=+=--=+++=++=+++==++==--=+=++==+++-
        """;
    private const string Three =
        """
        S+= +=-== +=++=     =+=+=--=    =-= ++=     +=-  =+=++=-+==+ =++=-=-=--
        - + +   + =   =     =      =   == = - -     - =  =         =-=        -
        = + + +-- =-= ==-==-= --++ +  == == = +     - =  =    ==++=    =++=-=++
        + + + =     +         =  + + == == ++ =     = =  ==   =   = =++=       
        = = + + +== +==     =++ == =+=  =  +  +==-=++ =   =++ --= + =          
        + ==- = + =   = =+= =   =       ++--          +     =   = = =--= ==++==
        =     ==- ==+-- = = = ++= +=--      ==+ ==--= +--+=-= ==- ==   =+=    =
        -               = = = =   +  +  ==+ = = +   =        ++    =          -
        -               = + + =   +  -  = + = = +   =        +     =          -
        --==++++==+=+++-= =-= =-+-=  =+-= =-= =--   +=++=+++==     -=+=++==+++-
        """;

    public static string GetPlan(int part)
    {
        var trackStr = part switch
        {
            1 => One,
            2 => Two,
            3 => Three,
            _ => throw new NoSolutionException($"Invalid track number ['{part}']")
        };
        
        var plan = new List<char>();
        var track = Grid2D<char>.MapChars(
            strings: trackStr.Split(["\r\n", "\r", "\n"], StringSplitOptions.RemoveEmptyEntries));
        
        var start = track.Single(pos => track[pos] == 'S');
        var pose = new Pose2D(Pos: start, Face: Vec2D.Right);
        var visited = new HashSet<Vec2D>();
        
        while (track.Contains(pose.Pos) && visited.Add(pose.Pos))
        {
            plan.Add(track[pose.Pos]);
            pose = CanMove(track, pose.Ahead)
                ? pose.Step()
                : CanMove(track, pose.Right)
                    ? pose.Turn(Rot3D.N90Z).Step()
                    : pose.Turn(Rot3D.P90Z).Step();
        }

        return new string(plan[1..].Append(plan[0]).ToArray());
    }

    private static bool CanMove(Grid2D<char> track, Vec2D to)
    {
        return track.Contains(to) && !char.IsWhiteSpace(track[to]);
    }
}