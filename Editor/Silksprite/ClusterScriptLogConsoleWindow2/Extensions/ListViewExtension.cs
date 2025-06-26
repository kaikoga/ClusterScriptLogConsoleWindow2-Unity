using System;
using ClusterVR.CreatorKit.Translation;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Silksprite.ClusterScriptLogConsoleWindow2.Extensions
{
    public static class ListViewExtension
    {
        public static void RegisterCopyItemCallback<T>(this ListView listView, Func<T, string> stringFactory)
        {
            listView.RegisterCallback<MouseDownEvent>(mouseDownEvent =>
            {
                if (listView.selectedIndex == -1)
                {
                    return;
                }
                switch (mouseDownEvent.button, mouseDownEvent.clickCount)
                {
                    case (1, 1):
                    {
                        var menu = new GenericMenu();
                        menu.AddItem(new GUIContent(TranslationTable.cck_copy_to_clipboard), false, CopyItemToClipboard);
                        menu.ShowAsContext();
                        break;
                    }
                }
            });

            listView.RegisterCallback<KeyDownEvent>(keyDownEvent =>
            {
                if (listView.selectedIndex == -1)
                {
                    return;
                }
    #if UNITY_EDITOR_OSX
                if (keyDownEvent.commandKey && keyDownEvent.keyCode == KeyCode.C)
    #else
                if (keyDownEvent.ctrlKey && keyDownEvent.keyCode == KeyCode.C)
    #endif
                {
                    CopyItemToClipboard();
                }
            });

            void CopyItemToClipboard()
            {
                var item = (T) listView.itemsSource[listView.selectedIndex];
                GUIUtility.systemCopyBuffer = stringFactory(item);
            }
        }
    }
}
