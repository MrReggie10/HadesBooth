
public class Tutorial : GameParallelState
{
    public Tutorial(GameStatus status, string id = null) : base(status, id)
    {
        AddState(new PlayNotes(status, 0, "Play tutorial notes"));
        AddState(new ReadConductorNotes(status, 0, OnConductorTransition, "Read conductor notes"));
        AddState(new ReadLyreNotes(status, 0, true, OnLyreTransition, "Read lyre notes"));
    }

    protected static void OnConductorTransition(int numSuccess)
    {
        if (numSuccess == 0)
        {
            // TODO fail noise or lights?
        }
        else
        {
            // TODO success noise or lights?
        }
    }

    protected static void OnLyreTransition(int numSuccess)
    {
        if (numSuccess == 0)
        {
            // TODO fail noise or lights?
        }
        else
        {
            // TODO success noise or lights?
        }
    }
}
