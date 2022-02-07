namespace AlphabetUpdate.Client.Patch.Core.Handlers
{
    public interface IPatchStep
    {
        IPatchHandler CreateHandler(PatchContext context);
    }
}
