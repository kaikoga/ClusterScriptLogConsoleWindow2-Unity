using System;
using Silksprite.ClusterScriptLogConsoleWindow2.Repository;
using Silksprite.ClusterScriptLogConsoleWindow2.Utils;
using UnityEngine;
using UnityEngine.UIElements;

namespace Silksprite.ClusterScriptLogConsoleWindow2.Window.LogConsole
{
    public sealed class ScriptLogConsoleView : VisualElement, IDisposable
    {
        readonly ScriptLogListView listView;
        readonly ScriptLogDetailView detailView;
        readonly ScriptLogSourceListView sourceListView;
        readonly ScriptLogStackView stackView;
        readonly ScriptLogToolbarView toolbar;

        readonly TwoPaneSplitView splitView;
        readonly TwoPaneSplitView upperSplitView;
        readonly TwoPaneSplitView lowerSplitView;

        bool hasSourceListItems;
        bool hasStackItems;

        readonly Disposable disposables = new();

        public ScriptLogConsoleView()
        {
            style.flexGrow = 1.0f;
            toolbar = new ScriptLogToolbarView
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    flexShrink = 0
                }
            };
            listView = new ScriptLogListView
            {
                style =
                {
                    minHeight = 48f
                }
            };
            detailView = new ScriptLogDetailView
            {
                style =
                {
                    minHeight = 48f
                }
            };
            sourceListView = new ScriptLogSourceListView
            {
                style =
                {
                    minWidth = 100f
                }
            };
            stackView = new ScriptLogStackView
            {
                style =
                {
                    minWidth = 100f
                }
            };
            splitView = new TwoPaneSplitView
            {
                fixedPaneIndex = 1,
                fixedPaneInitialDimension = Mathf.Clamp(LogConsoleWindowSettingsRepository.Instance.SplitViewSize.Value, 16f, 8192f),
                orientation = TwoPaneSplitViewOrientation.Vertical,
                style =
                {
                    flexGrow = 1.0f
                }
            };
            upperSplitView = new TwoPaneSplitView
            {
                fixedPaneIndex = 1,
                fixedPaneInitialDimension = Mathf.Clamp(LogConsoleWindowSettingsRepository.Instance.UpperSplitViewSize.Value, 16f, 8192f),
                orientation = TwoPaneSplitViewOrientation.Vertical,
                style =
                {
                    flexGrow = 1.0f
                }
            };
            lowerSplitView = new TwoPaneSplitView
            {
                fixedPaneIndex = 0,
                fixedPaneInitialDimension = Mathf.Clamp(LogConsoleWindowSettingsRepository.Instance.LowerSplitViewSize.Value, 16f, 8192f),
                orientation = TwoPaneSplitViewOrientation.Horizontal,
                style =
                {
                    flexGrow = 1.0f,
                    minHeight = 48f
                }
            };
            hierarchy.Add(toolbar);
            hierarchy.Add(splitView);
            splitView.Add(upperSplitView);
            splitView.Add(lowerSplitView);
            upperSplitView.Add(listView);
            upperSplitView.Add(detailView);
            lowerSplitView.Add(sourceListView);
            lowerSplitView.Add(stackView);
        }

        public void Bind(ScriptLogConsoleViewModel viewModel)
        {
            toolbar.Bind(viewModel);
            toolbar.AddTo(disposables);
            listView.Bind(viewModel);
            listView.AddTo(disposables);
            detailView.Bind(viewModel);
            detailView.AddTo(disposables);
            sourceListView.Bind(viewModel.SourceListViewModel);
            sourceListView.AddTo(disposables);
            stackView.Bind(viewModel.StackViewModel);
            stackView.AddTo(disposables);
            viewModel.SourceListViewModel.Items.Subscribe(SourceListUpdated).AddTo(disposables);
            viewModel.StackViewModel.Items.Subscribe(StackUpdated).AddTo(disposables);
        }

        void SourceListUpdated(ScriptLogSourceItemViewModel[] sourceListItems)
        {
            hasSourceListItems = sourceListItems?.Length > 0;
            RefreshLowerSplitViewVisible();
        }

        void StackUpdated(ScriptLogStackItemViewModel[] stackItems)
        {
            hasStackItems = stackItems?.Length > 1;
            RefreshLowerSplitViewVisible();
        }

        void RefreshLowerSplitViewVisible()
        {
            lowerSplitView.visible = hasSourceListItems || hasStackItems;
        }

        public void Dispose()
        {
            disposables.Dispose();
            LogConsoleWindowSettingsRepository.Instance.SplitViewSize.Value = ((IResolvedStyle) splitView.fixedPane).height;
            LogConsoleWindowSettingsRepository.Instance.UpperSplitViewSize.Value = ((IResolvedStyle) upperSplitView.fixedPane).height;
            LogConsoleWindowSettingsRepository.Instance.LowerSplitViewSize.Value = ((IResolvedStyle) lowerSplitView.fixedPane).width;
        }
    }
}
