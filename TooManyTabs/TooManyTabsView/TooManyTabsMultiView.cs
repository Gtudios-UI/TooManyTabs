using Get.UI.Data;
using Get.Data.Collections;
using Get.Data.Properties;
using Get.Data.DataTemplates;
using Get.Data.Bindings.Linq;
using Gtudios.UI.Controls.Tabs;
using Get.Data.Bindings;
using Gtudios.UI.MotionDragContainers;
using Get.Data.Helpers;
using System.Collections.Generic;
using Get.Data.Collections.Update;
using Get.Data.XACL;
namespace Gtudios.UI.TooManyTabs;
class TooManyTabsMultiView<T> : TemplateControl<TabView<TooManyTabsItem<T>>>
{
    public Property<TooManyTabsMultiItem<T>> TooManyTabsMultiItemProperty { get; }
    TooManyTabsView<T> _Parent;
    public TooManyTabsMultiView(TooManyTabsView<T> Parent, TooManyTabsMultiItem<T> initial)
    {
        _Parent = Parent;
        TooManyTabsMultiItemProperty = new(initial);
    }
    readonly UpdateCollection<TooManyTabsItem<T>> empty = new();
    readonly ReadOnlyProperty<int> NegativeOne = new(-1);
    protected override void Initialize(TabView<TooManyTabsItem<T>> rootElement)
    {
        void Set(TooManyTabsMultiItem<T> newItem)
        {
            rootElement.PreferAlwaysSelectItemProperty.Value = false;
            rootElement.SelectedIndexProperty.Bind(NegativeOne, ReadOnlyBindingModes.OneWay);
            rootElement.ItemsSourceProperty.Value = newItem.Tabs;
            rootElement.SelectedIndexProperty.Bind(newItem.SelectedIndexProperty, BindingModes.TwoWay);
            rootElement.PreferAlwaysSelectItemProperty.Value = true;
        }
        Set(TooManyTabsMultiItemProperty.Value);
        TooManyTabsMultiItemProperty.ValueChanged += (_, x) => Set(x);
        rootElement.TabItemTemplateProperty.Value = new DataTemplate<SelectableItem<TooManyTabsItem<T>>, MotionDragItem<TooManyTabsItem<T>>>(
            tmti =>
            new TabItem<TooManyTabsItem<T>>
            {
                ContentBundle = new ContentBundle<TooManyTabsItem<T>, UIElement>()
                .WithCustomCode(x =>
                    x.ContentProperty.Bind(
                        tmti
                        .Select(x => x.IndexItemBinding)
                        .Select(x => x.CurrentValue.Value),
                        ReadOnlyBindingModes.OneWay)
                )
                .WithCustomCode(x =>
                    x.ContentTemplateProperty.Value = new DataTemplate<TooManyTabsItem<T>, UIElement>(
                        root => new TextBlock()
                        .WithCustomCode(x =>
                            TextBlock.TextProperty.AsProperty<TextBlock, string>(x)
                            .Bind(root.SelectPath(x => x.NameProperty), ReadOnlyBindingModes.OneWay)
                        )
                    )
                )
            }
        );
        rootElement.TabContentTemplateProperty.Value = new DataTemplate<TooManyTabsItem<T>, UIElement>(
            tmti => new TooManyTabsViewSelector<T>(_Parent, tmti)
        );
    }
}