﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Vim;
using Vim.Modes;
using System.Windows.Threading;
using Microsoft.FSharp.Core;
using System.Windows.Input;
using NUnit.Framework;

namespace VimCoreTest
{
    internal static class Extensions
    {
        #region CountResult

        internal static CountResult.NeedMore AsNeedMore(this CountResult res)
        {
            return (CountResult.NeedMore)res;
        }

        #endregion

        #region ProcessResult

        internal static ProcessResult.SwitchMode AsSwitchMode(this ProcessResult res)
        {
            return (ProcessResult.SwitchMode)res;
        }

        #endregion

        #region MotionResult


        internal static MotionResult.Complete AsComplete(this MotionResult res)
        {
            Assert.IsTrue(res.IsComplete);
            return (MotionResult.Complete)res;
        }

        internal static MotionResult.InvalidMotion AsInvalidMotion(this MotionResult res)
        {
            Assert.IsTrue(res.IsInvalidMotion);
            return (MotionResult.InvalidMotion)res;
        }

        #endregion

        #region ModeUtil.Result

        internal static Result.Failed AsFailed(this Result res)
        {
            return (Result.Failed)res;
        }

        #endregion

        #region ParseRangeResult

        internal static Vim.Modes.Command.ParseRangeResult.Succeeded AsSucceeded(this Vim.Modes.Command.ParseRangeResult res)
        {
            return (Vim.Modes.Command.ParseRangeResult.Succeeded)res;
        }

        internal static Vim.Modes.Command.ParseRangeResult.Failed AsFailed(this Vim.Modes.Command.ParseRangeResult res)
        {
            return (Vim.Modes.Command.ParseRangeResult.Failed)res;
        }

        #endregion

        #region KeyMappingResult

        internal static Vim.KeyMappingResult.SingleKey AsSingleKey(this KeyMappingResult res)
        {
            Assert.IsTrue(res.IsSingleKey);
            return (KeyMappingResult.SingleKey)res;
        }

        internal static KeyMappingResult.KeySequence AsKeySequence(this KeyMappingResult res)
        {
            Assert.IsTrue(res.IsKeySequence);
            return (KeyMappingResult.KeySequence)res;
        }

        #endregion

        #region SettingValue

        internal static SettingValue.StringValue AsStringValue(this SettingValue value)
        {
            Assert.IsTrue(value.IsStringValue);
            return (SettingValue.StringValue)value;
        }

        internal static SettingValue.ToggleValue AsBooleanValue(this SettingValue value)
        {
            Assert.IsTrue(value.IsToggleValue);
            return (SettingValue.ToggleValue)value;
        }

        internal static SettingValue.NumberValue AsNumberValue(this SettingValue value)
        {
            Assert.IsTrue(value.IsNumberValue);
            return (SettingValue.NumberValue)value;
        }

        #endregion

        #region Range

        internal static Vim.Modes.Command.Range.Lines AsLines(this Vim.Modes.Command.Range range)
        {
            return (Vim.Modes.Command.Range.Lines)range;
        }

        internal static Vim.Modes.Command.Range.RawSpan AsRawSpan(this Vim.Modes.Command.Range range)
        {
            return (Vim.Modes.Command.Range.RawSpan)range;
        }

        internal static Vim.Modes.Command.Range.SingleLine AsSingleLine(this Vim.Modes.Command.Range range)
        {
            return (Vim.Modes.Command.Range.SingleLine)range;
        }

        #endregion

        #region Option

        public static bool IsNone<T>(this FSharpOption<T> option)
        {
            return FSharpOption<T>.get_IsNone(option);
        }

        public static bool IsSome<T>(this FSharpOption<T> option)
        {
            return FSharpOption<T>.get_IsSome(option);
        }

        public static bool HasValue<T>(this FSharpOption<T> option)
        {
            return FSharpOption<T>.get_IsSome(option);
        }

        #endregion

        #region IMode

        public static bool CanProcess(this IMode mode,VimKey key)
        {
            return mode.CanProcess(InputUtil.VimKeyToKeyInput(key));
        }

        public static ProcessResult Process(this IMode mode, VimKey key)
        {
            return mode.Process(InputUtil.VimKeyToKeyInput(key));
        }

        public static ProcessResult Process(this IMode mode, char c)
        {
            return mode.Process((InputUtil.CharToKeyInput(c)));
        }

        public static ProcessResult Process(this IMode mode, string input)
        {
            ProcessResult last = null;
            foreach (var c in input)
            {
                var i = InputUtil.CharToKeyInput(c);
                last = mode.Process(c);
            }

            return last;
        }

        #endregion

        #region IVimBuffer

        public static void ProcessInputAsString(this IVimBuffer buf, string input)
        {
            foreach (var c in input)
            {
                var i = InputUtil.CharToKeyInput(c);
                buf.ProcessInput(i);
            }
        }

        #endregion

        #region ITextView

        public static ITextSnapshotLine GetLine(this ITextView textView, int line)
        {
            return textView.TextSnapshot.GetLineFromLineNumber(line);
        }

        public static SnapshotSpan GetLineSpan(this ITextView textView, int startLine, int endLine)
        {
            return textView.TextSnapshot.GetLineSpan(startLine, endLine);
        }

        public static CaretPosition MoveCaretTo(this ITextView textView, int position)
        {
            return textView.Caret.MoveTo(new SnapshotPoint(textView.TextSnapshot, position));
        }

        #endregion

        #region ITextBuffer

        public static ITextSnapshotLine GetLineFromLineNumber(this ITextBuffer buffer, int line)
        {
            return buffer.CurrentSnapshot.GetLineFromLineNumber(line);
        }

        #endregion

        #region ITextSnapshot

        public static SnapshotSpan GetLineSpan(this ITextSnapshot tss, int startLine, int endLine)
        {
            var start = tss.GetLineFromLineNumber(startLine);
            var end = tss.GetLineFromLineNumber(endLine);
            return new SnapshotSpan(start.Start, end.EndIncludingLineBreak);
        }

        #endregion

        internal static SnapshotSpan GetSpan(this ITextSelection selection)
        {
            var span = new SnapshotSpan(selection.Start.Position, selection.End.Position);
            return span;
        }

        internal static void UpdateValue(this Register reg, string value)
        {
            var regValue = new RegisterValue(value, MotionKind.Inclusive, OperationKind.CharacterWise);
            reg.UpdateValue(regValue);
        }

        internal static SnapshotPoint GetCaretPoint(this ITextView view)
        {
            return view.Caret.Position.BufferPosition;
        }

        internal static void DoEvents(this System.Windows.Threading.Dispatcher dispatcher)
        {
            var frame = new DispatcherFrame();
            Action<DispatcherFrame> action = _ => { frame.Continue = false; };
            dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                action,
                frame);
            Dispatcher.PushFrame(frame);

        }

    }
}
