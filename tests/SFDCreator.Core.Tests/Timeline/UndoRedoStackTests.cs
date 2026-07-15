using SFDCreator.Core.Timeline.UndoRedo;

namespace SFDCreator.Core.Tests.Timeline;

public sealed class UndoRedoStackTests
{
    private sealed class RecordingCommand : ICommand
    {
        public int ExecuteCount { get; private set; }

        public int UndoCount { get; private set; }

        public void Execute() => ExecuteCount++;

        public void Undo() => UndoCount++;
    }

    [Fact]
    public void Do_ExecutesCommandImmediately()
    {
        var stack = new UndoRedoStack();
        var command = new RecordingCommand();

        stack.Do(command);

        Assert.Equal(1, command.ExecuteCount);
        Assert.True(stack.CanUndo);
        Assert.False(stack.CanRedo);
    }

    [Fact]
    public void Undo_CallsUndoAndEnablesRedo()
    {
        var stack = new UndoRedoStack();
        var command = new RecordingCommand();
        stack.Do(command);

        stack.Undo();

        Assert.Equal(1, command.UndoCount);
        Assert.False(stack.CanUndo);
        Assert.True(stack.CanRedo);
    }

    [Fact]
    public void Redo_ReExecutesCommand()
    {
        var stack = new UndoRedoStack();
        var command = new RecordingCommand();
        stack.Do(command);
        stack.Undo();

        stack.Redo();

        Assert.Equal(2, command.ExecuteCount);
        Assert.True(stack.CanUndo);
        Assert.False(stack.CanRedo);
    }

    [Fact]
    public void Do_AfterUndo_ClearsRedoHistory()
    {
        var stack = new UndoRedoStack();
        var first = new RecordingCommand();
        var second = new RecordingCommand();

        stack.Do(first);
        stack.Undo();
        stack.Do(second);

        Assert.False(stack.CanRedo);
    }

    [Fact]
    public void Undo_WhenNothingToUndo_IsNoOp()
    {
        var stack = new UndoRedoStack();

        stack.Undo();

        Assert.False(stack.CanUndo);
    }

    [Fact]
    public void Redo_WhenNothingToRedo_IsNoOp()
    {
        var stack = new UndoRedoStack();

        stack.Redo();

        Assert.False(stack.CanRedo);
    }
}
