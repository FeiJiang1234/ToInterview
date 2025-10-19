using ToInterview.API.Patterns.Memento;
using Xunit;

namespace ToInterview.API.Tests.Patterns
{
    public class MementoTests
    {
        [Fact]
        public void TextEditor_ShouldRestoreFromMemento()
        {
            // Arrange
            var editor = new TextEditor();
            editor.Content = "原始内容";

            var memento = editor.CreateMemento();
            editor.Content = "修改后内容";

            // Act
            editor.RestoreFromMemento(memento);

            // Assert
            Assert.Equal("原始内容", editor.Content);
        }

        [Fact]
        public void Caretaker_ShouldSaveMemento()
        {
            // Arrange
            var caretaker = new Caretaker();
            var memento = new TextMemento("测试内容");

            // Act
            caretaker.SaveMemento(memento);

            // Assert
            Assert.Equal(1, caretaker.GetMementosCount());
        }

        [Fact]
        public void Caretaker_ShouldUndo()
        {
            // Arrange
            var caretaker = new Caretaker();
            var memento1 = new TextMemento("第一个版本");
            var memento2 = new TextMemento("第二个版本");
            caretaker.SaveMemento(memento1);
            caretaker.SaveMemento(memento2);

            // Act
            var undoMemento = caretaker.Undo();

            // Assert
            Assert.NotNull(undoMemento);
            Assert.Equal("第一个版本", undoMemento.Content);
        }

        [Fact]
        public void Caretaker_ShouldRedo()
        {
            // Arrange
            var caretaker = new Caretaker();
            var memento1 = new TextMemento("第一个版本");
            var memento2 = new TextMemento("第二个版本");
            caretaker.SaveMemento(memento1);
            caretaker.SaveMemento(memento2);
            caretaker.Undo(); // 先撤销

            // Act
            var redoMemento = caretaker.Redo();

            // Assert
            Assert.NotNull(redoMemento);
            Assert.Equal("第二个版本", redoMemento.Content);
        }

        [Fact]
        public void Caretaker_ShouldNotUndoWhenAtFirstVersion()
        {
            // Arrange
            var caretaker = new Caretaker();
            var memento = new TextMemento("唯一版本");
            caretaker.SaveMemento(memento);

            // Act
            var undoMemento = caretaker.Undo();

            // Assert
            Assert.Null(undoMemento);
        }

        [Fact]
        public void Caretaker_ShouldNotRedoWhenAtLastVersion()
        {
            // Arrange
            var caretaker = new Caretaker();
            var memento = new TextMemento("唯一版本");
            caretaker.SaveMemento(memento);

            // Act
            var redoMemento = caretaker.Redo();

            // Assert
            Assert.Null(redoMemento);
        }

        [Fact]
        public void Caretaker_ShouldGetMementoByIndex()
        {
            // Arrange
            var caretaker = new Caretaker();
            var memento1 = new TextMemento("第一个版本");
            var memento2 = new TextMemento("第二个版本");
            caretaker.SaveMemento(memento1);
            caretaker.SaveMemento(memento2);

            // Act
            var retrievedMemento = caretaker.GetMemento(0);

            // Assert
            Assert.NotNull(retrievedMemento);
            Assert.Equal("第一个版本", retrievedMemento.Content);
        }

        [Fact]
        public void Caretaker_ShouldReturnNullForInvalidIndex()
        {
            // Arrange
            var caretaker = new Caretaker();

            // Act
            var memento = caretaker.GetMemento(5);

            // Assert
            Assert.Null(memento);
        }

        [Fact]
        public void TextEditor_ShouldUpdateContent()
        {
            // Arrange
            var editor = new TextEditor();

            // Act
            editor.Content = "新内容";

            // Assert
            Assert.Equal("新内容", editor.Content);
        }
    }
}
