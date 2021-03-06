﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using VsVim;
using EnvDTE;
using Moq;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text.Editor;
using Vim;
using Microsoft.VisualStudio.Text;
using VsVim.Properties;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Editor;
using VimCoreTest.Utils;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Text.Operations;

namespace VsVimTest
{
    [TestFixture]
    public class VsVimHostTest
    {
        private VsVimHost _hostRaw;
        private IVimHost _host;
        private Mock<IVsEditorAdaptersFactoryService> _editorAdaptersFactoryService;
        private Mock<ITextBufferUndoManagerProvider> _undoManagerProvider;
        private Mock<_DTE> _dte;
        private Mock<IVsTextManager> _textManager;
        private Mock<StatusBar> _statusBar;

        private void Create()
        {
            _undoManagerProvider = new Mock<ITextBufferUndoManagerProvider>(MockBehavior.Strict);
            _editorAdaptersFactoryService = new Mock<IVsEditorAdaptersFactoryService>(MockBehavior.Strict);
            _hostRaw = new VsVimHost(_undoManagerProvider.Object, _editorAdaptersFactoryService.Object);
            _host = _hostRaw;
        }

        private void CreateRest()
        {
            _statusBar = new Mock<StatusBar>(MockBehavior.Strict);
            _dte = new Mock<_DTE>(MockBehavior.Strict);
            _dte.SetupGet(x => x.StatusBar).Returns(_statusBar.Object);
            _textManager = new Mock<IVsTextManager>(MockBehavior.Strict);
            _hostRaw.Init(_dte.Object, _textManager.Object);
        }

        private void CreateAll()
        {
            Create();
            CreateRest();
        }

        [TearDown]
        public void TearDown()
        {
            _statusBar = null;
            _dte = null;
            _host = null;
            _hostRaw = null;
        }

        [Test, Description("Don't crash id _dte is not set")]
        public void UpdateStatus1()
        {
            Create();
            _host.UpdateStatus("foo");
        }

        [Test]
        public void UpdateStatus2()
        {
            CreateAll();
            _statusBar.SetupSet(x => x.Text).Verifiable();
            _host.UpdateStatus("foo");
            _statusBar.Verify();
        }

        [Test]
        public void Undo1()
        {
            CreateAll();
            var buffer = new Mock<ITextBuffer>(MockBehavior.Strict);
            _undoManagerProvider.Setup(x => x.GetTextBufferUndoManager(buffer.Object)).Returns((ITextBufferUndoManager)null).Verifiable();
            _statusBar
                .SetupSet(x => x.Text)
                .Callback(msg => Assert.AreEqual(Resources.VimHost_NoUndoRedoSupport, msg))
                .Verifiable();
            _host.Undo(buffer.Object, 1);
            _undoManagerProvider.Verify();
            _statusBar.Verify();
        }

        [Test]
        public void Undo2()
        {
            CreateAll();
            var buffer = new Mock<ITextBuffer>(MockBehavior.Strict);
            var manager = new Mock<ITextBufferUndoManager>(MockBehavior.Strict);
            var history = new Mock<ITextUndoHistory>(MockBehavior.Strict);
            history.SetupGet(x => x.CanUndo).Throws(new NotSupportedException());
            manager.SetupGet(x => x.TextBufferUndoHistory).Returns(history.Object);
            _undoManagerProvider.Setup(x => x.GetTextBufferUndoManager(buffer.Object)).Returns(manager.Object);
            _statusBar
                .SetupSet(x => x.Text)
                .Callback(msg => Assert.AreEqual(Resources.VimHost_CannotUndo, msg))
                .Verifiable();
            _host.Undo(buffer.Object, 1);
            _statusBar.Verify();
        }

        [Test]
        public void Redo1()
        {
            CreateAll();
            var buffer = new Mock<ITextBuffer>(MockBehavior.Strict);
            _undoManagerProvider.Setup(x => x.GetTextBufferUndoManager(buffer.Object)).Returns((ITextBufferUndoManager)null).Verifiable();
            _statusBar
                .SetupSet(x => x.Text)
                .Callback(msg => Assert.AreEqual(Resources.VimHost_NoUndoRedoSupport, msg))
                .Verifiable();
            _host.Redo(buffer.Object, 1);
            _undoManagerProvider.Verify();
            _statusBar.Verify();
        }

