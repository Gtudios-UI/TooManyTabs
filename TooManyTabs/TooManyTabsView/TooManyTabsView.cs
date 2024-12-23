using Get.UI.Data;
using Get.Data.Collections;
using Get.Data.Properties;
using Get.Data.DataTemplates;
using Get.Data.Bindings.Linq;
using Gtudios.UI.MotionDrag;
using Gtudios.UI.Controls.Tabs;
using Get.Data.Bundles;
namespace Gtudios.UI.TooManyTabs;
[AutoProperty]
public partial class TooManyTabsView<T>(TooManyTabsMultiItem<T> root) : TemplateControl<UserControl>
{
    public IProperty<bool> IsTabSnapEnabledProperty { get; } = Auto(false);
    public IProperty<IDataTemplate<T, UIElement>?> TabHeaderTemplateProperty { get; } = Auto<IDataTemplate<T, UIElement>?>(DataTemplates.TextBlockUIElement<T>());
    public IProperty<IDataTemplate<T, UIElement>?> TabContentTemplateProperty { get; } = Auto<IDataTemplate<T, UIElement>?>(DataTemplates.TextBlockUIElement<T>());
    public IProperty<IDataTemplate<T, UIElement>?> ToolbarTemplateProperty { get; } = Auto<IDataTemplate<T, UIElement>?>(default);
    public IProperty<TooManyTabsMultiItem<T>> TooManyTabsItemProperty { get; } = Auto(root);
    public IProperty<MotionDragConnectionContext<TooManyTabsItem<T>>?> ConnectionContextProperty { get; } = Auto<MotionDragConnectionContext<TooManyTabsItem<T>>?>(new());
    public IProperty<IDataTemplate<TooManyTabsMultiItem<T>, UIElement>?> TabViewHeaderProperty { get; } = Auto<IDataTemplate<TooManyTabsMultiItem<T>, UIElement>?>(default);
    public IProperty<Visibility> AddTabButtonVisibilityProperty { get; } = Auto(Visibility.Collapsed);
    public event Action<TooManyTabsMultiItem<T>>? AddTabButtonClicked;
    public event Action<object?, TabClosingRequestEventArgs<TooManyTabsItem<T>>>? TabClosing;
    public event TooManyTabsSnappedEventHandler<T>? TabSnapped;
    public IProperty<IDataTemplate<ITooManyTabsStyle<T>, TabItem<TooManyTabsItem<T>>>> TabStyleTemplateProperty { get; } =
        Auto<IDataTemplate<ITooManyTabsStyle<T>, TabItem<TooManyTabsItem<T>>>>(
            new DataTemplate<ITooManyTabsStyle<T>, TabItem<TooManyTabsItem<T>>>(
                x => new TabItem<TooManyTabsItem<T>>
                {
                    Content = new TextBlock { Text = "" }
                }
            )
        );
    protected override void Initialize(UserControl rootElement)
    {
        var mtv = new TooManyTabsMultiView<T>(
                this,
                TooManyTabsItem
            )
        {
            TooManyTabsMultiItemBinding = OneWay(TooManyTabsItemProperty)
        };
        rootElement.Content = mtv;
        mtv.Initialized += tv => RootTabViewCreated?.Invoke(tv);
    }
    public event Action<TabView<TooManyTabsItem<T>>>? RootTabViewCreated;
    internal void OnAddTabButtonClicked(TooManyTabsMultiItem<T> items)
    {
        AddTabButtonClicked?.Invoke(items);
    }
    internal void OnTabCloseButtonClicked(object? o, TabClosingRequestEventArgs<TooManyTabsItem<T>> e)
    {
        TabClosing?.Invoke(o, e);
    }
    internal void OnTabSnapped(object? sender, TooManyTabsSingleItem<T> snapTarget, TooManyTabsItem<T> droppedTab, TooManyTabsSnapMode snapMode, DropManager dropManager)
    {
        TabSnapped?.Invoke(sender, snapTarget, droppedTab, snapMode, dropManager);
    }
}
public delegate void TooManyTabsSnappedEventHandler<T>(object? sender, TooManyTabsSingleItem<T> snapTarget, TooManyTabsItem<T> droppedTab, TooManyTabsSnapMode snapMode, DropManager dropManager);