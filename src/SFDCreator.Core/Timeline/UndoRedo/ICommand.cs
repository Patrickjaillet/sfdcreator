namespace SFDCreator.Core.Timeline.UndoRedo;

public interface ICommand
{
    void Execute();

    void Undo();
}
