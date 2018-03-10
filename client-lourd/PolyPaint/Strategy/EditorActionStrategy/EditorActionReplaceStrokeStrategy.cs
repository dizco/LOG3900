using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Threading;
using PolyPaint.Helpers.Communication;
using PolyPaint.Models;
using PolyPaint.Models.MessagingModels;

namespace PolyPaint.Strategy.EditorActionStrategy
{
    internal class EditorActionReplaceStrokeStrategy : IEditorActionStrategy
    {
        private static readonly ConcurrentDictionary<string, StrokeCollection> ReceivedReplaceActions =
            new ConcurrentDictionary<string, StrokeCollection>();

        private readonly EditorActionModel _replaceStrokeAction;

        public EditorActionReplaceStrokeStrategy(EditorActionModel action)
        {
            _replaceStrokeAction = action;
        }

        private static bool IsReplacing { get; set; }

        /// <summary>
        ///     Places received replacement actions into the dictionary so that the modification can be applied.
        /// </summary>
        /// <param name="editor">Editor on which the stroke replacement is applied</param>
        public void ExecuteStrategy(Editor editor)
        {
            if (_replaceStrokeAction.Author.Username == editor.CurrentUsername)
            {
                return;
            }

            string[] removedStrokes = _replaceStrokeAction.Delta.Remove;

            StrokeCollection addedStrokes = new StrokeCollection();

            if (_replaceStrokeAction.Delta.Add != null)
            {
                foreach (StrokeModel stroke in _replaceStrokeAction.Delta.Add)
                {
                    addedStrokes.Add(StrokeHelper.BuildIncomingStroke(stroke, _replaceStrokeAction.Author.Username));
                }
            }

            foreach (string removedStroke in removedStrokes)
            {
                ReceivedReplaceActions.TryAdd(removedStroke, addedStrokes);
            }

            if (IsReplacing)
            {
                return;
            }

            Task replaceStrokes = new Task(() => ReplaceStrokesFromDictionary(editor));
            replaceStrokes.Start();
        }

        /// <summary>
        ///     To insure sync between clients, all received ReplaceStroke actions are placed in a Dictionary. This is currently
        ///     necessary since actions are not received in the order in which they are sent in
        ///     The function will iterate over the Dictionary of received actions until it is empty.
        ///     An action is removed from the Dictionary once it is successfully applied to the editor instance.
        ///     A single instance of this loop is executed at a time.
        /// </summary>
        /// <param name="editor">Editor on which the stroke replacement is applied</param>
        private static void ReplaceStrokesFromDictionary(Editor editor)
        {
            IsReplacing = true;

            do
            {
                foreach (KeyValuePair<string, StrokeCollection> replaceAction in ReceivedReplaceActions)
                {
                    try
                    {
                        (Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher).Invoke(() =>
                        {
                            editor
                                .ReplaceStroke(replaceAction.Key, replaceAction.Value);
                        });
                        ReceivedReplaceActions.TryRemove(replaceAction.Key, out _);
                        break;
                    }
                    catch (ArgumentException)
                    {
                        // ignored
                    }
                    catch (InvalidOperationException)
                    {
                        // ignored
                    }
                }
            } while (ReceivedReplaceActions.Count > 0);

            IsReplacing = false;
        }
    }
}
