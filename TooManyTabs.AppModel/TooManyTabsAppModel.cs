using System.Threading.Tasks;
using Get.Data.Collections;
using Get.Data.Collections.Update;
using Get.Data.DataTemplates;
using Get.Data.Properties;
using Get.Data.UIModels;
using Gtudios.UI.Controls.Tabs;
using Gtudios.UI.Windowing;
namespace Gtudios.UI.TooManyTabs.AppModel;
[AutoProperty]
public partial class TooManyTabsAppModel<T> where T : notnull
{
    public IProperty<bool> ShouldAlwaysSelectTabsProperty { get; } = Auto(true);
    public MotionDragConnectionContext<TooManyTabsItem<T>> ConnectionContext { get; } = new();
    
    public async void Start()
    {
        ConnectionContext.DroppedOutside += async (sender, item, dragPosition, dropManager) =>
        {
            if (AllowTabDropNewWindowCreation)
            {
                var def = dropManager.GetDeferral();
                var newWindow = await CreateNewWindowAsync();
                await dropManager.RemoveItemFromHostAsync();
                def.Complete();
                newWindow.TooManyTabsItem.Tabs.Add(item);
                newWindow.TooManyTabsItem.ChildrenSelectionManager.SelectedIndex = 0;
                var pos = dragPosition.MousePositionToScreen;
                newWindow.Window.Bounds = newWindow.Window.Bounds with
                {
                    X = pos.X - 20,
                    Y = pos.Y - 20
                };
                newWindow.Window.Activate();
            }
        };
#if WINDOWS_UWP
        var window = await CreateNewWindowAsync(WindowCoreWindow.CurrentThreadCoreWindow);
#else
        var window = await CreateNewWindowAsync();
#endif
        window.TooManyTabsItem.Tabs.Add(await CreateSingleTabAsync(window.Window));
        window.TooManyTabsItem.ChildrenSelectionManager.SelectedIndex = 0;
        window.Window.Activate();
    }
    public async Task<TooManyTabsWindow<T>> CreateNewWindowAsync(Window? window = null)
    {
        window ??= await CreateWindowOverrideAsync();
        var tmt = CreateTooManyTabsOverride(window);
        tmt.TabStyleTemplate = TabStyleTemplate;
        tmt.ConnectionContext = ConnectionContext;
        tmt.TabClosing += OnTabClosing;
        tmt.IsTabSnapEnabled = true;
        tmt.TabSnapped += TabSnapped;
        tmt.ToolbarTemplate = ToolbarTemplate;
        TooManyTabsMultiItem<T> tabs = new(CreateStyleForMultiItem());
        tmt.TooManyTabsItem = tabs;
        tmt.TabViewHeader = TabViewHeader;
        tmt.TabContentTemplate = TabContentTemplate;
#if WINDOWS_UWP
        if (window.WindowHandle == WindowCoreWindow.CurrentThreadCoreWindow.WindowHandle
            && Windows.UI.Xaml.Window.Current.Content is Page p1)
            p1.Content = tmt;
        else
#endif
        if (window.RootContent is Page p)
            p.Content = tmt;
        else
            window.RootContent = tmt;
        tmt.AddTabButtonVisibility = Visibility.Visible;
        tmt.AddTabButtonClicked += async (tmtmi) =>
        {
            tmtmi.Tabs.Add(await CreateSingleTabAsync(window));
            tmtmi.ChildrenSelectionManager.SelectedIndex = tmtmi.Tabs.Count - 1;
        };
        var w = new TooManyTabsWindow<T>(window, tabs);
        window.Closed += (o, _) =>
        {
            if (o is Window w)
                WindowsMap.Remove(w.WindowHandle);
        };
        tabs.ChildrenSelectionManager.PreferAlwaysSelectItemBinding = OneWay(ShouldAlwaysSelectTabsProperty);
        tabs.Tabs.ItemsChanged += async delegate
        {
            if (CloseWindowOnNoTabs && tabs.Tabs.Count == 0)
            {
                await Task.Delay(300);
                if (tabs.Tabs.Count == 0)
                    CloseWindow(window);
            }
        };
        WindowsMap.Add(window.WindowHandle, w);
        return w;
    }

    private async void TabSnapped(object? sender, TooManyTabsSingleItem<T> snapTarget, TooManyTabsItem<T> droppedTab, TooManyTabsSnapMode snapMode, DropManager dropManager)
    {
        IUpdateCollection<TooManyTabsItem<T>> parentCollection;
        SelectionManager<TooManyTabsItem<T>>? selectionManager = null;
        TooManyTabsItem<T> newItem;
        if (snapTarget.Parent is TooManyTabsMultiItem<T> p)
        {
            parentCollection = p.Tabs;
            selectionManager = p.ChildrenSelectionManager;
        }
        else if (snapTarget.Parent is TooManyTabsStackItem<T> s)
        {
            parentCollection = s.Tabs;
        }
        else
        {
            return;
        }
        await dropManager.RemoveItemFromHostAsync();
        Debug.Assert(droppedTab.Parent is null);
        var idx = parentCollection.IndexOf(snapTarget);
        if (snapMode is not TooManyTabsSnapMode.Center)
        {
            var stack = new TooManyTabsStackItem<T>(CreateStyleForStackItem())
            {
                Orientation = snapMode is TooManyTabsSnapMode.Top or TooManyTabsSnapMode.Bottom ? Orientation.Vertical : Orientation.Horizontal
            };
            parentCollection[idx] = stack; // should remove 'i' from having parent
            Debug.Assert(snapTarget.Parent is null);
            if (snapMode is TooManyTabsSnapMode.Right or TooManyTabsSnapMode.Bottom)
            {
                stack.Tabs.Add(snapTarget);
                stack.Tabs.Add(droppedTab);
            }
            else
            {
                stack.Tabs.Add(droppedTab);
                stack.Tabs.Add(snapTarget);
            }
            newItem = stack;
        }
        else
        {
            
            var multiItem = new TooManyTabsMultiItem<T>(CreateStyleForMultiItem());
            parentCollection[idx] = multiItem; // should remove 'i' from having parent
            Debug.Assert(snapTarget.Parent is null);
            multiItem.Tabs.Add(snapTarget);
            multiItem.Tabs.Add(droppedTab);
            multiItem.ChildrenSelectionManager.SelectedIndex = 1; // select the dropped tab
            newItem = multiItem;
        }
        if (selectionManager is not null)
        {
            selectionManager.SelectedIndex = parentCollection.IndexOf(newItem);
        }
    }
}