        [Test]
        public void Redo2()
        {
            CreateAll();
            var buffer = new Mock<ITextBuffer>(MockBehavior.Strict);
            var manager = new Mock<ITextBufferUndoManager>(MockBehavior.Strict);
            var history = new Mock<ITextUndoHistory>(MockBehavior.Strict);
            history.SetupGet(x => x.CanRedo).Throws(new NotSupportedException());
            manager.SetupGet(x => x.TextBufferUndoHistory).Returns(history.Object);
            _undoManagerProvider.Setup(x => x.GetTextBufferUndoManager(buffer.Object)).Returns(manager.Object);
            _statusBar
                .SetupSet(x => x.Text)
                .Callback(msg => Assert.AreEqual(Resources.VimHost_CannotRedo, msg))
                .Verifiable();
            _host.Redo(buffer.Object, 1);
            _statusBar.Verify();
        }

        [Test]
        public void GotoDefinition1()
        {
            Create();
            Assert.IsFalse(_host.GoToDefinition());
        }

        [Test]
        public void GotoDefinition2()
        {
            CreateAll();
            _dte.Setup(x => x.ExecuteCommand(It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception());
            Assert.IsFalse(_host.GoToDefinition());
        }

        [Test]
        public void GotoDefinition3()
        {
            CreateAll();
            _dte.Setup(x => x.ExecuteCommand(It.IsAny<string>(), It.IsAny<string>()));
            Assert.IsTrue(_host.GoToDefinition());
        }

        [Test]
        public void GoToMatch1()
        {
            CreateAll();
            _dte.Setup(x => x.ExecuteCommand(It.IsAny<string>(), It.IsAny<string>()));
            Assert.IsTrue(_host.GoToMatch());
        }

        [Test]
        public void GoToMatch2()
        {
            CreateAll();
            _dte.Setup(x => x.ExecuteCommand(It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception());
            Assert.IsFalse(_host.GoToMatch());
        }

        [Test, Description("Don't fail without VS")]
        public void NavigateTo1()
        {
            Create();
            Assert.IsFalse(_host.NavigateTo(new VirtualSnapshotPoint()));
        }

        [Test]
        public void NavigateTo2()
        {
            CreateAll();
            var buffer = EditorUtil.CreateBuffer("foo", "bar");
            var vsBuffer = new Mock<IVsTextBuffer>(MockBehavior.Strict);
            _editorAdaptersFactoryService.Setup(x => x.GetBufferAdapter(buffer)).Returns(vsBuffer.Object);
            var viewGuid = VSConstants.LOGVIEWID_Code;
            _textManager
                .Setup(x => x.NavigateToLineAndColumn(vsBuffer.Object, ref viewGuid, 0, 2, 0, 2))
                .Returns(0)
                .Verifiable();
            _host.NavigateTo(new VirtualSnapshotPoint(buffer.CurrentSnapshot, 2));
            _textManager.Verify();
        }

        [Test]
        public void GetName1()
        {
            Create();
            var buffer = new Mock<ITextBuffer>();
            _editorAdaptersFactoryService.Setup(x => x.GetBufferAdapter(buffer.Object)).Returns((IVsTextBuffer)null);
            Assert.AreEqual("", _host.GetName(buffer.Object));
        }

        [Test]
        public void GetName2()
        {
            Create();
            var buffer = new Mock<ITextBuffer>(MockBehavior.Strict);
            var vsTextBuffer = (new Mock<IVsTextLines>(MockBehavior.Strict));
            var userData = vsTextBuffer.As<IVsUserData>();
            var moniker = VsVim.Constants.VsUserData_FileNameMoniker;
            object ret = "foo";
            userData.Setup(x => x.GetData(ref moniker, out ret)).Returns(0);
            _editorAdaptersFactoryService.Setup(x => x.GetBufferAdapter(buffer.Object)).Returns(vsTextBuffer.Object);
            Assert.AreEqual("foo", _host.GetName(buffer.Object));
        }


    }
}
