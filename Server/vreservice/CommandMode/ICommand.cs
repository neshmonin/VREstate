namespace Vre.Server.Command
{
    internal interface ICommand
    {
        string Name { get; }
        void Execute(Parameters param);
    }
}